param            
(                  
    [parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [int] $Checkpoint

    ,[parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [string] $ScriptDirectory
)

Show-Message -Message ""
Show-Message -Message "Reverting till step $Checkpoint" -Type ([MessageType]::Warning)

# Get the current directory of the script
Function ScriptRoot {Split-Path $MyInvocation.ScriptName}
$ScriptDirectory = (ScriptRoot)

# Get the parent directory of the script
Function Get-ParentDirectory {Split-Path -Parent(Split-Path $MyInvocation.ScriptName)}
$ParentDirectory = (Get-ParentDirectory)

# Global Variables to be used throughout the scripts
#Set Excel file path
$ExcelFilePath = "$ParentDirectory\MCDeploymentConfig.xlsx"
    
# Set log file path
$ErrorLogFile = "$ScriptDirectory\Logs\ErrorLog.txt"

# Set revert log file path
$RevertLogFile = "$ScriptDirectory\Logs\RevertLog.txt"

# Set helper utilities folder path
$HelperPath = "$ScriptDirectory\Helper Utilities"

Function RevertAll($ScriptDirectory, $Checkpoint) {
        . "$ScriptDirectory\Revert.ps1" -Checkpoint: $Checkpoint -ScriptDirectory: $ScriptDirectory
}

#----------------------------------------------
# Clear all existing logs from error file or creates a new file
#----------------------------------------------

Show-Message -Message "Clearing all the previous logs..." -Type ([MessageType]::Warning)
"" | Out-File $ErrorLogFile
Clear-Content $ErrorLogFile

"" | Out-File $RevertLogFile
Clear-Content $RevertLogFile

Show-Message -Message "All previous logs cleared" -Type ([MessageType]::Success)

#----------------------------------------------
# Add SharePoint Client DLLs
#----------------------------------------------
Show-Message -Message "Adding SharePoint libraries..." -Type ([MessageType]::Warning)
if ((Test-Path -Path "$HelperPath\Microsoft.SharePoint.Client.dll") -and (Test-Path -Path "$HelperPath\Microsoft.SharePoint.Client.Runtime.dll") -and (Test-Path -Path "$HelperPath\Microsoft.SharePoint.Client.Search.dll")) {
    Add-Type -Path (Resolve-Path "$HelperPath\Microsoft.SharePoint.Client.dll")
    Add-Type -Path (Resolve-Path "$HelperPath\Microsoft.SharePoint.Client.Runtime.dll")
	Add-Type -Path (Resolve-Path "$HelperPath\Microsoft.SharePoint.Client.Search.dll")
	Show-Message -Message "Added SharePoint libraries" -Type ([MessageType]::Success)
}
else {
    Write-Log $ErrorLogFile "Unable to load SharePoint libraries..."
    return $false
}

#----------------------------------------------
# Include Common functions script
#----------------------------------------------

Show-Message -Message "Adding common library functions" -Type ([MessageType]::Warning)
. "$ScriptDirectory\LibraryFunctions.ps1"
Show-Message -Message "Added common library functions" -Type ([MessageType]::Success)

#----------------------------------------------
# Reverting step 13
#----------------------------------------------
if (13 -le $Checkpoint)
{
    Show-Message -Message ""
    Show-Message -Message "Deleting files from SharePoint library"
    try {
        & "$HelperPath\Microsoft.Legal.MatterCenter.UploadFile.exe" "false" $Username $Password
    }
    catch {        
        Write-Log $RevertLogFile "Could not delete files from SharePoint library"
    }
}

#----------------------------------------------
# Reverting step 12
#----------------------------------------------
if (12 -le $Checkpoint)
{
    Show-Message -Message ""
    Show-Message -Message "Removing apps from Exchange"
    try {
        . "$ScriptDirectory\DeployOutlookApp.ps1" -IsDeploy: $false
    }
    catch {        
        Write-Log $RevertLogFile "Could not remove apps from Exchange"
    }
}

#----------------------------------------------
# Reverting step 11
#----------------------------------------------
if (11 -le $Checkpoint)
{
    Show-Message -Message ""
    Show-Message -Message "Removing apps from SharePoint and Office"
    try {
        . "$ScriptDirectory\AppInstall.ps1" -IsDeploy: $false
        . "$ScriptDirectory\DeployOfficeApp.ps1" -IsDeploy: $false -IsOfficeApp: $false
        . "$ScriptDirectory\DeployOfficeApp.ps1" -IsDeploy: $false -IsOfficeApp: $true
    }
    catch {        
        Write-Log $RevertLogFile "Could not remove apps from SharePoint and Office"
    }
}

#----------------------------------------------
# Reverting Step 9
#----------------------------------------------
if (9 -le $Checkpoint)
{
    Show-Message -Message ""
    Show-Message -Message "Undoing Encryption"
    . "$ScriptDirectory\EncryptDecrypt.ps1" -ToEncrypt: $false -ErrorLogPath: $RevertLogFile
}

#----------------------------------------------
# Reverting step 3
#----------------------------------------------
if (3 -le $Checkpoint)
{
    Show-Message -Message ""
    Show-Message -Message "Deleting Site columns and Content types"
    try {
        & "$HelperPath\Microsoft.Legal.MatterCenter.CreateContentTypes.exe" "false" $Username $Password
        & "$HelperPath\Microsoft.Legal.MatterCenter.CreateSiteColumns.exe" "false" $Username $Password
    }
    catch {        
        Write-Log $RevertLogFile "Could not delete Site columns and Content types"
    }
}

#----------------------------------------------
# Reverting step 2
#----------------------------------------------
if (2 -le $Checkpoint)
{
    Show-Message -Message ""
    Show-Message -Message "Deleting Taxonomy hierarchy"
    try {
        & "$HelperPath\Microsoft.Legal.MatterCenter.CreateTerm.exe" "false" $Username $Password
    }
    catch {        
        Write-Log $RevertLogFile "Could not delete Taxonomy hierarchy"
    }
}

#----------------------------------------------
# Reverting step 1
#----------------------------------------------
if (1 -le $Checkpoint)
{
    Show-Message -Message ""
    Show-Message -Message "Deleting configuration lists"
    try {
        & "$HelperPath\Microsoft.Legal.MatterCenter.ConfigureLists.exe" "true" $Username $Password
    }
    catch {        
        Write-Log $RevertLogFile "Could not delete configuration lists"
    }

    Show-Message -Message ""
    Show-Message -Message "Deleting groups"
    try {
        & "$HelperPath\Microsoft.Legal.MatterCenter.CreateGroups.exe" "false" $Username $Password
    }
    catch {        
        Write-Log $RevertLogFile "Could not delete groups"
    }
}

If ($Null -eq (Get-Content $RevertLogFile)) {
	Show-Message -Message "Revert completed successfully" -Type ([MessageType]::Success)
}
else {
	Show-Message -Message "Revert failed" -Type ([MessageType]::Failure)
}
Show-Message -Message ""
Show-Message -Message "Deployment failed" -Type ([MessageType]::Failure)
Show-Message -Message "Deployment logs can be found at $ScriptDirectory\Logs\ErrorLog.txt" -Type ([MessageType]::Warning)
Show-Message -Message "Reverting operation logs can be found at $ScriptDirectory\Logs\RevertLog.txt" -Type ([MessageType]::Warning)