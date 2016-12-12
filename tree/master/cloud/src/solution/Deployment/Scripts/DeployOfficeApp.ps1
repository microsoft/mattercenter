param            
(                  
    [parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [bool] $IsDeploy
)

# Function is used to remove app from specified URL
function removeAppFromOffice()
{
param            
    (                  
        [parameter(Mandatory=$true)]                         
        [ValidateNotNullOrEmpty()]
        [String] $webUrl,

        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $DocLibName,

        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $FolderPath,

        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $FileName

    )

    try
    {
        $context = New-Object Microsoft.SharePoint.Client.ClientContext($webUrl)
        
        if($SPCredential -eq $null)
        {
            $SPCredential = Get-Credential -Message "Enter credentials to access SharePoint tenant."
        }

        $onlinecredential = New-Object Microsoft.SharePoint.Client.SharePointOnlineCredentials($SPCredential.UserName, $SPCredential.Password)
        
        $context.Credentials = $onlinecredential 
        $List = $context.Web.Lists.GetByTitle($DocLibName)
        $context.Load($List)
        $context.Load($List.RootFolder)
        $context.Load($List.RootFolder.Files)
        $context.ExecuteQuery()
        
        foreach( $Folder in $List.RootFolder)
        {
            foreach ($files in $Folder.Files) 
            {
                if($FileName -Eq $files.name)
                {
                    $FileToRemove = $files
                }
            }
            
            try
            {
                $FileToRemove.DeleteObject()
                $context.ExecuteQuery()
				Show-Message -Message "$FileToRemove.name  has been deleted" -Type ([MessageType]::Success)
            }
            catch [Exception]
            {
				Show-Message -Message "An Error Occurred while trying to remove the Apps" -Type ( [MessageType]::Failure )
                Write-Log $ErrorLogFile $_.Exception.ToString()
            }
        }
    }
    catch [Exception]
    {
		Show-Message -Message "Failed to access the app library or context" -Type ( [MessageType]::Failure )
        Write-Log $ErrorLogFile $_.Exception.ToString()
    }
    finally {
        $context.Dispose()
    }
}


# Function is used to loop through all app files and calls remove Office App function to remove individual apps
function removeAppsFromOffice()
{
param            
    (                  
        [parameter(Mandatory=$true)]                         
        [ValidateNotNullOrEmpty()]
        [String] $webUrl,

        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $DocLibName,

        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $FolderPath

    )

    #Remove Apps from App Catalog
    Foreach ($File in (dir $FolderPath))
    {
        removeAppFromOffice -webUrl $webUrl -DocLibName $DocLibName -FolderPath $FolderPath -FileName $File.Name        
    }
}

# function is used to add apps to the specified URL
function addAppsToOffice()
{
param            
    (                  
        [parameter(Mandatory=$true)]                         
        [ValidateNotNullOrEmpty()]
        [String] $webUrl,

        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $DocLibName,

        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $FolderPath

    )

    try
    {
		Show-Message -Message "Fetching Client Context" -Type ( [MessageType]::Warning )
		$Assembly = [Reflection.Assembly]::LoadFile(“$ExternalPath\Microsoft.SharePoint.Client.dll”)
        $context = New-Object Microsoft.SharePoint.Client.ClientContext($webUrl)

        if($SPCredential -eq $null)
        {
            $SPCredential = Get-Credential -Message "Enter credentials to access SharePoint tenant."
        }

        $onlinecredential = New-Object Microsoft.SharePoint.Client.SharePointOnlineCredentials($SPCredential.UserName, $SPCredential.Password)
        
        $context.Credentials = $onlinecredential
    
		Show-Message -Message "Fetching the Library context" -Type ( [MessageType]::Warning )
        $List = $context.Web.Lists.GetByTitle($DocLibName)
    
        $context.Load($List)
        $context.ExecuteQuery()
		Show-Message -Message "Successfully retrieved the Library context" -Type ([MessageType]::Success)
        try
        {
            Foreach ($File in (dir $FolderPath))
            {
                $FileStream = New-Object IO.FileStream($File.FullName, [System.IO.FileMode]::Open)
                $FileCreationInfo = New-Object Microsoft.SharePoint.Client.FileCreationInformation
                $FileCreationInfo.Overwrite = $true
                $FileCreationInfo.ContentStream = $FileStream
                $FileCreationInfo.URL = $File
                $Upload = $List.RootFolder.Files.Add($FileCreationInfo)
                $Context.Load($Upload)
                $Context.ExecuteQuery()
                $fileName = split-path $File -Leaf
				Show-Message -Message "$fileName  has been successfully added" -Type ([MessageType]::Success)
                $FileStream.Dispose()
            }
        }
        catch [Exception]
        {
			Show-Message -Message "Failed to add an app" -Type ( [MessageType]::Failure )
            Write-Log $ErrorLogFile $_.Exception.ToString()
            #revert all changes
            Foreach ($File in (dir $FolderPath))
            {
                removeAppFromOffice -webUrl $webUrl -DocLibName $DocLibName -FolderPath $FolderPath -FileName $File.Name        
            }
        }
    }
    catch [Exception]
    {
		Show-Message -Message "Failed to access the app library or context" -Type ( [MessageType]::Failure )
        Write-Log $ErrorLogFile $_.Exception.ToString()
    }
    finally {
        $context.Dispose()
    }
}

# Get the current directory of the script
Function ScriptRoot {Split-Path $MyInvocation.ScriptName}
$ScriptDirectory = (ScriptRoot)

# Get the parent directory of the script
Function Get-ParentDirectory {Split-Path -Parent(Split-Path $MyInvocation.ScriptName)}
$ParentDirectory = (Get-ParentDirectory)

# Set  folder paths
$RootPath = Split-Path(Split-Path (Split-Path $PSScriptRoot -Parent) -Parent) -Parent
$DeployPath = "$RootPath\deployments"
$HelperPath = "$RootPath\deployments\scripts\Helper Utilities"
$ExternalPath = "$RootPath\Helper Utilities\External"

#Set Excel file path, uncomment below line if you want to use this script separately
#$ExcelFilePath = "$ParentDirectory\MCDeploymentConfig.xlsx"
#$ExcelFilePath = "C:\Repos\mattercenter2\tree\master\cloud\src\deployments\deployments\MCDeploymentConfig.xlsx"
    
# Set log file path, uncomment below line if you want to use this script separately
#$ErrorLogFile = "$ScriptDirectory\Logs\ErrorLog.txt"
# Set error log file path
$ErrorLogFile = "$ScriptDirectory\Logs\ErrorLog.txt" 

#App Catalog URL for Office Apps
$DocLibName = "Apps for Office"

#Fetch the Folder containing the XML files for Office Apps

$FolderPath = Join-Path $DeployPath "Office App"



#Fetch the Catalog URL from the Excel File
Show-Message -Message "Fetching the Catalog URL from the Configuration File" -Type ( [MessageType]::Warning )
$ExcelValues = Read-FromExcel $ExcelFilePath "Config" ("CatalogSiteURL") $ErrorLogFile

if($ExcelValues -is [system.string]){
        $ExcelValues = $ExcelValues.Split(";")
}
												

if($ExcelValues.length -le 0)
{
    Write-Log $ErrorLogFile "Error reading values from Excel file. Aborting!"
    return $false
}
$webUrl = $ExcelValues[0]
if($webUrl -ne $null)
{
	Show-Message -Message "Successfully retrieved the Catalog URL" -Type ([MessageType]::Success)
}

# Check whether app is to be added or removed
if ($IsDeploy)
{
	Show-Message -Message "Proceeding to add the apps to the App Catalog" -Type ( [MessageType]::Warning ) 
    #Add Apps to the App Catalog
    addAppsToOffice -webUrl $webUrl -DocLibName $DocLibName -FolderPath $FolderPath
}
else
{
	Show-Message -Message "Proceeding to remove the apps from the App Catalog" -Type ( [MessageType]::Warning )
    #Remove all apps from App Catalog
    removeAppsFromOffice -webUrl $webUrl -DocLibName $DocLibName -FolderPath $FolderPath
}
