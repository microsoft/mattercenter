param
(                  
    [parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [bool] $IsDeploy
)

# Online functions
function UninstallSharePointApps()
{
param            
    (                  
        [parameter(Mandatory=$true)]                         
        [ValidateNotNullOrEmpty()]
        [String] $webUrl,

        [parameter(Mandatory=$true)]                         
        [ValidateNotNullOrEmpty()]
        [String] $adminUrl,

        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $FolderPath
    )

    try
    {
        $context = New-Object Microsoft.SharePoint.Client.ClientContext($webUrl)
		if($SPCredential -eq $null)
        {
            $SPCredential = Get-Credential -Message "Enter SharePoint Credentials"
        }
        $onlinecredential = New-Object Microsoft.SharePoint.Client.SharePointOnlineCredentials($SPCredential.UserName, $SPCredential.Password)
        $context.Credentials = $onlinecredential
        Connect-SPOService -Url $adminurl -Credential $SPCredential
		Show-Message -Message "Proceeding to uninstall the apps" -Type ([MessageType]::Warning)
    
        # Retrieve the specific site before looping through each App
        $web = $context.Web
        $context.Load($web)
        $context.ExecuteQuery()

        $OfficeSolutionList = @("Microsoft.Legal.MatterCenter")

        #Flag to specify the friendly name for copied files
        $FriendlyName = @("Matter Center Beta")
            
        Foreach ($File in (dir $FolderPath))
        {
            # Retrieve the specific file name
            $fileNameWithExtension = split-path $File -Leaf
            $fileName = [System.IO.Path]::GetFileNameWithoutExtension($fileNameWithExtension)

            [int] $position = $OfficeSolutionList.IndexOf($fileName)
            if (-1 -ne $position) {

                $fileName = $FriendlyName[$position]

                $AppInfo = Get-SPOAppInfo -Name $fileName
                foreach($app in $AppInfo)
                {
                    $AppInstance = $web.GetAppInstancesByProductId($app.ProductId)
                    $context.Load($AppInstance)
                    $context.ExecuteQuery()
                    if($AppInstance.Count)
                    {
                        Write-Warning "Retrieving App instances to uninstall"
                        if($AppInstance.Item(0) -ne $null)
                        {
                            $AppInstance.Item(0).Uninstall()
							$context.Load($AppInstance)
                            $context.ExecuteQuery()
                            
							#Check app is uninstalled successfully                                                        
							Show-Message -Message ([String]::Concat("Status: ", $AppInstance.Status)) -Type ([MessageType]::Success)                         
                            while($AppInstance.Status -eq "Uninstalling") {
								Show-Message -Message "." -Type ([MessageType]::Success) -Newline $false
                                Start-Sleep -Seconds 10
                                $context.Load($AppInstance)
                                $context.ExecuteQuery()                                
                            }                            
                            if([String]::IsNullOrEmpty($AppInstance)) {
								Show-Message -Message ([String]::Concat($fileNameWithExtension, " has been uninstalled successfully")) -Type ([MessageType]::Success)
                             }
                             else {                                
                                Write-Log $ErrorLogFile "Failed to uninstall app from SharePoint, kindly uninstall manually (refer to document which contains manual steps)"
                             }
                        }
                    }
                }
            }
        }
    }
    catch [exception]
    {
        Write-Error "Failed to uninstall the app"
        Write-Log $ErrorLogFile $_.Exception.ToString()
    }
}

function loadAndInstallSharePointApps()
{
    param            
    (                  
        [parameter(Mandatory=$true)]                         
        [ValidateNotNullOrEmpty()]
        [String] $webUrl,

        [parameter(Mandatory=$true)]                         
        [ValidateNotNullOrEmpty()]
        [String] $adminUrl,

        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $FolderPath,

		[parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $AppName
    )

    #get the stream from app file
    try
    {
        $context = New-Object Microsoft.SharePoint.Client.ClientContext($webUrl)
        if($SPCredential -eq $null)
        {
            $SPCredential = Get-Credential
        }
        $onlinecredential = New-Object Microsoft.SharePoint.Client.SharePointOnlineCredentials($SPCredential.UserName, $SPCredential.Password)
        $context.Credentials = $onlinecredential
        $sideLoadingEnabled = [Microsoft.SharePoint.Client.appcatalog]::IsAppSideloadingEnabled($context);
        $site = $context.Site;
        $context.ExecuteQuery()
    
        Write-Warning ("Checking for SideLoading feature on site " + $webUrl)
        
        if($sideLoadingEnabled.value -eq $false) 
        {
			Show-Message -Message "SideLoading feature is disabled on site $webUrl" -Type ([MessageType]::Warning)
            $sideLoadingGuid = new-object System.Guid "AE3A1339-61F5-4f8f-81A7-ABD2DA956A7D"
            $site.Features.Add($sideLoadingGuid, $false, [Microsoft.SharePoint.Client.FeatureDefinitionScope]::None);
            $context.ExecuteQuery();
			Show-Message -Message "SideLoading feature enabled on site $webUrl" -Type ([MessageType]::Success)
        }
        else {
			Show-Message -Message "SideLoading feature enabled on site $webUrl" -Type ([MessageType]::Success)
        }
    
		Show-Message -Message "Proceeding to load and install the apps" -Type ([MessageType]::Warning)
        # Load the site context
        $web = $context.Web
        $context.Load($web)
    
        Foreach ($File in (dir $FolderPath))
        {
            if(-1 -ne $appPath.IndexOf($File.FullName))
            {
                $appIoStream = New-Object IO.FileStream($File.FullName ,[System.IO.FileMode]::Open)
                $web.LoadAndInstallApp($appIoStream)
                $context.ExecuteQuery()
                $fileName = split-path $File -Leaf
                $AppInfo = Get-SPOAppInfo -Name $AppName
				#Ensure app is installed on catalog
                Foreach($app in $AppInfo)
                 {
                    $AppInstance = $web.GetAppInstancesByProductId($app.ProductId)
                    $context.Load($AppInstance)
                    $context.ExecuteQuery()
					 Show-Message -Message ([String]::Concat("Status:", $AppInstance.Status)) -Type ([MessageType]::Success) -Newline $false
                    while($AppInstance.Status -eq "Installing")
                     {                        
					   Show-Message -Message "." -Type ([MessageType]::Success) -Newline $false
                       Start-Sleep -s 10
                       $context.Load($AppInstance)
                       $context.ExecuteQuery()
                     }
                     
                     if($AppInstance.Status -eq "Installed") {
						 Show-Message -Message ([String]::Concat($AppInstance.Title, " has been successfully loaded and installed")) -Type ([MessageType]::Success)
                     }
                     else {                        
						Write-Log $ErrorLogFile "Failed to install apps to SharePoint."
                     }
                }
            }
        }

        #Disable Sideloading feature
		Show-Message -Message "Disabling SideLoading feature on site $siteurl" -Type ([MessageType]::Warning)
    
        $sideLoadingEnabled = [Microsoft.SharePoint.Client.appcatalog]::IsAppSideloadingEnabled($context);
        $site = $context.Site;
        $context.ExecuteQuery()
    
        if($sideLoadingEnabled.value -eq $false)
        {
			Show-Message -Message "SideLoading is already disabled on site $webUrl" -Type ([MessageType]::Success)
        }
        else
        {
            $sideLoadingGuid = new-object System.Guid "AE3A1339-61F5-4f8f-81A7-ABD2DA956A7D"
            $site.Features.Remove($sideLoadingGuid, $true);
            $context.ExecuteQuery();
			Show-Message -Message "SideLoading disabled on site $webUrl" -Type ([MessageType]::Success)
        }
    }
    catch [Exception]
    {
		Show-Message -Message "Failed to install the app" -Type ([MessageType]::Failure)
        Write-Log $ErrorLogFile $_.Exception.ToString()
    }
}

# On Premise functions
function InstallAppOnPremise() {

param(
    [ValidateNotNullOrEmpty()]
    [string]$AppName,
    [ValidateNotNullOrEmpty()]
    [string]$appPath
)

 #Install App

 try {
         $appInstance = Get-SPAppInstance -Web $webUrl | where-object {$_.Title -eq $AppName};
    
        if ($appInstance -eq $null) {

			Show-Message -Message "Installing the App in web site..." -Type ([MessageType]::Warning)

            $spapp = Import-SPAppPackage -Path $appPath -Site $webUrl -Source $sourceApp -Confirm:$false -ErrorAction SilentlyContinue -ErrorVariable err;
        
            $app = Install-SPApp -Web $webUrl -Identity $spapp -Confirm:$false -ErrorAction SilentlyContinue -ErrorVariable err;
        
            if ($err -or ($app -eq $null)) {
				Show-Message -Message "An error occurred while installing app..." -Type ([MessageType]::Warning)
                #throw $err;
            }
        
            # $AppName = $app.Title;
        
			Show-Message -Message "App $AppName registered, please wait during installation..." -Type ([MessageType]::Success)
        
            $appInstance = Get-SPAppInstance -Web $webUrl | where-object {$_.Title -eq $AppName};
            $counter = 1;
            $maximum = 150;
            $sleeptime = 2;

			Show-Message -Message "Please wait..." -Type ([MessageType]::Warning) -Newline $false
        
            while (($appInstance.Status -eq ([Microsoft.SharePoint.Administration.SPAppInstanceStatus]::Installing)) -and ($counter -lt $maximum)) {
				Show-Message -Message "." -Type ([MessageType]::Warning) -Newline $false
                sleep $sleeptime;
                $counter++;
                $appInstance = Get-SPAppInstance -Web $webUrl | where-object {$_.Title -eq $AppName} 
            }
        
			Show-Message -Message "." -Type ([MessageType]::Warning)
        
            if ($appInstance.Status -eq [Microsoft.SharePoint.Administration.SPAppInstanceStatus]::Installed) {
				Show-Message -Message "The App was successfully installed." -Type ([MessageType]::Success)
                $appUrl = $appInstance.AppWebFullUrl;
            }
            else {
				Show-Message -Message "An unknown error has occurred during app installation. Read SharePoint log for more information..." -Type ([MessageType]::Failure)
            }
        }
        else {
            UninstallAppOnPremise $AppName $appPath
            InstallAppOnPremise $AppName $appPath
        }
    }
    catch [exception]
    {
		Show-Message -Message "Failed to install the app" -Type ([MessageType]::Failure)
        Write-Log $ErrorLogFile $_.Exception.ToString()
    } 
}

function UninstallAppOnPremise() {

param(
    [ValidateNotNullOrEmpty()]
    [string]$AppName
)

#Uninstall
    try
    {
        $appInstance = Get-SPAppInstance -Web $webUrl | where-object {$_.Title -eq $AppName};
    
        if ($appInstance -ne $null) {
			Show-Message -Message "App instance detected on target web, uninstalling it..." -Type ([MessageType]::Warning)
    
            Uninstall-SPAppInstance –Identity $appInstance -Confirm:$false -ErrorAction SilentlyContinue -ErrorVariable err;
    
            if ($err) {
				Show-Message -Message "An error occurred while uninstalling app..." -Type ([MessageType]::Failure)
                #throw $err;
            }
    
			Show-Message -Message "Please wait..." -Type ([MessageType]::Warning) -Newline $false
    
            $appInstance = Get-SPAppInstance -Web $webUrl | where-object {$_.Title -eq $AppName};
            $counter = 1;
            $maximum = 50;
            $sleeptime = 2;
    
            while (($appInstance -ne $null) -and ($appInstance.Status -eq ([Microsoft.SharePoint.Administration.SPAppInstanceStatus]::Uninstalling)) -and ($counter -lt $maximum)){
				Show-Message -Message "." -Type ([MessageType]::Warning) -Newline $false
                sleep $sleeptime;
                $counter++;
                $appInstance = Get-SPAppInstance -Web $webUrl | where-object {$_.Title -eq $AppName} 
            }
    
			Show-Message -Message "." -Type ([MessageType]::Warning)
        
            $appInstance = Get-SPAppInstance -Web $webUrl | where-object {$_.Title -eq $AppName} 
    
            if ($appInstance -eq $null) {
				Show-Message -Message "App '$AppName' was successfully uninstalled..." -Type ([MessageType]::Success)
				Show-Message -Message "  " -Type ([MessageType]::Success)
            }
            else {
				Show-Message -Message "App '$AppName' was not successfully uninstalled..." -Type ([MessageType]::Failure)
            }
        }
        else
        {
            Write-Warning ("App " + $AppName + " not found in web site " + $webUrl + ".")
        }
    }
    catch [exception]
    {
		Show-Message -Message "Failed to uninstall the app" -Type ([MessageType]::Failure)
        Write-Log $ErrorLogFile $_.Exception.ToString()
    } 
}

# Get the current directory of the script
Function ScriptRoot {Split-Path $MyInvocation.ScriptName}
$ScriptDirectory = (ScriptRoot)

# Get the parent directory of the script
Function Get-ParentDirectory {Split-Path -Parent(Split-Path $MyInvocation.ScriptName)}
$ParentDirectory = (Get-ParentDirectory)

# SharePoint App Folder name
$SharePointAppFolder = "SharePoint App"

# Search Configuration XML path
$SearchConfigurationPath = Resolve-Path "$ParentDirectory\Static Content\XML\SearchConfiguration.xml"

#Fetch the Folder containing the XML files to add
$FolderPath = Join-Path $ParentDirectory $SharePointAppFolder

#Set Excel file path, uncomment below line if you want to use this script separately
#$ExcelFilePath = "$ParentDirectory\MCDeploymentConfig.xlsx"
    
# Set log file path, uncomment below line if you want to use this script separately
# $ErrorLogFile = "$ScriptDirectory\ErrorLog.txt"

# Set the helper files folder path, uncomment below line if you want to use this script separately
# $HelperPath = "$ScriptDirectory\Helper Utilities"

#Fetch the Catalog URL from the Excel File
Show-Message -Message "Fetching the Catalog URL from the Configuration File" -Type ([MessageType]::Warning)
$ExcelValues = Read-FromExcel $ExcelFilePath "Config" ("CatalogSiteURL", "TenantAdminURL", "IsDeployedOnAzure") $ErrorLogFile
$ExcelValues = $ExcelValues.Split(";")
if($ExcelValues.length -le 0)
{
    Write-Log $ErrorLogFile "Error reading values from Excel file. Aborting!"
    return $false
}
$webUrl = $ExcelValues[0]
$adminUrl = $ExcelValues[1]
$AppName = @("Matter Center Beta")
$AppPkgName = @("Microsoft.Legal.MatterCenter.app")

#On Premise Variables
$Source = "ObjectModel"

#Get the app names
$appPath = ''
$Files = dir $FolderPath
for ($iIterator = 0; $iIterator -lt $AppPkgName.Length; $iIterator++)
{
    for($id = 0; $id -lt $Files.Length; $id++)
    {
       if($Files[$id].FullName.Contains($AppPkgName[$iIterator]))
        {
            $appPath = $appPath + ',' + $Files[$id].FullName
            break
        }
    }
}
$appPath = $appPath.Trim(',').Split(',')

if($webUrl -ne $null)
{
	Show-Message -Message "Successfully retrieved the Catalog URL" -Type ([MessageType]::Success)
}

#Fetch the Admin URL from the Excel File
Show-Message -Message "Fetching the Admin URL from the Configuration File" -Type ([MessageType]::Warning)

if($adminUrl -ne $null)
{
	Show-Message -Message "Successfully retrieved the Admin URL" -Type ([MessageType]::Success)
}

Show-Message -Message "Proceeding to load and install the apps" -Type ([MessageType]::Warning)

if($ExcelValues[2].ToLower() -eq "true") {
    if ($IsDeploy) {
        #Load and Install Apps
        loadAndInstallSharePointApps -webUrl $webUrl -adminUrl $adminUrl -FolderPath $FolderPath -AppName $AppName[0]
		If ((Get-Content $ErrorLogFile) -eq $Null) {            
			& "$HelperPath\Microsoft.Legal.MatterCenter.UpdateListPermissions" $Username $Password
        }  
    }
    else
    {
        #Uninstall Apps
        UninstallSharePointApps -webUrl $webUrl -adminUrl $adminUrl -FolderPath $FolderPath 
    }
}
else {
    #Loads the SharePoint snapin
	Show-Message -Message "Loading SharePoint context..." -Type ([MessageType]::Warning)
    $ver = $host | select version

    if ($ver.Version.Major -gt 1) {
        $host.Runspace.ThreadOptions = "ReuseThread"
    }

    #Loads PowerShell settings
	Show-Message -Message "Loading PowerShell context..." -Type ([MessageType]::Warning)
    if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null) {
        Add-PSSnapin "Microsoft.SharePoint.PowerShell";
    }

    [void][System.Reflection.Assembly]::Load("Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c")

    if ($Source.Equals("ObjectModel", [System.StringComparison]::InvariantCultureIgnoreCase)) {
        $sourceApp = ([microsoft.sharepoint.administration.spappsource]::ObjectModel);
    }
    elseif ($Source.Equals("Marketplace", [System.StringComparison]::InvariantCultureIgnoreCase)) {
        $sourceApp = ([microsoft.sharepoint.administration.spappsource]::Marketplace);
    }
    elseif ($Source.Equals("CorporateCatalog", [System.StringComparison]::InvariantCultureIgnoreCase)) {
        $sourceApp = ([microsoft.sharepoint.administration.spappsource]::CorporateCatalog);
    }
    elseif ($Source.Equals("DeveloperSite", [System.StringComparison]::InvariantCultureIgnoreCase)) {
        $sourceApp = ([microsoft.sharepoint.administration.spappsource]::DeveloperSite);
    }
    elseif ($Source.Equals("RemoteObjectModel", [System.StringComparison]::InvariantCultureIgnoreCase)) {
        $sourceApp = ([microsoft.sharepoint.administration.spappsource]::RemoteObjectModel);
    }

    for($iIterator = 0 ; $iIterator -lt $appPath.Length; $iIterator++) {
        # Import App
		Show-Message -Message "Importing app package  $appPath[$iIterator] ..." -Type ([MessageType]::Warning)

        $spapp = Import-SPAppPackage -Path $appPath[$iIterator] -Site $webUrl -Source $sourceApp -Confirm:$false -ErrorAction SilentlyContinue -ErrorVariable err;
        
        if ($err -or ($spapp -eq $null)) {
			Show-Message -Message "An error occurred while importing the app..." -Type ([MessageType]::Failure)
            #throw $err;
        }
        else {    
			Show-Message -Message "Package imported successfully..." -Type ([MessageType]::Success)
    
            if($IsDeploy) {
               InstallAppOnPremise $AppName[$iIterator] $appPath[$iIterator] # Function call to Install an App
            }
            else {
                UninstallAppOnPremise $AppName[$iIterator] # Function call to Uninstall an App
            }
        }
    }
}