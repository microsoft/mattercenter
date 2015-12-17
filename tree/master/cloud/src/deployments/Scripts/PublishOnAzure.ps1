# Get the current directory of the script
Function ScriptRoot {Split-Path $MyInvocation.ScriptName}
$ScriptDirectory = (ScriptRoot)

# Get the parent directory of the script
Function Get-ParentDirectory {Split-Path -Parent(Split-Path $MyInvocation.ScriptName)}
$ParentDirectory = (Get-ParentDirectory)

#Set Excel file path, uncomment below line if you want to use this script separately
$ExcelFilePath = "$ParentDirectory\MCDeploymentConfig.xlsx"
    
# Set log file path, uncomment below line if you want to use this script separately
$ErrorLogFile = "$ScriptDirectory\Logs\ErrorLog.txt"

# Set revert log file path
$RevertLogFile = "$ScriptDirectory\Logs\RevertLog.txt"

# Set helper utilities folder path
$HelperPath = "$ScriptDirectory\Helper Utilities"

$Path = (get-item $ScriptDirectory ).parent
$ServicePackagePath = "$ParentDirectory\Service Publish"
$WebsitePackagePath = "$ParentDirectory\Web Publish"
$ExcelValues = (Read-FromExcel $ExcelFilePath "Config" ("AzureWebsiteName", "AzureWebServiceName", "TenantURL")($ErrorLogFile))
$ExcelValues = $ExcelValues.Split(";")
# Create GUID to be used for Encryption Key
$Guid = [guid]::NewGuid().ToString().ToUpperInvariant()


# Variable for Azure properties
$AzureProperties = $null

#Variable for Azure cache properties 
$MatterCenterCache = $null

# Global variables with default values
[string]$CacheName = "MatterCenterCache"
[string]$CacheLocation = "West US"
[string]$ResourceGroupName = "Default-Web-WestUS"

# Publish site on Azure and update the configurations
Function Publish-SiteOnAzure
{
    Show-Message -Message "Publishing web service on Azure"
	Publish-AzureWebsiteProject -Name $ExcelValues[1] -Package $ServicePackagePath
	Show-Message -Message "Publishing web site on Azure"
	Publish-AzureWebsiteProject -Name $ExcelValues[0] -Package $WebsitePackagePath
		
    Show-Message -Message "Updating App settings for website"
	$Website = Get-AzureWebsite -Name $ExcelValues[0] 
	$Website.AppSettings["Old_Encryption_Key"]= $Guid
	$Website.AppSettings["Encryption_Key"]= $Guid
	Set-AzureWebsite -Name $ExcelValues[0] -AppSettings $Website.AppSettings

	Show-Message -Message "Updating App settings for web service"
	$WebService = Get-AzureWebsite -Name $ExcelValues[1]

	# Accept MailCart username and Password
	
	if($null -eq $ExchangeCredential) {
	    $ExchangeCredential = Get-Credential -Message "Enter credentials to connect with Exchange server."	
	}
    $WebService.AppSettings["Mail_Cart_Mail_User_Name"]= $ExchangeCredential.UserName
	$WebService.AppSettings["Mail_Cart_Mail_Password"]= $ExchangeCredential.GetNetworkCredential().Password
	$WebService.AppSettings["Old_Encryption_Key"]= $Guid
	$WebService.AppSettings["Encryption_Key"]= $Guid
	Set-AzureWebsite -Name $ExcelValues[1] -AppSettings $WebService.AppSettings
}

# Get the azure properties of the website
Function Get-AzureWebsiteProperties
{
    $AzureProperties =  Get-AzureRmResource | 
                        Where-Object { $_.ResourceType -like '*site*' -and $_.Name -like $ExcelValues[1] } | `
                        Select-Object ResourceGroupName, Location

    
    # Return the azure properties
    return $AzureProperties
}

# Create redis cache
Function Create-RedisCache 
{
param (
    [parameter(Mandatory=$true)]                         
    [ValidateNotNullOrEmpty()]
    [String] $CacheLocation,

    [parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [String] $ResourceGroupName,

    [parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [String] $CacheName
)
    #Check if redis cache group already exists
    $IsCacheExist = $null
    $CacheInfo  = $null
    try 
    {   
        $ResourceGroups = Get-AzureRmResourceGroup             
        Foreach ($ResourceGroup IN $ResourceGroups)
        {
            # Check redis cache is already exists
            $IsCacheExist = Find-AzureRmResource -ResourceGroupNameContains $ResourceGroup.ResourceGroupName -ResourceNameContains $CacheName -Top 1 -ErrorAction SilentlyContinue
            if($null -ne  $IsCacheExist)                
             {
                $CacheInfo = Get-AzureRmRedisCache -ResourceGroupName $ResourceGroup.ResourceGroupName -Name $CacheName -ErrorAction SilentlyContinue
                $CacheExist = $true
             }
        }

        if(!$CacheExist)                
        {
            Show-Message -Message "Matter Center Redis cache doesn't exist, creating redis cache for Matter Center"
            # Create a new cache.    
             $CacheInfo = New-AzureRmRedisCache -Location $CacheLocation -Name $CacheName  -ResourceGroupName $ResourceGroupName -Size 250MB -Sku Basic

			Show-Message -Message "Redis cache is in process of creation, it will take sometime to complete! Continuing with the rest of the process" -Type ([MessageType]::Success)

            Show-Message -Message "Primary Key: $($CacheInfo.PrimaryKey)"  
            Show-Message -Message "Host name: $($CacheInfo.HostName)"                        
        } 
        else 
        {
            Write-Output "Matter Center Redis cache already exists, hence skipping the process!" 
        }
        #Return the cache details
        return $CacheInfo
		         
    }
    catch 
    {
        $ErrorMessage = $Error[0].Exception.Error.Message             
		Write-Log $ErrorLogFile $ErrorMessage
		return $null
    }        
}
# First Phase : Resource Configuration
Show-Message -Message "Please enter your Azure credentials for Azure Resource Management"
if(Login-AzureRmAccount)
{

   try{
        $AzureProperties = Get-AzureWebsiteProperties

        # Check if azure properties are not null and set it as per the service location
            if($AzureProperties -ne $null)
            {
                $CacheLocation = $AzureProperties.Location
                $ResourceGroupName = $AzureProperties.ResourceGroupName
            }

            Show-Message -Message "Creating Redis cache"
            # Call the function to create redis cache
            $cacheName = ($ExcelValues[2] -split '//')[1].Split('.')[0] + $CacheName
           $MatterCenterCache= Create-RedisCache $CacheLocation $ResourceGroupName $CacheName

        }
        catch{

              $ErrorMessage = $Error[0].Exception.ErrorRecord.Exception.Message             
		      Write-Log $ErrorLogFile $ErrorMessage
		      return $false
        }
}
else{
        return
}

# Second phase: Service Configuration and deployment

Show-Message -Message "Please enter your Azure credentials for Azure service management"
if(Add-AzureAccount)
{
	try
	{		
        # Call azure publish for site and service
        Publish-SiteOnAzure

        # Update the service appsettings with Primary key and host name of cache
         if($MatterCenterCache -ne $null)
         {
            Write-Output "Updating the service appsettings with Primary key and host name of cache"
			
			Show-Message -Message "Updating App settings for web service"
            $WebService = Get-AzureWebsite -Name $ExcelValues[1]	        
            
            $WebService.AppSettings["Cache_Primary_Key"]= $MatterCenterCache.PrimaryKey
            $WebService.AppSettings["Cache_Host_Name"]= $MatterCenterCache.HostName

	        Set-AzureWebsite -Name $ExcelValues[1] -AppSettings $WebService.AppSettings   
         }
         else
         {
            return $false

         }
	}
	catch 
	{
		$ErrorMessage = $Error[0].Exception.ErrorRecord.Exception.Message             
		Write-Log $ErrorLogFile $ErrorMessage
		return $false
	}    
}
else {
	return
}