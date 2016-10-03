# Message Types 
Add-Type -TypeDefinition @"
	public enum MessageType
	{
		Success,
		Warning,
		Failure
	}
"@

# Get the current directory of the script
Function ScriptRoot {Split-Path $MyInvocation.ScriptName}
$ScriptDirectory = (ScriptRoot)

# Get the parent directory of the script
Function Get-ParentDirectory {Split-Path -Parent(Split-Path $MyInvocation.ScriptName)}
$ParentDirectory = (Get-ParentDirectory)


#----------------------------------------------
# Include Common functions script
#----------------------------------------------

Show-Message -Message "Adding common library functions" -Type ([MessageType]::Warning)
"$ScriptDirectory\LibraryFunctions.ps1"
Show-Message -Message "Added common library functions" -Type ([MessageType]::Success)


#Create Log folder if not exist
$LogFolder = "$ScriptDirectory\Logs"
If (-not (Test-Path -Path $LogFolder -PathType Container))
{ 
	New-Item -Path $LogFolder -ItemType directory -Force 
}

# Set error log file path
$ErrorLogFile = "$ScriptDirectory\Logs\ErrorLog.txt" 

if (!(Test-Path "$ErrorLogFile"))
{
	New-Item -path "$ErrorLogFile" -type "file" -value ""	  
}

# Set log file path
$LogFile = "$ScriptDirectory\Logs\Log.txt"



# Function to display message on console
Function Show-Message([string] $Message, [string] $Type, [bool] $Newline = $true)
{
	$timestamp = Get-Date -Format G
	$Message = $timestamp + " - " + $Message
	switch ($Type)
	{
		([MessageType]::Success)
		{ 
		if($Newline) {
			Write-Host $Message -ForegroundColor Green
			}
			else {
			Write-Host $Message -ForegroundColor Green -NoNewline
			}
		}
		([MessageType]::Warning) 
		{ 
			if($Newline) {
				Write-Host $Message -ForegroundColor Yellow     
			}
			else {
				Write-Host $Message -ForegroundColor Yellow -NoNewline
			}
		}
		([MessageType]::Failure)
		{
			if($Newline) { 
				Write-Host $Message -ForegroundColor Red 
			}
			else {
				Write-Host $Message -ForegroundColor Red -NoNewline
			}
		}
		Default { Write-Host $Message -ForegroundColor White }
	}
	# Write into log file
	if(-not [String]::IsNullOrEmpty($Message)) {
		($Message) | Out-File $LogFile -Append
	}
}


Function Deploy-SPOFiles
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    Param
    (
	    [Parameter(Mandatory=$true)]
        [String]$WebSiteName,
        [Parameter(Mandatory=$true)]
        [String]$UserName,
	    [Parameter(Mandatory=$true)]
        [String]$PassWord
    )  
	
	cd $HelperPath
	
	#---------------------------------------------------------------------
	# Upload files required for Matter landing page to SharePoint library
	#---------------------------------------------------------------------
	Show-Message -Message "Upload files to SharePoint Library"
	[Environment]::CurrentDirectory = Get-Location
	& "$HelperPath\Microsoft.Legal.MatterCenter.UploadFile.exe" "true" $UserName $Password $WebSiteName $global:appInsightsId

	If ((Get-Content $ErrorLogFile) -ne $Null) {
		Show-Message -Message "Uploading files to SharePoint Library failed" -Type ([MessageType]::Failure)    
		return
	}
	else {
		Show-Message -Message "Completed uploading files to SharePoint library" -Type ([MessageType]::Success)
	}
    
    #---------------------------------------------------------------------
    # Provisioning Web Dashboard page(s) on SharePoint library
    #---------------------------------------------------------------------
    Show-Message -Message "Provisioning Web dashboard"
    & "$HelperPath\Microsoft.Legal.MatterCenter.ProvisionWebDashboard.exe" "true" $Username $Password $WebSiteName

    If ((Get-Content $ErrorLogFile) -ne $Null) {
		Show-Message -Message "Provisioning Web dashboard failed" -Type ([MessageType]::Failure)  
    }
    else {
		Show-Message -Message "Completed Provisioning Web dashboard" -Type ([MessageType]::Success)
    }

	cd $PSScriptRoot
    
}

$UIUrl = [string]::format("https://{0}.azurewebsites.net", $WebAppName)
Deploy-SPOFiles -WebSiteName $UIUrl  -UserName $SPCredential.UserName -PassWord $SPPassword 


#----------------------------------------------
# Update Office, Outlook and SharePoint App schema files
#----------------------------------------------
cd $HelperPath
Show-Message -Message "Step : Update Office, Outlook and SharePoint App schema files"
& "$HelperPath\Microsoft.Legal.MatterCenter.UpdateAppConfig.exe" "1" $SPCredential.UserName $SPPassword $UIUrl

If ((Get-Content $ErrorLogFile) -ne $Null) {
	Show-Message -Message "Updating Office, Outlook and SharePoint App schema files failed" -Type ([MessageType]::Failure)
    return
}
else {
	Show-Message -Message "Completed updating Office, Outlook and SharePoint App schema files" -Type ([MessageType]::Success)
}

cd $PSScriptRoot


#----------------------------------------------
# Add Apps to SharePoint and Office
#----------------------------------------------
#Show-Message -Message "Step : Add and install apps to SharePoint and Office"
#. "$ScriptDirectory\AppInstall.ps1" -IsDeploy: $false
#. "$ScriptDirectory\DeployOfficeApp.ps1" -IsDeploy: $true
#. "$ScriptDirectory\AppInstall.ps1" -IsDeploy: $true
    
#If ((Get-Content $ErrorLogFile) -ne $Null) {
#	Show-Message -Message "Adding and installing apps to SharePoint and Office failed" -Type ([MessageType]::Failure)
#    return
#}
#else {
#	Show-Message -Message "Completed adding and installing apps to SharePoint and Office" -Type ([MessageType]::Success)
#}


#----------------------------------------------
# Add Apps to Exchange
#----------------------------------------------
$ExchangeCredential = Get-Credential -Message "Enter credentials to connect with Exchange server."
Show-Message -Message "Step : Add apps to Exchange"
. "$ScriptDirectory\DeployOutlookApp.ps1" -IsDeploy: $true
    
If ((Get-Content $ErrorLogFile) -ne $Null) {
	Show-Message -Message "Adding apps to Exchange failed" -Type ([MessageType]::Failure)
    return
}
else {
	Show-Message -Message "Completed adding apps to Exchange" -Type ([MessageType]::Success)
}
