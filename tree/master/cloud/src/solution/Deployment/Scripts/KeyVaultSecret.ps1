#
# KeyVaultSecret.ps1
#

Param(
    [string] [Parameter(Mandatory=$true)] $AppKey,
    [string] [Parameter(Mandatory=$true)] $VaultName,
    [string] [Parameter(Mandatory=$true)] $secretvalue
)

Login-AzureRmAccount 

$secretvalue = ConvertTo-SecureString $AppKey  -AsPlainText -Force
$secret = Set-AzureKeyVaultSecret -VaultName $VaultName -Name 'General-AppKey' -SecretValue $secretvalue 