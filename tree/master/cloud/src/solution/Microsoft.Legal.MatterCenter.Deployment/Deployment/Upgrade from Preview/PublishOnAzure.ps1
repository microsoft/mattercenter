# Get the current directory of the script
Function ScriptRoot {Split-Path $MyInvocation.ScriptName}
$ScriptDirectory = (ScriptRoot)

# Get the parent directory of the script
Function Get-ParentDirectory {Split-Path -Parent(Split-Path $MyInvocation.ScriptName)}
$ParentDirectory = (Get-ParentDirectory)

#Set Excel file path, uncomment below line if you want to use this script seperately
#$ExcelFilePath = "$ParentDirectory\MCDeploymentConfig.xlsx"
    
# Set log file path, uncomment below line if you want to use this script seperately
#$ErrorLogFile = "$ScriptDirectory\ErrorLog.txt"

$Path = (get-item $ScriptDirectory ).parent
$ServicePackagePath = "$ParentDirectory\Service Publish"
$WebsitePackagePath = "$ParentDirectory\Web Publish"
$ExcelValues = (Read-FromExcel $ExcelFilePath "Config" ("AzureWebsiteName", "AzureWebServiceName", "MailCartUserName","MailCartPassword","TenantURL")($ErrorLogFile))
$ExcelValues = $ExcelValues.Split(";")
# Create guid to be used for Encryption Key
$Guid = [guid]::NewGuid().ToString().ToUpperInvariant()


# Azure properties of the variable
$AzureProperties = $null

# Global variables with default values
[string]$CacheName = "MatterCenterCache"
[string]$CacheLocation = "West US"
[string]$ResourceGroupName = "Default-Web-WestUS"

# Publish site on Azure and update the configurations
Function Publish-SiteOnAzure
{
    Write-Host "Publishing web service on Azure"
	Publish-AzureWebsiteProject -Name $ExcelValues[1] -Package $ServicePackagePath
	Write-Host "Publishing web site on Azure"
	Publish-AzureWebsiteProject -Name $ExcelValues[0] -Package $WebsitePackagePath
		
    Write-Host "Updating App settings for website"
	$Website = Get-AzureWebsite -Name $ExcelValues[0] 
	$Website.AppSettings["Old_Encryption_Key"]= $Guid
	$Website.AppSettings["Encryption_Key"]= $Guid
	Set-AzureWebsite -Name $ExcelValues[0] -AppSettings $Website.AppSettings

	Write-Host "Updating App settings for web service"
	$WebService = Get-AzureWebsite -Name $ExcelValues[1]
	$WebService.AppSettings["Mail_Cart_Mail_User_Name"]= $ExcelValues[2]
	$WebService.AppSettings["Mail_Cart_Mail_Password"]= $ExcelValues[3]
	$WebService.AppSettings["Old_Encryption_Key"]= $Guid
	$WebService.AppSettings["Encryption_Key"]= $Guid
	Set-AzureWebsite -Name $ExcelValues[1] -AppSettings $WebService.AppSettings
}

# Get the azure properties of the website
Function Get-AzureWebsiteProperties
{
    $AzureProperties =  Get-AzureResource | `
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
    try 
    {   
        $ResourceGroups = Get-AzureResourceGroup             
        Foreach ($ResourceGroup IN $ResourceGroups)
        {
            $MatterCenterCache = Get-AzureRedisCache -ResourceGroupName $ResourceGroup.ResourceGroupName -Name $CacheName -ErrorAction SilentlyContinue
            if($null -ne $MatterCenterCache)                
             {
                $CacheExist = $true
             }
        }                

        if(!$CacheExist)                
        {
            Write-Host "Matter Center Redis cache doesn't exist, creating redis cache for Matter Center" -ForegroundColor White
            # Create a new cache.    
            $MatterCenterCache = New-AzureRedisCache -Location $CacheLocation -Name $CacheName  -ResourceGroupName $ResourceGroupName -Size 250MB -Sku Basic

            Write-Host "Redis cache is in process of creation, it will take sometime to complete! Continuing with the rest of the process" -ForegroundColor Green

            Write-Host "Primary Key: $($MatterCenterCache.PrimaryKey)"  
            Write-Host "Host name: $($MatterCenterCache.HostName)"                        
        } 
        else 
        {
            Write-Output "Matter Center Redis cache already exists, hence skipping the process!" 
        }
			Write-Output "Updating the service appsettings with Primary key and host name of cache"

            # Switch the azure mode to service to update the app settings
            Switch-AzureMode -Name:AzureServiceManagement

            Write-Host "Updating App settings for web service"
            $WebService = Get-AzureWebsite -Name $ExcelValues[1]	        
             
            $WebService.AppSettings["Cache_Primary_Key"]= $MatterCenterCache.PrimaryKey          
            $WebService.AppSettings["Cache_Host_Name"]= $MatterCenterCache.HostName       
	        	        
	        Set-AzureWebsite -Name $ExcelValues[1] -AppSettings $WebService.AppSettings            
    }
    catch 
    {
        return $false
    }        
}



Write-Host "Please enter your Azure credentials"
if(Add-AzureAccount)
{
	try
	{		
        # Switch the azure mode to service management state to update the properties of other items
        Switch-AzureMode -Name:AzureServiceManagement

        # Call azure publish for site and service
        Publish-SiteOnAzure
        
        #Switch to azure resource manager mode
        Switch-AzureMode AzureResourceManager

        $AzureProperties = Get-AzureWebsiteProperties

        # Check if azure properties are not null and set it as per the service location
        if($AzureProperties -ne $null)
        {
            $CacheLocation = $AzureProperties.Location
            $ResourceGroupName = $AzureProperties.ResourceGroupName
        }
        
        Write-Host "Creating Redis cache"
        # Call the function to create redis cache
        $cacheName = ($ExcelValues[4] -split '//')[1].Split('.')[0] + $CacheName
        Create-RedisCache $CacheLocation $ResourceGroupName $CacheName
	}
	catch 
	{
		return $false
	}    
}
else {
	return
}