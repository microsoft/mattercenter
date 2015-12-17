#----------------------------------------------
# Steps performed in Delta script:
#
# Upgrade from 18.1.0.1 to 22.0.0.0 (Upgrade to preview) build
#
# Check PreRequisites 
# Checking for Matter Center App is present or not
# Step 1: Create site columns and content types
# Step 2: Update search configuration file and upload to SharePoint
# Step 3: Update resource and config files in build 
# Step 4: Activate SharePoint Server Publishing infrastructure feature on catalog site collection
# Step 5: Update App files for SharePoint and OneDrive Ribbon Apps
# Step 6: Publishing files to Azure
# Step 7: Add and install apps to SharePoint
# Step 8: Removing and installing Exchange App
# Step 9: Upload files to SharePoint Library
# Step 10: Provisioning Web dashboard, Setting Page and Document landing page 
# Step 11: Update Pinned List Permission for PII implementation
# Step 12: Delete pinned data from UserPinnedDetails list
# Any changes in these steps, kindly update this list.
#----------------------------------------------

# The below section would start new PowerShell in elevated mode
param
(    
    [parameter(Mandatory=$false)]                             
    [switch]$Elevated    
)


# Function to check current logged in user role
function Test-Admin {
  $currentUser = New-Object Security.Principal.WindowsPrincipal $([Security.Principal.WindowsIdentity]::GetCurrent())
  $currentUser.IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator)
}

if ((Test-Admin) -eq $false)  {
    if ($Elevated) 
    {
        # tried to elevate, did not work, aborting
        Write-Host 'Elevation of privileges did not work, Aborting!' -ForegroundColor Red
    } 
    else {
        Start-Process powershell.exe -Verb RunAs -ArgumentList ('-noprofile -noexit -file "{0}" -elevated' -f ($myinvocation.MyCommand.Definition))
    }
    exit
}


Write-Output 'Running with full privileges'
Write-Host 'Upgrading App from preview version' -ForegroundColor Green

$Date = Get-Date
Write-Host 'Starting scripts at:'  $Date


# Get the current directory of the script
function ScriptRoot {Split-Path $MyInvocation.ScriptName}
$ScriptDirectory = (ScriptRoot)

# Get the parent directory of the script
function Get-ParentDirectory {Split-Path -Parent(Split-Path $MyInvocation.ScriptName)}
$ParentDirectory = (Get-ParentDirectory)

# Global Variables to be used throughout the scripts
# Set Excel file path
$ExcelFilePath = "$ParentDirectory\MCDeploymentConfig.xlsx"
    
# Set log file path
$ErrorLogFile = "$ScriptDirectory\ErrorLog.txt"


# Set helper utilities folder path
$HelperPath = "$ScriptDirectory\Helper Utilities"

Set-Location $ScriptDirectory


#----------------------------------------------
# Clear all existing logs from error file or creates a new file
#----------------------------------------------

Write-Host 'Clearing all the previous logs...' -ForegroundColor Yellow
"" | Out-File $ErrorLogFile
Clear-Content $ErrorLogFile


Write-Host 'All previous logs cleared' -ForegroundColor Green

#----------------------------------------------
# Add SharePoint Client DLLs
#----------------------------------------------
Write-Host 'Adding SharePoint libraries...' -ForegroundColor Yellow
if ((Test-Path -Path "$HelperPath\Microsoft.SharePoint.Client.dll") -and (Test-Path -Path "$HelperPath\Microsoft.SharePoint.Client.Runtime.dll") -and (Test-Path -Path "$HelperPath\Microsoft.SharePoint.Client.Search.dll")) {
    Add-Type -Path (Resolve-Path "$HelperPath\Microsoft.SharePoint.Client.dll")
    Add-Type -Path (Resolve-Path "$HelperPath\Microsoft.SharePoint.Client.Runtime.dll")
	Add-Type -Path (Resolve-Path "$HelperPath\Microsoft.SharePoint.Client.Search.dll")
    Write-Host "Added SharePoint libraries" -ForegroundColor Green
}
else {
    Write-Log $ErrorLogFile 'Unable to load SharePoint libraries...'
    return $false
}

#----------------------------------------------
# Include Common functions script
#----------------------------------------------

Write-Host 'Adding common library functions' -ForegroundColor Yellow
. "$ScriptDirectory\LibraryFunctions.ps1"
Write-Host 'Added common library functions' -ForegroundColor Green

#----------------------------------------------
# Run Pre-requisite checker
#----------------------------------------------

Write-Output ""
."$ScriptDirectory\PreRequisitesScript.ps1"

$ExcelValues = (Read-FromExcel $ExcelFilePath "Config" ("TenantURL", "Username", "Password", "IsDeployedOnAzure","TenantAdminURL", "CatalogSiteURL") $ErrorLogFile)
$ExcelValues = $ExcelValues.Split(";")
if($ExcelValues.length -le 0)
{
    Write-Log $ErrorLogFile 'Error reading values from Excel file. Aborting!'
    return $false
}
$TenantUrl = $ExcelValues[0]
$Username = $ExcelValues[1]
$Password = $ExcelValues[2]
if ("false" -eq $ExcelValues[3].ToLowerInvariant()) {
    $IsDeployedOnAzure = $false
}
else {
    $IsDeployedOnAzure = $true
}
$TenantAdminURL=$ExcelValues[4]
$CatalogSiteUrl=$ExcelValues[5]
$AppName = "Matter Center Beta"
$Listname = "UserPinnedDetails"
$TotalSteps= 12;
[int]$StepComplete = 0;

if($IsValid -eq $true)
{

    Write-Output "Checking Matter Center app is installed"
    $Credential = New-PSCredential $UserName $Password
    Check-IsMatterCenterAppExist $Credential $TenantAdminURL $AppName $ErrorLogFile;

    Write-Output 'Starting deployment...'

	# Setting current location to "Helper Utilities" folder, in order to run the utility applications
    cd $HelperPath
    
    #----------------------------------------------
    # Create Site Columns and Content Types
    #----------------------------------------------
	Write-Output 'Step 1: Create site columns and update content types'
	& "$HelperPath\Microsoft.Legal.MatterCenter.CreateSiteColumns.exe" "true"
       
    Trace-ErrorLogFile 'Completed creating site columns and updating content types' 'Creating site columns and updating content types failed' $StepComplete;
    $StepComplete++;

	#----------------------------------------------
	# Update search configuration file and upload to SharePoint
	#----------------------------------------------
	Write-Output 'Step 2: Update Search Configuration files and upload to SharePoint'
	& "$HelperPath\Microsoft.Legal.MatterCenter.UpdateAppConfig.exe" "3"
    
    Trace-ErrorLogFile 'Completed updating search configuration file' 'Updating Search Configuration failed' $StepComplete;
	$StepComplete++;

    #----------------------------------------------
    # Update resource files for Utility, Service and UI projects
    #----------------------------------------------
    Write-Output 'Step 3: Update resource and config files in build'
     & "$HelperPath\Microsoft.Legal.MatterCenter.UpdateAppConfig.exe" "2"
     & "$HelperPath\Microsoft.Legal.MatterCenter.UpdateAppConfig.exe" "1"
    
     Trace-ErrorLogFile 'Completed updating resource and config files in solution' 'Updating resource and config files in solution failed' $StepComplete;
     $StepComplete++;

	#----------------------------------------------
	# Activate SharePoint Server Publishing Infrastructure
	#----------------------------------------------
    Write-Output 'Step 4: Activating SharePoint Server Publishing Infrastructure at Catalog site collection'
    if($IsDeployedOnAzure)
    {	
	try 
	{
		$SecurePassword = ConvertTo-SecureString $Password -AsPlainText -Force		
		$Context = New-Object Microsoft.SharePoint.Client.ClientContext($CatalogSiteUrl)  
		$Context.Credentials = New-Object Microsoft.SharePoint.Client.SharePointOnlineCredentials($Username, $SecurePassword)   
		Write-Host "Enabling the Feature with GUID $PublishingFeatureGuid" -ForegroundColor Green 
		$FeatureGuid = [System.Guid] $PublishingFeatureGuid
		$SpoSite = $Context.Site 
		$SpoSite.Features.Add($FeatureGuid, $True, [Microsoft.SharePoint.Client.FeatureDefinitionScope]::None) 
		$Context.ExecuteQuery();
	}
	catch [System.Exception]
	{
        Write-Log $ErrorLogFile $_.Exception.ToString();
	}
	finally {
        $Context.Dispose()
    }
    }
    Trace-ErrorLogFile 'Activated SharePoint Server Publishing Infrastructure Feature' 'Could not activate SharePoint Server Publishing Infrastructure feature' $StepComplete;
    $StepComplete++;

    #----------------------------------------------
	# Update App files for SharePoint and OneDrive Ribbon Apps
	#----------------------------------------------
	Write-Output 'Step 5: Update App files for SharePoint and OneDrive Ribbon Apps'
	. "$ScriptDirectory\UpdateAppPackage.ps1" -IsDeployedOnAzure $IsDeployedOnAzure -UserName $Username -Password $Password

    Trace-ErrorLogFile 'Completed updating App files for SharePoint and OneDrive Ribbon Apps' 'Updating App files for SharePoint and OneDrive Ribbon Apps failed' $StepComplete;
    $StepComplete++;


    #----------------------------------------------
    # Publish websites
    #----------------------------------------------
    
    Write-Output 'Step 6: Publishing files to Azure' 
    if($IsDeployedOnAzure)
	{           
      . "$ScriptDirectory\PublishOnAzure.ps1"  
      Trace-ErrorLogFile 'Completed publishing file to Azure' 'Publishing files to Azure failed' $StepComplete;
      $StepComplete++;
	}

    #----------------------------------------------
    # Upgrade SharePoint App 
    #----------------------------------------------
    Write-Output 'Step 7:Removing and installing apps from SharePoint'
    $credentials = New-PSCredential $UserName $Password;
    try {
        # Remove existing app
        . "$ScriptDirectory\AppInstall.ps1" -IsDeploy: $false
       . "$ScriptDirectory\DeployOfficeApp.ps1" -IsDeploy: $false -IsOfficeApp: $false
        # Install new version of app
       . "$ScriptDirectory\DeployOfficeApp.ps1" -IsDeploy: $true -IsOfficeApp: $false      
        . "$ScriptDirectory\AppInstall.ps1" -IsDeploy: $true

    }
    catch {
        Write-Log $ErrorLogFile "Could not remove apps from SharePoint"
    }
        
    Trace-ErrorLogFile 'Completed installing SharePoint apps' 'Failed to install SharePoint apps' $StepComplete;  
    $StepComplete++  

    
    #----------------------------------------------
    # Removing and Add Exchange App
    #----------------------------------------------
     Write-Output 'Step 8:Removing and installing Exchange App'
     . "$ScriptDirectory\DeployOutlookApp.ps1" -IsDeploy: $false
     . "$ScriptDirectory\DeployOutlookApp.ps1" -IsDeploy: $True
    Trace-ErrorLogFile 'Completed installing Exchange apps' 'Failed to install Exchange apps' $StepComplete;  
    $StepComplete++  


    #----------------------------------------------
    # Removing deny permission on tenant level
    #----------------------------------------------

    if($IsDeployedOnAzure)
	{
		$SecurePassword = ConvertTo-SecureString $Password -AsPlainText -force
		$O365Credential = New-Object System.Management.Automation.PsCredential($Username, $SecurePassword)
		Connect-SPOService -url $TenantAdminUrl -Credential $O365Credential
		$Rootsite = Get-SPOSite($TenantUrl)
		$ReturnedValue = [string[]]$Rootsite.DenyAddAndCustomizePages
		If("false" -ne $ReturnedValue.ToLower())
		{
			Set-SPOSite -Identity $TenantUrl -DenyAddAndCustomizePages $false
		}
	}

    #---------------------------------------------------------------------
    # Upload files required for Matter landing page to SharePoint library
    #---------------------------------------------------------------------
    Write-Output 'Step 9: Upload files to SharePoint Library'
    [Environment]::CurrentDirectory = Get-Location
    & "$HelperPath\Microsoft.Legal.MatterCenter.UploadFile.exe" "true"

    Trace-ErrorLogFile 'Completed uploading files to SharePoint library' 'Uploading files to SharePoint Library failed' $StepComplete;
    $StepComplete++;

    #---------------------------------------------------------------------
    # Provisioning Web Dashboard page(s) on SharePoint library
    #---------------------------------------------------------------------
    Write-Output 'Step 10: Provisioning Web dashboard'
    & "$HelperPath\Microsoft.Legal.MatterCenter.ProvisionWebDashboard.exe" "true"

     $StepComplete++;

    #---------------------------------------------------------------------
    # Update Permission on Pinned list
    #---------------------------------------------------------------------
    Write-Output 'Step 11: Updating permission on Pinned list'
    & "$HelperPath\Microsoft.Legal.MatterCenter.UpdateLists.exe"

    Trace-ErrorLogFile 'Completed updating permission on Pinned list' 'Updating permission on Pinned list failed' $StepComplete;
    $StepComplete++;

    #---------------------------------------------------------------------
    # Delte pinned data from 'UserPinnedDetails'list
    #---------------------------------------------------------------------
    Write-Output 'Step 12: Deleting pinned data from UserPinnedDetails list'
    ."$ScriptDirectory\DeleteListData.ps1" -Listname: $Listname -Username: $Username -Password: $Password -WebUrl:$CatalogSiteUrl

    Trace-ErrorLogFile 'Completed deleting user pinned documents successfully' 'Deleting user pinned document failed' $StepComplete
    $StepComplete++;
    #----------------------------------------------
    # Complete tool error check
    #----------------------------------------------
    Trace-ErrorLogFile 'Upgrade completed successfully' 'Upgrade failed' $StepComplete;

}
else
{
    Write-Output ""
    Write-Host 'Upgrade failed' -ForegroundColor Red
}

$Date = Get-Date
Write-Host 'Ended Deployment at:'  $Date
