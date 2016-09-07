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
	# Get the current directory of the script
	Function ScriptRoot {Split-Path $MyInvocation.ScriptName}
	$ScriptDirectory = (ScriptRoot)

	# Get the parent directory of the script
	Function Get-ParentDirectory {Split-Path -Parent(Split-Path $MyInvocation.ScriptName)}
	$ParentDirectory = (Get-ParentDirectory)

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



	# Message Types 
	Add-Type -TypeDefinition @"
	   public enum MessageType
	   {
		  Success,
		  Warning,
		  Failure
	   }
"@
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

	# Set helper utilities folder path
	$RootPath = Split-Path(Split-Path (Split-Path $PSScriptRoot -Parent) -Parent) -Parent
	$HelperPath = "$RootPath\deployments\scripts\Helper Utilities"
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
	cd $PSScriptRoot
    
}

$UIUrl = [string]::format("https://{0}.azurewebsites.net", $WebAppName)
$SPCredential = Get-Credential -Message "Enter credentials to access SharePoint tenant."
$Password = $SPCredential.GetNetworkCredential().Password
Deploy-SPOFiles -WebSiteName $UIUrl  -UserName $SPCredential.UserName -PassWord $Password 