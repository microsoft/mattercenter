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
        if($credentials -eq $null)
        {
            $credentials = Get-Credential
        }
        $onlinecredential = New-Object Microsoft.SharePoint.Client.SharePointOnlineCredentials($credentials.UserName,$credentials.Password)
        $context.Credentials = $onlinecredential
    
        Connect-SPOService -Url $adminurl -Credential $credentials
        Write-Host -f Yellow "Proceeding to uninstall the apps"
    
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
                            Write-Host -NoNewline -ForegroundColor Green 'Status:' $AppInstance.Status                            
                            while($AppInstance.Status -eq 'Uninstalling') {
                                Write-Host -NoNewline -ForegroundColor Green "."
                                Start-Sleep -Seconds 10
                                $context.Load($AppInstance)
                                $context.ExecuteQuery()                                
                            }                            
                            if([String]::IsNullOrEmpty($AppInstance)) {
                                Write-Host -ForegroundColor Green "`n"$fileNameWithExtension 'has been uninstalled successfully'
                             }
                             else {                                
                                Write-Log $ErrorLogFile 'Failed to uninstall app from SharePoint, kindly uninstall manually (refer to document which contains manual steps)'
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
        if($credentials -eq $null)
        {
            $credentials = Get-Credential
        }
        $onlinecredential = New-Object Microsoft.SharePoint.Client.SharePointOnlineCredentials($credentials.UserName,$credentials.Password)
        $context.Credentials = $onlinecredential
        $sideLoadingEnabled = [Microsoft.SharePoint.Client.appcatalog]::IsAppSideloadingEnabled($context);
        $site = $context.Site;
        $context.ExecuteQuery()
    
        Write-Warning ("Checking for SideLoading feature on site " + $webUrl)
        
        if($sideLoadingEnabled.value -eq $false) 
        {
            Write-Host -f yellow "SideLoading feature is disabled on site " $webUrl
            $sideLoadingGuid = new-object System.Guid "AE3A1339-61F5-4f8f-81A7-ABD2DA956A7D"
            $site.Features.Add($sideLoadingGuid, $false, [Microsoft.SharePoint.Client.FeatureDefinitionScope]::None);
            $context.ExecuteQuery();
            Write-Host -f Green "SideLoading feature enabled on site "$webUrl
        }
        else {
            Write-Host -f Green "SideLoading feature enabled on site "$webUrl
        }
    
        Write-Host -f Yellow "Proceeding to load and install the apps"
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
                    Write-Host -NoNewline -ForegroundColor Green 'Status:' $AppInstance.Status
                    while($AppInstance.Status -eq 'Installing')
                     {                        
                       Write-Host -NoNewline -ForegroundColor Green "."
                       Start-Sleep -s 10
                       $context.Load($AppInstance)
                       $context.ExecuteQuery()
                     }
                     
                     if($AppInstance.Status -eq 'Installed') {
                        Write-Host -ForegroundColor Green "`n"$AppInstance.Title 'has been successfully loaded and installed'
                     }
                     else {                        
						Write-Log $ErrorLogFile 'Failed to install apps to SharePoint.'                         
                     }
                }
            }
        }

        #Disable Sideloading feature
        Write-Host -f yellow "Disabling SideLoading feature on site "$siteurl
    
        $sideLoadingEnabled = [Microsoft.SharePoint.Client.appcatalog]::IsAppSideloadingEnabled($context);
        $site = $context.Site;
        $context.ExecuteQuery()
    
        if($sideLoadingEnabled.value -eq $false)
        {
            Write-Host -f Green "SideLoading is already disabled on site "$webUrl
        }
        else
        {
            $sideLoadingGuid = new-object System.Guid "AE3A1339-61F5-4f8f-81A7-ABD2DA956A7D"
            $site.Features.Remove($sideLoadingGuid, $true);
            $context.ExecuteQuery();
            Write-Host -ForegroundColor Green "SideLoading disabled on site "$webUrl
        }
    }
    catch [Exception]
    {
        write-host -f red "Failed to install the app"
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

            Write-Host -ForegroundColor Yellow "Installing the App in web site..."

            $spapp = Import-SPAppPackage -Path $appPath -Site $webUrl -Source $sourceApp -Confirm:$false -ErrorAction SilentlyContinue -ErrorVariable err;
        
            $app = Install-SPApp -Web $webUrl -Identity $spapp -Confirm:$false -ErrorAction SilentlyContinue -ErrorVariable err;
        
            if ($err -or ($app -eq $null)) {
                Write-Host -ForegroundColor Yellow "An error occured while installing app..."
                #throw $err;
            }
        
            # $AppName = $app.Title;
        
            Write-Host -ForegroundColor Green "App " $AppName " registered, please wait during installation..."
        
            $appInstance = Get-SPAppInstance -Web $webUrl | where-object {$_.Title -eq $AppName};
            $counter = 1;
            $maximum = 150;
            $sleeptime = 2;
        
            Write-Host -ForegroundColor Yellow "Please wait..." -NoNewline;
        
            while (($appInstance.Status -eq ([Microsoft.SharePoint.Administration.SPAppInstanceStatus]::Installing)) -and ($counter -lt $maximum)) {
                Write-Host -ForegroundColor Yellow "." -NoNewline;
                sleep $sleeptime;
                $counter++;
                $appInstance = Get-SPAppInstance -Web $webUrl | where-object {$_.Title -eq $AppName} 
            }
        
            Write-Host -ForegroundColor Yellow ".";
        
            if ($appInstance.Status -eq [Microsoft.SharePoint.Administration.SPAppInstanceStatus]::Installed) {
                Write-Host -ForegroundColor Green "The App was successfully installed.";
                $appUrl = $appInstance.AppWebFullUrl;
            }
            else {
                Write-Host -ForegroundColor Red "An unknown error has occured during app installation. Read SharePoint log for more information...";
            }
        }
        else {
            UninstallAppOnPremise $AppName $appPath
            InstallAppOnPremise $AppName $appPath
        }
    }
    catch [exception]
    {
        write-host -f red "Failed to install the app"
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
            Write-Host -ForegroundColor Yellow "App instance detected on target web, uninstalling it...";
    
            Uninstall-SPAppInstance –Identity $appInstance -Confirm:$false -ErrorAction SilentlyContinue -ErrorVariable err;
    
            if ($err) {
                Write-Host -ForegroundColor Red "An error occured while uninstalling app...";
                #throw $err;
            }
    
            Write-Host -ForegroundColor Yellow "Please wait..." -NoNewline;
    
            $appInstance = Get-SPAppInstance -Web $webUrl | where-object {$_.Title -eq $AppName};
            $counter = 1;
            $maximum = 50;
            $sleeptime = 2;
    
            while (($appInstance -ne $null) -and ($appInstance.Status -eq ([Microsoft.SharePoint.Administration.SPAppInstanceStatus]::Uninstalling)) -and ($counter -lt $maximum)){
                Write-Host -ForegroundColor Yellow "." -NoNewline;
                sleep $sleeptime;
                $counter++;
                $appInstance = Get-SPAppInstance -Web $webUrl | where-object {$_.Title -eq $AppName} 
            }
    
            Write-Host -ForegroundColor Yellow ".";
        
            $appInstance = Get-SPAppInstance -Web $webUrl | where-object {$_.Title -eq $AppName} 
    
            if ($appInstance -eq $null) {
                Write-Host -ForegroundColor Green "App '$AppName' was successfully uninstalled..."
                Write-Host -ForegroundColor Green "  "
            }
            else {
                Write-Host -ForegroundColor Red "App '$AppName' was not successfully uninstalled..."
            }
        }
        else
        {
            Write-Warning ("App " + $AppName + " not found in web site " + $webUrl + ".")
        }
    }
    catch [exception]
    {
        write-host -f red "Failed to uninstall the app"
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

#Set Excel file path, uncomment below line if you want to use this script seperately
#$ExcelFilePath = "$ParentDirectory\MCDeploymentConfig.xlsx"
    
# Set log file path, uncomment below line if you want to use this script seperately
# $ErrorLogFile = "$ScriptDirectory\ErrorLog.txt"

# Set the helper files folder path, uncomment below line if you want to use this script seperately
# $HelperPath = "$ScriptDirectory\Helper Utilities"

#Fetch the Catalog URL from the Excel File
write-host -f yellow "Fetching the Catalog URL from the Configuration File"
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
    write-host -f Green "Successfully retrieved the Catalog URL"
}

#Fetch the Admin URL from the Excel File
write-host -f yellow "Fetching the Admin URL from the Configuration File"

if($adminUrl -ne $null)
{
    write-host -f Green "Successfully retrieved the Admin URL"
}

Write-Host -f Yellow "Proceeding to load and install the apps"

if($ExcelValues[2].ToLower() -eq "true") {
    if ($IsDeploy) {
        #Load and Install Apps
        loadAndInstallSharePointApps -webUrl $webUrl -adminUrl $adminUrl -FolderPath $FolderPath -AppName $AppName[0]
		If ((Get-Content $ErrorLogFile) -eq $Null) {            
			& "$HelperPath\Microsoft.Legal.MatterCenter.UpdateListPermissions"
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
    Write-Host -ForegroundColor Yellow "Loading SharePoint context..."
    $ver = $host | select version

    if ($ver.Version.Major -gt 1) {
        $host.Runspace.ThreadOptions = "ReuseThread"
    }

    #Loads powershell settings
    Write-Host -ForegroundColor Yellow "Loading Powershell context..."
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
        Write-Host -ForegroundColor Yellow "Importing app package " $appPath[$iIterator] "..."

        $spapp = Import-SPAppPackage -Path $appPath[$iIterator] -Site $webUrl -Source $sourceApp -Confirm:$false -ErrorAction SilentlyContinue -ErrorVariable err;
        
        if ($err -or ($spapp -eq $null)) {
            Write-Host -ForegroundColor Red "An error occured while importing the app..."
            #throw $err;
        }
        else {    
            Write-Host -ForegroundColor Green "Package imported successfully..."
    
            if($IsDeploy) {
               InstallAppOnPremise $AppName[$iIterator] $appPath[$iIterator] # Function call to Install an App
            }
            else {
                UninstallAppOnPremise $AppName[$iIterator] # Function call to Uninstall an App
            }
        }
    }
}