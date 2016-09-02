
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
		$ADApplicationName,

        [Parameter(Mandatory=$true)]
		[ValidateNotNullOrEmpty()]
		[string]
		$applicationURL
    
    )


    #$certFileName = "MCWebApp.cer"

	$dnsname =  [string]::format("{0}.azurewebsites.net", $WebAppName)
 
    $crt = New-SelfSignedCertificate -DnsName $dnsname -CertStoreLocation cert:\CurrentUser\My
    
    #$mypwd = ConvertTo-SecureString -String $creds.Password -Force –AsPlainText
 
    $expfxprt = Export-PfxCertificate -cert $crt -FilePath "$PSScriptRoot\MatterWebApp.pfx" -Password  $creds.Password
 
    Export-Certificate -Cert $crt -FilePath "$PSScriptRoot\MatterWebApp.cer"
 
    New-AzureRmWebAppSSLBinding -ResourceGroupName $ResourceGroupName -WebAppName $WebAppName.ToLowerInvariant() -CertificateFilePath "$PSScriptRoot\MatterWebApp.pfx" -CertificatePassword $creds.GetNetworkCredential().Password -Name $dnsname
    
    Write-Host "Certificate uploaded successfully.."

	$credValue = [System.Convert]::ToBase64String($crt.GetRawCertData())
 
    $now = [System.DateTime]::Now
	Write-Host "Creating Azure RM AD Application"

	$adapp = New-AzureRmADApplication -DisplayName $ADApplicationName -HomePage $applicationURL -IdentifierUris $applicationURL -CertValue $credValue -StartDate $crt.NotBefore -EndDate $crt.NotAfter
    
	$sp = New-AzureRmADServicePrincipal -ApplicationId $adapp.ApplicationId
	
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
		[string] $RedisCacheHostName ,
		[Parameter (Mandatory=$true)] 
		[string] $AppInsightsInstrumentationKey

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

	$secretvalue = ConvertTo-SecureString $AppInsightsInstrumentationKey  -AsPlainText -Force

	$secret = Set-AzureKeyVaultSecret -VaultName $VaultName -Name 'ApplicationInsights-InstrumentationKey' -SecretValue $secretvalue
}


$appURL = [string]::format("https://{0}.azurewebsites.net", $WebAppName)

#creating the keyVault
Write-Output "Creating Keyvault..."
New-AzureRmKeyVault -VaultName $vaults_KeyVault_name -ResourceGroupName $ResourceGroupName -Location $ResourceGroupLocation

$kvADApp = Create-ADAppFromCert -ADApplicationName $ADApp_Name -applicationURL $appURL

Write-Output "END: creating AD app. Return value is ..."

For ($i=0; $i -lt $kvADApp.Length; $i++) {
		if ($kvADApp[$i] -ne $null)
		{
			if (Get-Member -InputObject $kvADApp[$i] -Name 'ApplicationId' -MemberType Properties)
			{
				if( $kvADApp[$i].ApplicationId -ne $null)
				{
					$ADApplicationId = $kvADApp[$i].ApplicationId.Guid.ToString()
				}
			}
		}
    }

Write-Output "Writing for AppGuid $ADApplicationId"

$storageConnString =  [string]::format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", $storageAccount_name, $StorageAccountKey.Item(0).Value)

$AppInsightsApp = Get-AzureRmResource -ResourceType "Microsoft.Insights/components" -ResourceGroupName $ResourceGroupName -ResourceName $components_AppInsights_name

Write-Output "Writing secrets to key vault..."
Create-KeyVaultSecrets -VaultName $vaults_KeyVault_name -AdminUserName $creds.UserName -AdminPassword $creds.Password -CloudStorageConnectionString $storageConnString -ClientId  $ADApplicationId -RedisCacheHostName $Redis_cache_name -AppInsightsInstrumentationKey $AppInsightsApp.Properties.InstrumentationKey 

