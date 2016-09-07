#Script to check Pre-requisites before deployment

#Global Variable
$objSheet
[bool] $IsValid = $false
[string] $SharePointURL = ''
[string] $PublishingFeatureGuid = "f6924d36-2fa8-4f0b-b16d-06b7250180fa"

#--------------------------------------------------------

#Global Functions
#-------------------------------------------------------
Function Test-Key([string]$Path)
{
    if(Test-Path -Path $Path) { return $true}
    return $false
}

# Function is used to display Error Information
Function DisplayErrorInfo([string]$Info)
{    
    $IsValid = $false    
    Write-Log $ErrorLogFile $Info
}

#.NET FRAMEWORK CHECK Start
#--------------------------------------------------

Function Test-FrameWorkVersion([string]$Path, [string]$Key)
{
  if(Test-Key $Path){$KeyValue=(Get-ItemProperty $Path);return $KeyValue.$Key} 
}

Function DisplayNETFRAMEWORKVersion
{  
  if((Test-FrameWorkVersion 'HKLM:\Software\Microsoft\NET Framework Setup\NDP\v4\Client' 'Install') -ne $null)
  {            
        return $true
  }
  
  if((Test-FrameWorkVersion 'HKLM:\Software\Microsoft\NET Framework Setup\NDP\v4\Full' 'Install') -ne $null)
  {           
        return $true
  }    
  
  DisplayErrorInfo '.NET Framework 4.0 or above is not installed'
  return $false
}

#.NET FRAMEWORK CHECK End
#--------------------------------------------------

#Function to check IIS version

#IIS Check Start
#--------------------------------------------------------

Function DisplayIISVersion([string]$Path,[string]$Key)
{
 if(Test-Key $Path) {
   $KeyValue=(Get-ItemProperty $Path)      
    Switch($KeyValue.$Key)
        { 
         'IIS 7.0'{return $true}
         'IIS 7.5'{return $true}
         'IIS 8.0'{return $true}
         'IIS 8.5'{return $true} 
          default
          { 
                DisplayErrorInfo 'IIS is Installed, but version is less than 7.0' 
                return $false
          }     
        }   
  }
  else  
  {
        DisplayErrorInfo 'IIS is not Installed' 
        return $false
  }
}

#Function to check SharePoint Version

Function CheckSharePointVersion([string]$URL)
{
    try 
    {
        [string]$SharePointVersion = ''    
            
        # Create request object
        $RequestObject=[system.Net.HttpWebRequest]::Create("$URL/_vti_pvt/services.cnf")     
    
        # Set credentials to the request object    
        $RequestObject.Credentials = new-object System.Net.NetworkCredential($SPCredential.UserName, $SPCredential.Password)        
        
        # Get response for the URL specified
        $ResponseObject = $RequestObject.getresponse()    
    
        # Loop through the response headers to find MicrosoftSharePointTeamServices header
        foreach ($HeaderKey in $ResponseObject.Headers) {    
            $HeaderStr = $ResponseObject.Headers[$HeaderKey]
            if ($HeaderStr) {            
               if ($HeaderKey.Equals("MicrosoftSharePointTeamServices")) { #Check if the header is the required one
                    $SharePointVersion = $HeaderStr  
				    Show-Message -Message "SharePoint Version is: $SharePointVersion" -Type ([MessageType]::Success)
                    $Version = [Int32]$SharePointVersion.ToString().Substring(0,2) # Extract the SharePoint Version of the site
                    if($Version -ge 15) {
                        return $true
                    }
                    else {                        
                        Write-Log $ErrorLogFile "Current Version of SharePoint is not SharePoint 2013 for $URL"
                        return $false
                    }
                 }            
              }
           }
        $ResponseObject.Close()
    }
    catch 
    {
        $ErrorMessage = $Error[0].Exception.ErrorRecord.Exception.Message             
        Write-Log $ErrorLogFile $ErrorMessage
        return $false
    }    
}

# Function is used to Test whether command is present in Command list of PowerShell
function Test-Command {
   
   param(

   [parameter(Mandatory=$false)]
   [ValidateNotNullOrEmpty()]
   [string]$Command
   
   )
 
   [bool]$IsFound = $false
   $Match = [Regex]::Match($Command, "(?<Verb>[a-z]{3,11})-(?<Noun>[a-z]{3,})", "IgnoreCase")
   if($Match.Success) {
       if(Get-Command -Verb $Match.Groups["Verb"] -Noun $Match.Groups["Noun"]) {
           $IsFound = $true
       }
   }
 
   return $IsFound
}

# Get the current directory of the script
Function ScriptRoot {Split-Path $MyInvocation.ScriptName}
$ScriptDirectory = (ScriptRoot)

# Get the parent directory of the script
Function Get-ParentDirectory {Split-Path -Parent(Split-Path $MyInvocation.ScriptName)}
$ParentDirectory = (Get-ParentDirectory)

#Set Excel file path, uncomment below line if you want to use this script separately
#$ExcelFilePath = "$ParentDirectory\MCDeploymentConfig.xlsx"
    
# Set log file path, uncomment below line if you want to use this script separately
#$ErrorLogFile = "$ScriptDirectory\Logs\ErrorLog.txt"

Show-Message -Message "Running Pre-requisites check..."
Show-Message -Message ""
Show-Message -Message "Checking Excel file existence" 

Show-Message -Message $ExcelFilePath

$IsValid = Test-Path $ExcelFilePath

#Perform check for Excel Configuration file existence if previous check is successful
if($IsValid -eq $true){
    $ExcelValues = ""
	Show-Message -Message "Configuration Excel file exists" -Type ([MessageType]::Success)
    Show-Message -Message "Reading parameters from Excel file..."    
    $ExcelValues = (Read-FromExcel $ExcelFilePath "Config" ("TenantURL") $ErrorLogFile)
    $ExcelValues = $ExcelValues.Split(";")
    if($ExcelValues.length -le 0)
    {
        Write-Log $ErrorLogFile "Error reading values from Excel file. Aborting!"
        return $false
    }
    $SharePointURL = $ExcelValues[0]   
}
else
{    
    Write-Log $ErrorLogFile "Configuration Excel file is missing"
}

if($IsValid -eq $true)
{     
	 Show-Message -Message "All the parameters are retrieved successfully" -Type ([MessageType]::Success)
     
     #Start with first check   
     Show-Message -Message ""
     Show-Message -Message "Running .NET Framework check..."

     #Perform check for .NET Framework Version
     $IsValid = DisplayNETFRAMEWORKVersion
}

# Perform IIS version check if previous check is successful
if($IsValid -eq $true){
	Show-Message -Message ".NET Framework Version 4.0 or greater exists" -Type ([MessageType]::Success)    
}

# Perform SharePoint version check if previous check is successful
if($IsValid -eq $true)
{    

    Show-Message -Message ""
    Show-Message -Message "Running SharePoint Version check for $SharePointURL..."

    $IsValid = CheckSharePointVersion $SharePointURL
}

if($IsValid -eq $true)
{
	Show-Message -Message "Current SharePoint version is 2013" -Type ([MessageType]::Success)
       Show-Message -Message ""
       Show-Message -Message "Running check for SharePoint Online library..." 
       $IsValid = Test-Command "Connect-SPOService"
       if(!($IsValid))
       {
		   Show-Message -Message "PowerShell SharePoint Online Library is not present, you can download it from following URL: http://www.microsoft.com/en-us/download/confirmation.aspx?id=35588" -Type ([MessageType]::Failure)
       }
}

# Final check to see whether all checks have completed successfully or error has been encountered
if($IsValid -eq $true)
{    

		Show-Message -Message "PowerShell SharePoint Online library is present" -Type ([MessageType]::Success)
}

# Check for Azure PowerShell cmdlets
if($IsValid -eq $true)
{  
	Show-Message -Message ""
	Show-Message -Message "Running check for Azure PowerShell..." 
	$IsValid = Test-Command "Get-AzureWebsite"
	if(!($IsValid))
	{
		Show-Message -Message "Azure PowerShell is not present, you can download it from following URL: http://go.microsoft.com/?linkid=9811175 " -Type ([MessageType]::Failure)
	}
	else
	{
		Show-Message -Message "Azure PowerShell is present" -Type ([MessageType]::Success)
	}
}

if($IsValid -eq $true)
{
    Show-Message -Message ""
    Show-Message -Message "Running check for Workflow Service Application Check..."
 }