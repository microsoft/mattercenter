param            
(                  
    [parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [bool] $IsDeploy
)

function addAppToExchange()
{
	param            
    (      
        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $AppName ,  
              
        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $filePath ,            
            
        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        $users         
	)	
	try
	{
        if (0 -ne $users.Length) {
		    write-host -f green "Adding " $AppName " App to Exchange..."

		    $Data=Get-Content -Path $filePath -Encoding Byte -ReadCount 0
		    $temp = New-App -OrganizationApp -FileData $Data -ProvidedTo SpecificUsers -UserList $users -DefaultStateForUser Enabled

		    write-host -f green "Successfully added " $AppName " App to Exchange"
        }
        else {
            write-host -f yellow "Skipping add " $AppName " App to Exchange"
        }
    }
    catch [Exception]
    {
        removeAllApps $AppName
		write-host -f red "Failed to add an app"
        Write-Log $ErrorLogFile $_.Exception.ToString()
    }
}

function removeAppFromExchange()
{	
    param            
    (            
        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $AppName
    )
	try
	{
        $Apps = Get-App -OrganizationApp
		$IsAppPresent= $False;
        foreach ($App in $Apps)
        {
          if ($App.DisplayName.Equals($AppName))
          {
            $IsAppPresent = $true;
            $ID = $App.AppId
            $Name = $App.DisplayName
            Write-Host "Removing " $Name " from Exchange..." -ForegroundColor Yellow
            Remove-App -Identity $ID -Confirm:$False -OrganizationApp
            Write-host -ForegroundColor Green "Removed" $Name "app from Exchange.."
            Write-Host "Successfully Removed Apps from Exchange..." -ForegroundColor Green
            break
          }

        }
        if(!$IsAppPresent)
        {
            Write-Host "Exchange App not found" -ForegroundColor Green
        }
    }
    catch [Exception]
    {
		write-host -f red "Failed to remove an app"
        Write-Log $ErrorLogFile $_.Exception.ToString()
    }
}

function removeAllApps()
{
    param            
    (            
        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        $AppDetails
    )

	foreach ($App in $AppDetails)
	{
		removeAppFromExchange -AppName $App
	}
 }

# Get the current directory of the script
Function ScriptRoot {Split-Path $MyInvocation.ScriptName}
$ScriptDirectory = (ScriptRoot)

# Get the parent directory of the script
Function Get-ParentDirectory {Split-Path -Parent(Split-Path $MyInvocation.ScriptName)}
$ParentDirectory = (Get-ParentDirectory)

#Set Excel file path, uncomment below line if you want to use this script seperately
#$ExcelFilePath = "$ParentDirectory\MCDeploymentConfig.xlsx"
    
# Set log file path, uncomment below line if you want to use this script seperately
#$ErrorLogFile = "$ScriptDirectory\ErrorLog.txt"

write-host -f green "Connecting to Exchange..."
if($credentials -eq $null){
	$Cred = Get-Credential
}

$appDirectory = Join-Path $ParentDirectory "Exchange App\"
$AppFiles = Get-ChildItem –Path $appDirectory # Get the .app files
$AppNames = "Matter Center Beta" # App Names
$AppNames = $AppNames.Split(';')

Write-Host "Reading inputs from Excel..." -ForegroundColor Yellow
$ExcelValues = Read-FromExcel $ExcelFilePath "Config" ("ExchangePowerShellURL") $ErrorLogFile
$sheetData = ReadSheet-FromExcel $ExcelFilePath "Create_Group" $ErrorLogFile
$ExcelValues = $ExcelValues.Split(";")
if($ExcelValues.length -le 0)
{
    Write-Log $ErrorLogFile "Error reading values from Excel file. Aborting!"
    return $false
}
$MatterCenterApps = ""
[string]$ExchangeURL = $ExcelValues[0]

for($iIterator=0; $iIterator -le $sheetData.length-1; $iIterator++) {
    if("Matter Center Users" -eq $sheetData[$iIterator][0]) {
        $MatterCenterApps = $sheetData[$iIterator][3];
    }
}

# Prerequisites for On Premise Deployment
# Run Enable-PsRemoting Power shell command on exchange CAS server
# Make Sure On Exchange Server at Virtual Directories, for PowerShell(Default Web Site), Authentication is set to Basic
# Certificate of Exchange server is installed in the client user machine
# New-PSSession: Creates a persistent connection to a local or remote computer

try
{
    Write-Host "Creating new session..." -ForegroundColor Yellow
    $sessionOption = New-PSSessionOption -SkipRevocationCheck #Does not validate the revocation status of the server certificate
    $s = New-PSSession -ConfigurationName Microsoft.Exchange -ConnectionUri $ExchangeURL -Credential $credentials -Authentication Basic –AllowRedirection -SessionOption $sessionOption
    if($null -eq $s)
    {
        Write-Host "Error in creating session... Connection to Exchange failed..." -ForegroundColor Red
        return
    }
    
    Write-Host "Session created..." -ForegroundColor Green
    
    Write-Host "Connecting to Exchange..." -Foreground Yellow
    Import-PSSession $s -AllowClobber # Import all the commands
}
catch [Exception]
{
    Write-Host "Unable to connect to Exchange server..." -ForegroundColor Red
    Write-Log $ErrorLogFile $_.Exception.ToString()
}

if ($IsDeploy)
{
  Write-Host "Deploying Apps on Exchange..." -ForegroundColor Yellow
  if ($null -ne $MatterCenterApps) {
      $filterUser= $MatterCenterApps.TrimEnd(';').Split(';', [System.StringSplitOptions]::RemoveEmptyEntries).Trim();
      addAppToExchange -AppName $AppFiles[0].Name -filePath $AppFiles[0].FullName -users $filterUser;
  }
  else {
      write-host -f yellow "No users specified for whom Matter 365 App is to be deployed"
  }
}
else
{
    Write-Host "Removing Apps from Exchange..." -ForegroundColor Yellow
    removeAllApps $AppNames
   
}

Write-Host "Releasing connection with Exchange server..." -ForegroundColor Green
Remove-PSSession $s