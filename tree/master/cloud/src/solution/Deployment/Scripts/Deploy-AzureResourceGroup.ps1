<#
.SYNOPSIS
    .
.DESCRIPTION
    .
.PARAMETER Path
    The path to the .
.PARAMETER LiteralPath
    Specifies a path to one or more locations. Unlike Path, the value of 
    LiteralPath is used exactly as it is typed. No characters are interpreted 
    as wildcards. If the path includes escape characters, enclose it in single
    quotation marks. Single quotation marks tell Windows PowerShell not to 
    interpret any characters as escape sequences.
.EXAMPLE
    C:\PS> 
    <Description of example>
.NOTES
    Author: Matter Center Core Team
    Date:   Sept 02, 2016    
#>

Param(
    [string] [Parameter(Mandatory=$true, HelpMessage="ex: west us")] $ResourceGroupLocation,
    [string] [Parameter(Mandatory=$true, HelpMessage="ex: MatterCenterRG")] $ResourceGroupName = 'MatterCenterRG',
    [string] [Parameter(Mandatory=$true, HelpMessage="ex: MatterCenterWeb")] $WebAppName = 'MatterCenterWeb',
	[string] [Parameter(Mandatory=$true, HelpMessage="Provide the catalog site url you used during sharepoint site deployment. `
	it will be https://<tenantname>.sharepoinT.com/sites/catalog if you didnt change default catalog site.")] $CentralRepositoryUrl,
	
    [switch] $UploadArtifacts,
    [string] $StorageAccountName,
    [string] $StorageAccountResourceGroupName, 
    [string] $StorageContainerName = $ResourceGroupName.ToLowerInvariant() + '-stageartifacts',
    [string] $TemplateFile = '..\Templates\template.json',
    [string] $TemplateParametersFile = '..\Templates\template.parameters.json',
    [string] $ArtifactStagingDirectory = '..\bin\Debug\staging',
    [string] $AzCopyPath = '..\Tools\AzCopy.exe',
    [string] $DSCSourceFolder = '..\DSC'
)
$logFileName = "MCDeploy"+(Get-Date).ToString('yyyyMMdd-HHmmss')+".log"
Start-Transcript -path $logFileName
$WebAppName = $WebAppName + ((Get-Date).ToUniversalTime()).ToString('MMddHHmm')
if($WebAppName.Length -gt 60)
{
	$WebAppName =  $WebAppName.Substring(0,60)
}

#Get sharepoint site root url from respository
$CentralRepositoryUrl = $CentralRepositoryUrl.ToLower()
$indexOfSPO = $CentralRepositoryUrl.IndexOf(".com")
$SiteURL = $CentralRepositoryUrl.Substring(0, $indexOfSPO + 4)

$Redis_cache_name = $WebAppName+"RedisCache"
$autoscalesettings_name = $WebAppName+"ScaleSettings"
$components_AppInsights_name = $WebAppName+"AppInsights"
if($WebAppName.Length -gt 24)
{
	$vaults_KeyVault_name = $WebAppName.Substring(0,24)
	$storageAccount_name = $WebAppName.Substring(0,24)
}
else
{
	$vaults_KeyVault_name = $WebAppName
	$storageAccount_name = $WebAppName
}
$serverfarms_WebPlan_name = $WebAppName+"WebPlan"
$ADApp_Name = $WebAppName+"ADApp"
$global:thumbPrint = ""
$global:appInsightsId = ""
$storageAccount_name 
$ADApplicationId = ""
Write-Output "Reading from template.parameters.json file..."
$params = ConvertFrom-Json -InputObject (Get-Content -Path $TemplateParametersFile -Raw)
$params.parameters.webSite_name.value = $WebAppName
Set-Content -Path $TemplateParametersFile -Value (ConvertTo-Json -InputObject $params -Depth 3)


Import-Module Azure -ErrorAction SilentlyContinue
#Add-AzureAccount
$subsc = Login-AzureRmAccount
$global:TenantName = $subsc.Context.Tenant.Domain
#$Tenant_id = $subsc.Context.Tenant.TenantId

try {
 #   [Microsoft.Azure.Common.Authentication.AzureSession]::ClientFactory.AddUserAgent("VSAzureTools-$UI$($host.name)".replace(" ","_"), "2.8")
} catch { }

Set-StrictMode -Version 3

$OptionalParameters = New-Object -TypeName Hashtable
$TemplateFile = [System.IO.Path]::Combine($PSScriptRoot, $TemplateFile)
$TemplateParametersFile = [System.IO.Path]::Combine($PSScriptRoot, $TemplateParametersFile)

if ($UploadArtifacts) {
    # Convert relative paths to absolute paths if needed
    $AzCopyPath = [System.IO.Path]::Combine($PSScriptRoot, $AzCopyPath)
    $ArtifactStagingDirectory = [System.IO.Path]::Combine($PSScriptRoot, $ArtifactStagingDirectory)
    $DSCSourceFolder = [System.IO.Path]::Combine($PSScriptRoot, $DSCSourceFolder)

    Set-Variable ArtifactsLocationName '_artifactsLocation' -Option ReadOnly -Force
    Set-Variable ArtifactsLocationSasTokenName '_artifactsLocationSasToken' -Option ReadOnly -Force

    $OptionalParameters.Add($ArtifactsLocationName, $null)
    $OptionalParameters.Add($ArtifactsLocationSasTokenName, $null)

    # Parse the parameter file and update the values of artifacts location and artifacts location SAS token if they are present
    $JsonContent = Get-Content $TemplateParametersFile -Raw | ConvertFrom-Json
    $JsonParameters = $JsonContent | Get-Member -Type NoteProperty | Where-Object {$_.Name -eq "parameters"}

    if ($JsonParameters -eq $null) {
        $JsonParameters = $JsonContent
    }
    else {
        $JsonParameters = $JsonContent.parameters
    }

    $JsonParameters | Get-Member -Type NoteProperty | ForEach-Object {
        $ParameterValue = $JsonParameters | Select-Object -ExpandProperty $_.Name

        if ($_.Name -eq $ArtifactsLocationName -or $_.Name -eq $ArtifactsLocationSasTokenName) {
            $OptionalParameters[$_.Name] = $ParameterValue.value
        }
    }

    $StorageAccountKey = (Get-AzureRmStorageAccountKey -ResourceGroupName $StorageAccountResourceGroupName -Name $StorageAccountName).Key1

    $StorageAccountContext = (Get-AzureRmStorageAccount -ResourceGroupName $StorageAccountResourceGroupName -Name $StorageAccountName).Context

    # Create DSC configuration archive
    if (Test-Path $DSCSourceFolder) {
        Add-Type -Assembly System.IO.Compression.FileSystem
        $ArchiveFile = Join-Path $ArtifactStagingDirectory "dsc.zip"
        Remove-Item -Path $ArchiveFile -ErrorAction SilentlyContinue
        [System.IO.Compression.ZipFile]::CreateFromDirectory($DSCSourceFolder, $ArchiveFile)
    }

    # Generate the value for artifacts location if it is not provided in the parameter file
    $ArtifactsLocation = $OptionalParameters[$ArtifactsLocationName]
    if ($ArtifactsLocation -eq $null) {
        $ArtifactsLocation = $StorageAccountContext.BlobEndPoint + $StorageContainerName
        $OptionalParameters[$ArtifactsLocationName] = $ArtifactsLocation
    }

    # Use AzCopy to copy files from the local storage drop path to the storage account container
    & $AzCopyPath """$ArtifactStagingDirectory""", $ArtifactsLocation, "/DestKey:$StorageAccountKey", "/S", "/Y", "/Z:$env:LocalAppData\Microsoft\Azure\AzCopy\$ResourceGroupName"
    if ($LASTEXITCODE -ne 0) { return }

    # Generate the value for artifacts location SAS token if it is not provided in the parameter file
    $ArtifactsLocationSasToken = $OptionalParameters[$ArtifactsLocationSasTokenName]
    if ($ArtifactsLocationSasToken -eq $null) {
        # Create a SAS token for the storage container - this gives temporary read-only access to the container
        $ArtifactsLocationSasToken = New-AzureStorageContainerSASToken -Container $StorageContainerName -Context $StorageAccountContext -Permission r -ExpiryTime (Get-Date).AddHours(4)
        $ArtifactsLocationSasToken = ConvertTo-SecureString $ArtifactsLocationSasToken -AsPlainText -Force
        $OptionalParameters[$ArtifactsLocationSasTokenName] = $ArtifactsLocationSasToken
    }
}

# Create or update the resource group using the specified template file and template parameters file
New-AzureRmResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation -Verbose -Force -ErrorAction Stop 

New-AzureRmResourceGroupDeployment -Name ((Get-ChildItem $TemplateFile).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')) `
                                   -ResourceGroupName $ResourceGroupName `
                                   -TemplateFile $TemplateFile `
                                   -TemplateParameterFile $TemplateParametersFile `
                                   @OptionalParameters `
                                   -Force -Verbose

$creds = Get-Credential -Message "Enter credentials for connecting to Azure"

Write-Output "Getting the storage key to write to key vault..."
$StorageAccountKey = Get-AzureRmStorageAccountKey -Name $storageAccount_name -ResourceGroupName $ResourceGroupName

#Write-Output "Getting the Redis connection string"
$RedisCacheName = (Get-AzureRmRedisCache -Name $Redis_cache_name -ResourceGroupName $ResourceGroupName )[0].HostName
$RedisCachePort = (Get-AzureRmRedisCache -Name $Redis_cache_name -ResourceGroupName $ResourceGroupName )[0].Port
$RedisCacheKey = (Get-AzureRmRedisCacheKey -Name $Redis_cache_name -ResourceGroupName $ResourceGroupName ).PrimaryKey
$redisConnString = [string]::format("{0}:{1},password={2},ssl=True,abortConnect=False", $RedisCacheName, $RedisCachePort,  $RedisCacheKey)


# Set helper utilities folder path
$RootPath = Split-Path(Split-Path (Split-Path $PSScriptRoot -Parent) -Parent) -Parent
$DeployPath = "$RootPath\deployments"
$HelperPath = "$RootPath\deployments\scripts\Helper Utilities"
$ExcelFilePath = "$RootPath\deployments\MCDeploymentConfig.xlsx"
$SPCredential = Get-Credential -Message "Enter credentials to access SharePoint tenant."
$SPPassword = $SPCredential.GetNetworkCredential().Password

cd $HelperPath
Write-Output "Getting the result source ID..."
$SearchResultSourceId = & ".\Microsoft.Legal.MatterCenter.UpdateAppConfig.exe" "4" $SPCredential.UserName $SPPassword
$SearchResultSourceId.ToString()
cd $PSScriptRoot


$custScriptFile = [System.IO.Path]::Combine($PSScriptRoot, 'KeyVault-Config.ps1')
Invoke-Expression $custScriptFile 

$storageScriptFile = [System.IO.Path]::Combine($PSScriptRoot, 'Create-AzureStorageTable.ps1')
Invoke-Expression $storageScriptFile

$webJobScriptFile = [System.IO.Path]::Combine($PSScriptRoot, 'Create-MatterCenterWebJob.ps1')
Invoke-Expression $webJobScriptFile

$spoDeployFiles = [System.IO.Path]::Combine($PSScriptRoot, 'Deploy-SPOFiles.ps1')
Invoke-Expression $spoDeployFiles

Stop-Transcript 