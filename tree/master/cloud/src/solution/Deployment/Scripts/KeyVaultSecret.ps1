#
# KeyVaultSecret.ps1
#

Param(
    [string] [Parameter(Mandatory=$true, HelpMessage="You can find this value in deployment log file. `
	ex: from VaultUri https://mattercenterw09022324.vault.azure.net mattercenterw09022324 is vaultName ")] $VaultName,
    [string] [Parameter(Mandatory=$true, HelpMessage="This is Azure AD application key. `
	You can get key by following steps in https://azure.microsoft.com/en-us/documentation/articles/resource-group-create-service-principal-portal/#get-client-id-and-authentication-key")] $ADApplicationKey
)

Login-AzureRmAccount 

$secretvalue = ConvertTo-SecureString $ADApplicationKey -AsPlainText -Force
$secret = Set-AzureKeyVaultSecret -VaultName $VaultName -Name 'General-AppKey' -SecretValue $secretvalue 