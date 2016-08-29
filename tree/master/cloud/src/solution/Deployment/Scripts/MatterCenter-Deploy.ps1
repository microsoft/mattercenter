
function Create-ADAppWithPassword{
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory=$true)]
		[ValidateNotNullOrEmpty()]
		[string]
		$ADApplicationName,

        [Parameter(Mandatory=$true)]
		[ValidateNotNullOrEmpty()]
		[string]
		$applicationURL,

		[Parameter(Mandatory=$true)]
		[ValidateNotNullOrEmpty()]
		[securestring]
		$password

    )
	
    $azureAdApplication = New-AzureRmADApplication -DisplayName $ADApplicationName -HomePage $applicationURL -IdentifierUris $applicationURL -Password $password
    New-AzureRmADServicePrincipal -ApplicationId $azureAdApplication.ApplicationId
    #New-AzureRmRoleAssignment -RoleDefinitionName Reader -ServicePrincipalName $azureAdApplication.ApplicationId

    return $azureAdApplication
}

function Create-ADAppFromCert
{
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory=$true)]
		[ValidateNotNullOrEmpty()]
		[string]
		$certFileName,

        [Parameter(Mandatory=$true)]
		[ValidateNotNullOrEmpty()]
		[string]
		$certExpiryDate,
        
        [Parameter(Mandatory=$true)]
		[ValidateNotNullOrEmpty()]
		[string]
        #eg: "matterwebappcert"
		$ADApplicationName,

        [Parameter(Mandatory=$true)]
		[ValidateNotNullOrEmpty()]
		[string]
		$applicationURL
    
    )


    #$certFileName = "MCWebApp.cer"

    $x509 = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2

    $x509.Import($certFileName)

    $credValue = [System.Convert]::ToBase64String($x509.GetRawCertData())

    $now = [System.DateTime]::Now

    # this is where the end date from the cert above is used
    $yearfromnow = [System.DateTime]::Parse($certExpiryDate)

    # will create a new AD ASpplication for the KeyVault and associate it with the certificate provided
    $adapp = New-AzureRmADApplication -DisplayName $ADApplicationName -HomePage $applicationURL -IdentifierUris $applicationURL -KeyValue $credValue -KeyType "AsymmetricX509Cert" -KeyUsage "Verify" -StartDate $now -EndDate $yearfromnow

    $sp = New-AzureRmADServicePrincipal -ApplicationId $adapp.ApplicationId

    $kv =  Get-AzureRMKeyVault
    Set-AzureRmKeyVaultAccessPolicy -VaultName $kv.VaultName -ServicePrincipalName $sp.ServicePrincipalName -PermissionsToSecrets all -ResourceGroupName $kv.ResourceGroupName

    #$returnParam:KeyVaultOutputs = $kv
    #$returnParam:ad

    return $adapp
}




function Create-KeyVaultSecrets
{  
	param 
	( 
		[Parameter (Mandatory=$true)] 
		[string] $VaultName, 
		[Parameter (Mandatory=$true)] 
		[string] $AdminUserName, 
		[Parameter (Mandatory=$true)] 
		[string] $AdminPassword,
		[Parameter (Mandatory=$true)] 
		[string] $CloudStorageConnectionString,
		[Parameter (Mandatory=$true)] 
		[string] $ClientId,
		#[Parameter (Mandatory=$true)] 
		#[string] $AppKey ,
		[Parameter (Mandatory=$true)] 
		[string] $RedisCacheHostName 
	) 
 
	$secretvalue = ConvertTo-SecureString $AdminUserName -AsPlainText -Force

	$secret = Set-AzureKeyVaultSecret -VaultName $VaultName -Name 'General-AdminUserName' -SecretValue $secretvalue

 
	$secretvalue = ConvertTo-SecureString $AdminPassword -AsPlainText -Force

	$secret = Set-AzureKeyVaultSecret -VaultName $VaultName -Name 'General-AdminPassword' -SecretValue $secretvalue

	$secretvalue = ConvertTo-SecureString $ClientId -AsPlainText -Force

	$secret = Set-AzureKeyVaultSecret -VaultName $VaultName -Name 'General-ClientId' -SecretValue $secretvalue

	#$secretvalue = ConvertTo-SecureString $AppKey  -AsPlainText -Force

	#$secret = Set-AzureKeyVaultSecret -VaultName $VaultName -Name 'General-AppKey' -SecretValue $secretvalue


	$secretvalue = ConvertTo-SecureString $CloudStorageConnectionString  -AsPlainText -Force

	$secret = Set-AzureKeyVaultSecret -VaultName $VaultName -Name 'General-CloudStorageConnectionString' -SecretValue $secretvalue

	$secretvalue = ConvertTo-SecureString $RedisCacheHostName  -AsPlainText -Force

	$secret = Set-AzureKeyVaultSecret -VaultName $VaultName -Name 'General-RedisCacheHostName' -SecretValue $secretvalue

	$secretvalue = ConvertTo-SecureString $CloudStorageConnectionString  -AsPlainText -Force

	$secret = Set-AzureKeyVaultSecret -VaultName $VaultName -Name 'Data-DefaultConnection-AzureStorageConnectionString' -SecretValue $secretvalue

	$secretvalue = ConvertTo-SecureString $ClientId   -AsPlainText -Force

	$secret = Set-AzureKeyVaultSecret -VaultName $VaultName -Name 'ADAL-clientId' -SecretValue $secretvalue
}


#Login-AzureRmAccount
$creds = Get-Credential

#$sp = Get-Location
#$parametersJsonPath = (Get-Item $sp.Path).Parent


$params = ConvertFrom-Json -InputObject (Get-Content -Path $TemplateParametersFile -Raw)
#$params = ConvertFrom-Json -InputObject (Get-Content -Path (Join-Path -Path $parametersJsonPath -ChildPath "/Templates/parameters.json") -Raw)
#$params.parameters.PlatformLoggingStorageAccountSettings.value.alertEmail = $Private:setup.Core.PlatformLoggingAlertEmail
#Set-Content -Path (Join-Path -Path $Global:ArtifactsPath -ChildPath "Core.Storage.param.json") -Value (ConvertTo-Json -InputObject $Private:CoreStorageParams -Depth 3)
$appURL = [string]::format("https://{0}.azurewebsites.net", $params.parameters.webSite_name.value)
	$appURL
	$params.parameters.Web_ADApp_Name.value
	$creds.Password
#$webADApp = Create-ADAppWithPassword -ADApplicationName $params.parameters.Web_ADApp_Name -applicationURL $appURL -password $creds.Password

$webADApp = New-AzureRmADApplication -DisplayName $params.parameters.Web_ADApp_Name.value -HomePage $appURL -IdentifierUris $appURL -Password $creds.Password
    New-AzureRmADServicePrincipal -ApplicationId $webADApp.ApplicationId
    

#creating the keyVault
#New-AzureRmKeyVault -VaultName MatterKeyVault -ResourceGroupName $ResourceGroupName -Location 'West US'


#Set-AzureRmKeyVaultAccessPolicy -VaultName vaults_KeyVault_name -ServicePrincipalName 'b9de791e-0b7b-402a-a3fa-d2a26f463783' -PermissionsToSecrets all -PermissionsToKeys all 

$certFilePath = [System.IO.Path]::Combine($PSScriptRoot, 'mattercentercert.pfx')

$kvADApp = Create-ADAppFromCert -certFileName $certFilePath -certExpiryDate $params.parameters.KeyVault_certificate_expiryDate.value -ADApplicationName $params.parameters.KeyVault_ADApp_Name.value -applicationURL $appURL

$storageKey = Get-AzureRmStorageAccountKey -Name mattercent -ResourceGroupName test26aug
$storageConnString =  [string]::format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", $params.parameters.storageAccount_name.value, $storageKey.Item(0).Value)

Create-KeyVaultSecrets -VaultName $params.parameters.vaults_KeyVault_name.value -AdminUserName $creds.UserName -AdminPassword $creds.Password [-CloudStorageConnectionString] -ClientId $kvADApp.ApplicationId -RedisCacheHostName $params.parameters.Redis_cache_name.value 

