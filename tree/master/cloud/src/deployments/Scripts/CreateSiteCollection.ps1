param            
(                  
    [parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [string] $Username

    ,[parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [string] $Password
)

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

# Set the helper files folder path, uncomment below line if you want to use this script separately
#$HelperPath = "$ScriptDirectory\Helper Utilities"

$IsValid = Test-Path $ExcelFilePath

if($IsValid -eq $true){
        Show-Message -Message "Provisioning Site Collection"
        & "$HelperPath\Microsoft.Legal.MatterCenter.CreateSiteCollection.exe" "true" $Username $Password
}