#----------------------------------------------
# Steps performed in this script:
#
# Step 1a: Create groups and adding users to it
# Step 1b: Create configuration lists
# Step 2: Create taxonomy hierarchy
# Step 3: Create site columns and content types
# Step 4: Update Office, Outlook and SharePoint App schema files
# Step 5: Update search configuration file and upload to SharePoint
# Step 6: Update resource and config files in build
# Step 7: Activate SharePoint Server Publishing infrastructure feature on catalog site collection
# Step 8: Update App files for SharePoint and OneDrive Ribbon Apps
# Step 9: Encrypting the config files
# Step 10: Publishing files to Azure
# Step 11: Add and install apps to SharePoint and Office
# Step 12: Add apps to Exchange
# Step 13: Upload files to SharePoint Library
# Step 14: Creating Site Collection(s)
# Step 15: Provisioning Web dashboard
# Step 16: Update site collection view with field(s)
# Step 17: Creating source in event viewer
#
# Any changes in these steps, kindly update this list. Also update the checkpoint in Revert script
#----------------------------------------------

# The below section would start new PowerShell in elevated mode
param([switch]$Elevated)

function Test-Admin {
  $currentUser = New-Object Security.Principal.WindowsPrincipal $([Security.Principal.WindowsIdentity]::GetCurrent())
  $currentUser.IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator)
}

# Get the current directory of the script
Function ScriptRoot {Split-Path $MyInvocation.ScriptName}
$ScriptDirectory = (ScriptRoot)

# Get the parent directory of the script
Function Get-ParentDirectory {Split-Path -Parent(Split-Path $MyInvocation.ScriptName)}
$ParentDirectory = (Get-ParentDirectory)


function Load-ModuleByName {
<#
	.SYNOPSIS
	Load-ModuleByName $moduleName
	.SYNTAX
	Load-ModuleByName $moduleName
	.DESCRIPTION
	Load the specified module
	.EXAMPLE
	Do-Something
	.PARAMETER moduleName
	The Module to be loaded
 #>
	[CmdletBinding()]
	param(
		[Parameter(Mandatory=$True,ValueFromPipeline=$True, ValueFromPipelineByPropertyName=$True,HelpMessage='The module to be loaded')]		
		[ValidateLength(3,30)]
		[string[]]$moduleName
	)

	begin
	{
	}

	process
	{
		# Unloading the functions and reloading them.
		if (Get-Module -Name $moduleName)
		{    
		   Write-Debug "Previous version of $moduleName found"
		   Remove-Module -Name $moduleName
		}
		Import-Module -Name $moduleName
	}

	end
	{		
	}
}


function Start-Main {
<#
	.SYNOPSIS
	Start-Main starts the installation process
	.SYNTAX
	Start-Main
	.DESCRIPTION
	Starts the steps of the deployment
	.EXAMPLE
	Start-Main	
 #>
    begin
    {

        if ($env:PSModulePath -notlike "*$path\Modules\*")
		{
			"Adding ;$path\Modules to PSModulePath" | Write-Debug 
			$env:PSModulePath += ";$path\Modules\"
		}
    }

    process
    {

	Load-ModuleByName "HelperFunctions"
	
  

	# Global Variables to be used throughout the scripts
	#Set Excel file path
	$ExcelFilePath = "$ParentDirectory\MCDeploymentConfig.xlsx"

	#Create Log folder if not exist
	$LogFolder = "$ScriptDirectory\Logs"
	If (-not (Test-Path -Path $LogFolder -PathType Container))
	{ 
	   New-Item -Path $LogFolder -ItemType directory -Force 
	}

	# Set error log file path
	$ErrorLogFile = "$ScriptDirectory\Logs\ErrorLog.txt"

	# Set log file path
	$LogFile = "$ScriptDirectory\Logs\Log.txt"

	# Change log file name if already present
	If (Test-Path $LogFile) {
		$timestamp = Get-Date -format "dd_MMM_yyyy_HH_mm_ss"
		$LogFile = "$ScriptDirectory\Logs\Log_$timestamp.txt"
	}

	# Set revert log file path
	$RevertLogFile = "$ScriptDirectory\Logs\RevertLog.txt"

	# Set helper utilities folder path
	$HelperPath = "$ScriptDirectory\Helper Utilities"

	Set-Location $ScriptDirectory

	# Message Types 
Add-Type -TypeDefinition @"
   public enum MessageType
   {
      Success,
	  Warning,
	  Failure
   }
"@

	

	if ((Test-Admin) -eq $false)  {
		if ($elevated) 
		{
			# tried to elevate, did not work, aborting
			Show-Message -message "Elevation of privileges did not work, Aborting!" -Type ([MessageType]::Failure)

		} 
		else {
			Start-Process powershell.exe -Verb RunAs -ArgumentList ('-noprofile -noexit -file "{0}" -elevated' -f ($myinvocation.MyCommand.Definition))
		}
		exit
	}
	Show-Message -Message "Running with full privileges"

	$Date = Get-Date
	Show-Message -Message "Starting scripts at: $Date" -Type ([MessageType]::Success)



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
	# Accept required credentials for deployment
	#----------------------------------------------

	$SPCredential = Get-Credential -Message "Enter credentials to access SharePoint tenant."
	$ExchangeCredential = Get-Credential -Message "Enter credentials to connect with Exchange server."
	If ($Null -eq $SPCredential -or $Null -eq $ExchangeCredential) {
		# Display error message and exit
		Write-Log $ErrorLogFile "Failed to get credentials"
		return
	}

	#----------------------------------------------
	# Run Pre-requisite checker
	#----------------------------------------------

	Show-Message -Message ""
	. "$ScriptDirectory\PreRequisitesScript.ps1"

	$ExcelValues = (Read-FromExcel $ExcelFilePath "Config" ("TenantURL", "IsDeployedOnAzure", "EventViewer_LogName", "EventViewer_Source", "TenantAdminURL", "CatalogSiteURL") $ErrorLogFile)
	$ExcelValues = $ExcelValues.Split(";")
	if($ExcelValues.length -le 0)
	{
		Write-Log $ErrorLogFile "Error reading values from Excel file. Aborting!"
		return $false
	}
	$TenantUrl = $ExcelValues[0]
	$IsDeployedOnAzure = [System.Convert]::ToBoolean($ExcelValues[1])
	$Log = $ExcelValues[2]
	$Source = $ExcelValues[3]
	$TenantAdminURL=$ExcelValues[4]
	$CatalogSiteUrl=$ExcelValues[5]
	$Username = $SPCredential.UserName
	$Password = $SPCredential.GetNetworkCredential().Password

	if($IsValid -eq $true)
	{
		Show-Message -Message "Starting deployment..."

		#----------------------------------------------
		# Configure trust for the apps and update Issuer Id in Web.config
		#----------------------------------------------
		. "$ScriptDirectory\ConfigureTrust.ps1"

		If ((Get-Content $ErrorLogFile) -ne $Null) {
			Show-Message -Message "Failed to configure trust of the app" -Type ([MessageType]::Failure)
			return
		}
		else {
			Show-Message -Message "Successfully configured trust of the app" -Type ([MessageType]::Success)
		}

		#----------------------------------------------
		# Create crawled and managed properties and add mapping
		#----------------------------------------------
		Show-Message -Message ""
		Show-Message -Message "Create Managed Properties"
		. "$ScriptDirectory\CreateProperties.ps1"
		If ((Get-Content $ErrorLogFile) -ne $Null) {
			Show-Message -Message "Failed to create Managed Properties" -Type ([MessageType]::Failure)
			return
		}
		else {
			Show-Message -Message "Successfully created Managed Properties" -Type ([MessageType]::Success)
		}

		# Setting current location to "Helper Utilities" folder, in order to run the utility applications
		cd $HelperPath
		#----------------------------------------------
		# Create configuration lists in central repository in SharePoint
		#----------------------------------------------
		Show-Message -Message "Step 1a: Create groups and adding users to it" 
		& "$HelperPath\Microsoft.Legal.MatterCenter.CreateGroups.exe" "true" $Username $Password

		If ((Get-Content $ErrorLogFile) -ne $Null) {
			Show-Message -Message "Creating group and adding users to it failed" -Type ([MessageType]::Failure)
			RevertAll $ScriptDirectory 1
			return
		}
		else {
			Show-Message -Message "Completed creating groups and adding users to it" -Type ([MessageType]::Success)
		}	
		#----------------------------------------------
		# Create configuration lists in central repository in SharePoint
		#----------------------------------------------
    
		Show-Message -Message "Step 1b: Create configuration lists" 
		[Environment]::CurrentDirectory = Get-Location
		& "$HelperPath\Microsoft.Legal.MatterCenter.ConfigureLists.exe" "false" $Username $Password

		If ((Get-Content $ErrorLogFile) -ne $Null) {
			Show-Message -Message "Creating configuration lists failed" -Type ([MessageType]::Failure)
			RevertAll $ScriptDirectory 1
			return
		}
		else {
			Show-Message -Message "Completed creating configuration lists" -Type ([MessageType]::Success)
		}
    

		#----------------------------------------------
		# Configure Taxonomy hierarchy in SharePoint
		#----------------------------------------------
		Show-Message -Message "Step 2: Create taxonomy hierarchy" 
		& "$HelperPath\Microsoft.Legal.MatterCenter.CreateTerm.exe" "false" $Username $Password
		& "$HelperPath\Microsoft.Legal.MatterCenter.CreateTerm.exe" "true" $Username $Password
    
		If ((Get-Content $ErrorLogFile) -ne $Null) {
			Show-Message -Message "Creating taxonomy hierarchy failed" -Type ([MessageType]::Failure)
			RevertAll $ScriptDirectory 2
			return
		}
		else {
			Show-Message -Message "Completed creating taxonomy hierarchy" -Type ([MessageType]::Success)
		}
    

		#----------------------------------------------
		# Create Site Columns and Content Types
		#----------------------------------------------

		Show-Message -Message "Step 3: Create site columns and content types"
		& "$HelperPath\Microsoft.Legal.MatterCenter.CreateSiteColumns.exe" "false" $Username $Password
 
		If ((Get-Content $ErrorLogFile) -ne $Null) {
			Show-Message -Message "Creating site columns and content types failed" -Type ([MessageType]::Failure)
			RevertAll $ScriptDirectory 3
			return
		}
		else {
			Show-Message -Message "Completed creating site columns and content types" -Type ([MessageType]::Success)
		}
	 
		& "$HelperPath\Microsoft.Legal.MatterCenter.CreateContentTypes.exe" "true" $Username $Password
		If ((Get-Content $ErrorLogFile) -ne $Null) {
			Show-Message -Message "Creating content types failed" -Type ([MessageType]::Failure)
			return
		}
		else {
			Show-Message -Message "Completed creating content types" -Type ([MessageType]::Success)
		}

		 #----------------------------------------------
		 # Update Office, Outlook and SharePoint App schema files
		 #----------------------------------------------
		 Show-Message -Message "Step 4: Update Office, Outlook and SharePoint App schema files"
		 & "$HelperPath\Microsoft.Legal.MatterCenter.UpdateAppConfig.exe" "1" $Username $Password

		 If ((Get-Content $ErrorLogFile) -ne $Null) {
			 Show-Message -Message "Updating Office, Outlook and SharePoint App schema files failed" -Type ([MessageType]::Failure)
			 RevertAll $ScriptDirectory 3		#Revert from step 3 to 1
			 return
		 }
		 else {
			 Show-Message -Message "Completed updating Office, Outlook and SharePoint App schema files" -Type ([MessageType]::Success)
		 }

		#----------------------------------------------
		# Update search configuration file and upload to SharePoint
		#----------------------------------------------
		Show-Message -Message "Step 5: Update Search Configuration files and upload to SharePoint"
		& "$HelperPath\Microsoft.Legal.MatterCenter.UpdateAppConfig.exe" "3" $Username $Password
	    
		If ((Get-Content $ErrorLogFile) -ne $Null) {
			Show-Message -Message "Updating Search Configuration failed" -Type ([MessageType]::Failure)
			RevertAll $ScriptDirectory 3		#Revert from step 3 to 1
			return
		}
		else {
			Show-Message -Message "Completed updating search configuration file" -Type ([MessageType]::Success)
		}


		#----------------------------------------------
		# Update resource files for Utility, Service and UI projects
		#----------------------------------------------
		Show-Message -Message "Step 6: Update resource and config files in build"
		 & "$HelperPath\Microsoft.Legal.MatterCenter.UpdateAppConfig.exe" "2" $Username $Password $ExchangeCredential.UserName $ExchangeCredential.GetNetworkCredential().Password
    
		 If ((Get-Content $ErrorLogFile) -ne $Null) {
			 Show-Message -Message "Updating resource and config files in solution failed" -Type ([MessageType]::Failure)
			 RevertAll $ScriptDirectory 3		#Revert from step 3 to 1
			 return
		 }
		 else {
			 Show-Message -Message "Completed updating resource and config files in solution" -Type ([MessageType]::Success)
		 }

		#----------------------------------------------
		# Activate SharePoint Server Publishing Infrastructure
		#----------------------------------------------
		Show-Message -Message "Step 7: Activating SharePoint Server Publishing Infrastructure at Catalog site collection"
		try 
		{
			$Context = New-Object Microsoft.SharePoint.Client.ClientContext($CatalogSiteUrl)  
			$Context.Credentials = New-Object Microsoft.SharePoint.Client.SharePointOnlineCredentials($Username, $SPCredential.Password)   
			Show-Message -Message "Enabling the Feature with GUID" $PublishingFeatureGuid
			$FeatureGuid = [System.Guid] $PublishingFeatureGuid
			$SpoSite = $Context.Site 
			$SpoSite.Features.Add($FeatureGuid, $True, [Microsoft.SharePoint.Client.FeatureDefinitionScope]::None) 
			$Context.ExecuteQuery()
			Show-Message -Message "Activated SharePoint Server Publishing Infrastructure Feature" -Type ([MessageType]::Success)
		}
		catch [System.Exception]
		{
			Show-Message -Message "Could not activate SharePoint Server Publishing Infrastructure feature." -Type ([MessageType]::Failure)
			Write-Log $ErrorLogFile $_.Exception.ToString()
		}
		finally {
			$Context.Dispose()
		}

		#----------------------------------------------
		# Update App files for SharePoint and OneDrive Ribbon Apps
		#----------------------------------------------
		Show-Message -Message "Step 8: Update App files for SharePoint and OneDrive Ribbon Apps"
		. "$ScriptDirectory\UpdateAppPackage.ps1" -IsDeployedOnAzure $IsDeployedOnAzure -Credentials $SPCredential
	    
		If ((Get-Content $ErrorLogFile) -ne $Null) {
			Show-Message -Message "Updating App files for SharePoint and OneDrive Ribbon Apps failed" -Type ([MessageType]::Failure)
			RevertAll $ScriptDirectory 3		#Revert from step 3 to 1
			return
		}
		else {
			Show-Message -Message "Completed updating App files for SharePoint and OneDrive Ribbon Apps" -Type ([MessageType]::Success)
		}

		#----------------------------------------------
		# Encrypt the appSettings section in web.config
		#----------------------------------------------
		Show-Message -Message "Step 9: Encrypting the config files"
		. "$ScriptDirectory\EncryptDecrypt.ps1" -ToEncrypt: $true -ErrorLogPath: $ErrorLogFile
		If ((Get-Content $ErrorLogFile) -ne $Null) {
			Show-Message -Message "Encryption failed..." -Type ([MessageType]::Failure)
			RevertAll $ScriptDirectory 9
			return
		}
		else {
			Show-Message -Message "Config files encrypted successfully..." -Type ([MessageType]::Success)
		}

		#----------------------------------------------
		# Publish websites
		#----------------------------------------------
		Show-Message -Message "Step 10: Publishing files to Azure/IIS"
	   $IsOnAzure = (Read-FromExcel $ExcelFilePath "Config" ("IsDeployedOnAzure") $ErrorLogFile)
		if($IsOnAzure[0])
		{       
		  . "$ScriptDirectory\PublishOnAzure.ps1"   
		  If ((Get-Content $ErrorLogFile) -ne $Null) {
			  Show-Message -Message "Publishing files to Azure or creating Azure Redis cache failed" -Type ([MessageType]::Failure)
			  RevertAll $ScriptDirectory 9
		  }
		}
		else {	    
		   . "$ScriptDirectory\PublishInIIS.ps1"    
		   If ((Get-Content $ErrorLogFile) -ne $Null) {
			   Show-Message -Message "Publishing files to IIS failed" -Type ([MessageType]::Failure)           
		  }      
		}

		#----------------------------------------------
		# Add Apps to SharePoint and Office
		#----------------------------------------------
		Show-Message -Message "Step 11: Add and install apps to SharePoint and Office"
		. "$ScriptDirectory\AppInstall.ps1" -IsDeploy: $false
		. "$ScriptDirectory\DeployOfficeApp.ps1" -IsDeploy: $true -IsOfficeApp: $false
		. "$ScriptDirectory\DeployOfficeApp.ps1" -IsDeploy: $true -IsOfficeApp: $true    
		. "$ScriptDirectory\AppInstall.ps1" -IsDeploy: $true
    
		If ((Get-Content $ErrorLogFile) -ne $Null) {
			Show-Message -Message "Adding and installing apps to SharePoint and Office failed" -Type ([MessageType]::Failure)
			RevertAll $ScriptDirectory 11
			return
		}
		else {
			Show-Message -Message "Completed adding and installing apps to SharePoint and Office" -Type ([MessageType]::Success)
		}


		#----------------------------------------------
		# Add Apps to Exchange
		#----------------------------------------------
		Show-Message -Message "Step 12: Add apps to Exchange"
		. "$ScriptDirectory\DeployOutlookApp.ps1" -IsDeploy: $true
    
		If ((Get-Content $ErrorLogFile) -ne $Null) {
			Show-Message -Message "Adding apps to Exchange failed" -Type ([MessageType]::Failure)
			RevertAll $ScriptDirectory 12
			return
		}
		else {
			Show-Message -Message "Completed adding apps to Exchange" -Type ([MessageType]::Success)
		}

		#---------------------------------------------------------------------
		# Upload files required for Matter landing page to SharePoint library
		#---------------------------------------------------------------------
		Show-Message -Message "Step 13: Upload files to SharePoint Library"
		[Environment]::CurrentDirectory = Get-Location
		& "$HelperPath\Microsoft.Legal.MatterCenter.UploadFile.exe" "true" $Username $Password

		If ((Get-Content $ErrorLogFile) -ne $Null) {
			Show-Message -Message "Uploading files to SharePoint Library failed" -Type ([MessageType]::Failure)
			RevertAll $ScriptDirectory 13
			return
		}
		else {
			Show-Message -Message "Completed uploading files to SharePoint library" -Type ([MessageType]::Success)
		}

		#---------------------------------------------------------------------
		# Create site collection(s) on SharePoint library
		#---------------------------------------------------------------------
		if($IsDeployedOnAzure)
		{
			Connect-SPOService -url $TenantAdminUrl -Credential $SPCredential
			$Rootsite = Get-SPOSite($TenantUrl)
			$ReturnedValue = [string[]]$Rootsite.DenyAddAndCustomizePages
			If("false" -ne $ReturnedValue.ToLower())
			{
				Set-SPOSite -Identity $TenantUrl -DenyAddAndCustomizePages $false
			}
		}
	
		Show-Message -Message "Step 14: Creating Site Collection(s)"
		. "$ScriptDirectory\CreateSiteCollection.ps1" -IsDeployedOnAzure: $IsDeployedOnAzure -Username: $Username -Password $Password
		If ((Get-Content $ErrorLogFile) -ne $Null) {
			Show-Message -Message "Creating site collection failed" -Type ([MessageType]::Failure)
		}
		else {
			Show-Message -Message "Completed creating site collection" -Type ([MessageType]::Success)
		}

		#---------------------------------------------------------------------
		# Provisioning Web Dashboard page(s) on SharePoint library
		#---------------------------------------------------------------------
		Show-Message -Message "Step 15: Provisioning Web dashboard"
		& "$HelperPath\Microsoft.Legal.MatterCenter.ProvisionWebDashboard.exe" "true" $Username $Password

		If ((Get-Content $ErrorLogFile) -ne $Null) {
			Show-Message -Message "Provisioning Web dashboard failed" -Type ([MessageType]::Failure)  
		}
		else {
			Show-Message -Message "Completed Provisioning Web dashboard" -Type ([MessageType]::Success)
		}

		#---------------------------------------------------------------------
		# Update site pages view with fields
		#---------------------------------------------------------------------
		Show-Message -Message "Step 16: Update site collection view with fields"
		& "$HelperPath\Microsoft.Legal.MatterCenter.UpdateView.exe" $Username $Password

		#---------------------------------------------------------------------
		# Creating source in event viewer
		#---------------------------------------------------------------------
		Show-Message -Message "Step 17: Creating source in event viewer"
		if(-not $IsDeployedOnAzure)
		{
			$logFileExists = Get-EventLog -list | Where-Object {$_.logdisplayname -eq $Log} 
			if (! $logFileExists) {
				New-EventLog -LogName $Log -Source $Source
			}
		}

		#----------------------------------------------
		# Complete tool error check
		#----------------------------------------------
		If ((Get-Content $ErrorLogFile) -eq $Null) {
			Show-Message -Message "Deployment completed successfully" -Type ([MessageType]::Success)
		}
		else {
			Show-Message -Message "Deployment failed" -Type ([MessageType]::Failure)
		}
	}
	else
	{
		Show-Message -Message ""
		Show-Message -Message "Deployment failed" -Type ([MessageType]::Failure)
	}

    end
    {

	    $Date = Get-Date
	    Show-Message -Message "Ended Deployment at: $Date"
    }

}
}




	



$path = Split-Path -parent $MyInvocation.MyCommand.Definition
Start-Main
