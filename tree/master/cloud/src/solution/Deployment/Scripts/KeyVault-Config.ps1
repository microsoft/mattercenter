
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
    $adapp = New-AzureRmADApplication -DisplayName $ADApplicationName -HomePage $applicationURL -IdentifierUris $applicationURL -CertValue $credValue -StartDate $now -EndDate $yearfromnow
	$adapp
    $sp = New-AzureRmADServicePrincipal -ApplicationId $adapp.ApplicationId
	$sp

    #$kv =  Get-AzureRMKeyVault 
    Set-AzureRmKeyVaultAccessPolicy -VaultName $vaults_KeyVault_name -ServicePrincipalName $sp.ApplicationId -PermissionsToSecrets all -ResourceGroupName $ResourceGroupName

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


$appURL = [string]::format("https://{0}.azurewebsites.net", $WebAppName)

Write-Host "Creating AD Application for web..."
#$webADApp = Create-ADAppWithPassword -ADApplicationName $params.parameters.Web_ADApp_Name -applicationURL $appURL -password $creds.Password

$webADApp = New-AzureRmADApplication -DisplayName $Web_ADApp_Name -HomePage $appURL -IdentifierUris $appURL -Password $creds.Password
$webADAppSp = New-AzureRmADServicePrincipal -ApplicationId $webADApp.ApplicationId

$webADAppSp    

#creating the keyVault
Write-Host "Creating Keyvault..."
New-AzureRmKeyVault -VaultName $vaults_KeyVault_name -ResourceGroupName $ResourceGroupName -Location $ResourceGroupLocation
Write-Host "Setting access policy for AD Application for web to key vault..."
Set-AzureRmKeyVaultAccessPolicy -VaultName $vaults_KeyVault_name -ServicePrincipalName $webADAppSp.ApplicationId -PermissionsToSecrets all -PermissionsToKeys all 

$certFilePath = [System.IO.Path]::Combine($PSScriptRoot, 'MatterWebApp.cer')

Write-Host "Creating AD Application for key vault..."
$KVappURL = "https://"+$WebAppName+((Get-Date).ToUniversalTime()).ToString('MMddHHmm')+".azurewebsites.net"
$KVappURL 
$kvADApp = Create-ADAppFromCert -certFileName $certFilePath -certExpiryDate $KeyVault_certificate_expiryDate -ADApplicationName $KeyVault_ADApp_Name -applicationURL $KVappURL 
$kvADApp

$storageConnString =  [string]::format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", $storageAccount_name, $StorageAccountKey.Item(0).Value)

Write-Host "Writing secrets to key vault..."
Create-KeyVaultSecrets -VaultName $vaults_KeyVault_name -AdminUserName $creds.UserName -AdminPassword $creds.Password -CloudStorageConnectionString $storageConnString -ClientId $kvADApp.ApplicationId.Guid.ToString() -RedisCacheHostName $Redis_cache_name 

