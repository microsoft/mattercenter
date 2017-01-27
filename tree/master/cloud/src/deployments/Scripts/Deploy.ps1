#----------------------------------------------
# Steps performed in this script:
#
# Step 1a: Create groups and adding users to it
# Step 1b: Create configuration lists
# Step 2: Create taxonomy hierarchy
# Step 3: Create site columns and content types
# Step 4: Update search configuration file and upload to SharePoint
# Step 5: Activate SharePoint Server Publishing infrastructure feature on catalog site collection 
# Step 6: Creating Site Collection(s)
# Step 7: Update site collection view with field(s)
# Step 8: Create ProvisionMatterList
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

If ($Null -eq $SPCredential) {
	# Display error message and exit
	Write-Log $ErrorLogFile "Failed to get credentials"
	return
}

#----------------------------------------------
# Run Pre-requisite checker
#----------------------------------------------

Show-Message -Message ""
. "$ScriptDirectory\PreRequisitesScript.ps1"

$ExcelValues = (Read-FromExcel $ExcelFilePath "Config" ("TenantURL", "TenantAdminURL", "CatalogSiteURL") $ErrorLogFile)
$ExcelValues = $ExcelValues.Split(";")
if($ExcelValues.length -le 0)
{
    Write-Log $ErrorLogFile "Error reading values from Excel file. Aborting!"
    return $false
}
$TenantUrl = $ExcelValues[0]
$TenantAdminURL=$ExcelValues[1]
$CatalogSiteUrl=$ExcelValues[2]
$Username = $SPCredential.UserName
$Password = $SPCredential.GetNetworkCredential().Password

if($IsValid -eq $true)
{
    Show-Message -Message "Starting deployment..."

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
	# Update search configuration file and upload to SharePoint
	#----------------------------------------------
	Show-Message -Message "Step 4: Update Search Configuration files and upload to SharePoint"
	& "$HelperPath\Microsoft.Legal.MatterCenter.UpdateAppConfig.exe" "3" $Username $Password "false"
	    
	If ((Get-Content $ErrorLogFile) -ne $Null) {
		Show-Message -Message "Updating Search Configuration failed" -Type ([MessageType]::Failure)
	    RevertAll $ScriptDirectory 4		#Revert from step 3 to 1
	    return
	}
	else {
		Show-Message -Message "Completed updating search configuration file" -Type ([MessageType]::Success)
	}

	#----------------------------------------------
	# Activate SharePoint Server Publishing Infrastructure
	#----------------------------------------------
	Show-Message -Message "Step 5: Activating SharePoint Server Publishing Infrastructure at Catalog site collection"
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
		Show-Message -Message "Could not activate SharePoint Server Publishing Infrastructure feature." -Type ([MessageType]::Warning)
        #Write-Log $ErrorLogFile $_.Exception.ToString()
	}
	finally {
        $Context.Dispose()
    }
   
    #---------------------------------------------------------------------
    # Create site collection(s) on SharePoint library
    #---------------------------------------------------------------------
	Connect-SPOService -url $TenantAdminUrl -Credential $SPCredential
	$Rootsite = Get-SPOSite($TenantUrl)
	$ReturnedValue = [string[]]$Rootsite.DenyAddAndCustomizePages
	If("false" -ne $ReturnedValue.ToLower())
	{
		Set-SPOSite -Identity $TenantUrl -DenyAddAndCustomizePages $false
	}
	
    Show-Message -Message "Step 6: Creating Site Collection(s)"
    . "$ScriptDirectory\CreateSiteCollection.ps1" -Username: $Username -Password $Password
    If ((Get-Content $ErrorLogFile) -ne $Null) {
		Show-Message -Message "Creating site collection failed" -Type ([MessageType]::Failure)
    }
    else {
		Show-Message -Message "Completed creating site collection" -Type ([MessageType]::Success)
    }

	#---------------------------------------------------------------------
    # Update site pages view with fields
    #---------------------------------------------------------------------
    Show-Message -Message "Step 7: Update site collection view with fields"
    & "$HelperPath\Microsoft.Legal.MatterCenter.UpdateView.exe" $Username $Password

    #----------------------------------------------
    # Complete tool error check
    #----------------------------------------------
    If ((Get-Content $ErrorLogFile) -eq $Null) {
		Show-Message -Message "Update site collection view with fields successfully" -Type ([MessageType]::Success)
    }
    else {
		Show-Message -Message "Update site collection view with fields failed" -Type ([MessageType]::Failure)
    }
	
	
	#---------------------------------------------------------------------
    # Create Provision Matter List
    #---------------------------------------------------------------------
    Show-Message -Message "Step 8: Create Provision Matter List"
    & "$HelperPath\Microsoft.Legal.MatterCenter.UpdateListPermissions" $Username $Password

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

$Date = Get-Date
Show-Message -Message "Ended Deployment at: $Date"