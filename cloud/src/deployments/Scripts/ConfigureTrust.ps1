#----------------------------------------------
# Configure trust for the apps
#----------------------------------------------

# Get the current directory of the script
Function ScriptRoot {Split-Path $MyInvocation.ScriptName}
$ScriptDirectory = (ScriptRoot)

# Get the parent directory of the script
Function Get-ParentDirectory {Split-Path -Parent(Split-Path $MyInvocation.ScriptName)}
$ParentDirectory = (Get-ParentDirectory)

#Set Excel file path, uncomment below line if you want to use this script separately
#$ExcelFilePath = "$ParentDirectory\MCDeploymentConfig.xlsx"
    
# Set log file path, uncomment below line if you want to use this script separately
#$ErrorLogFile = "$ScriptDirectory\Logs\ErrorLog.txt"

# Set the helper files folder path, uncomment below line if you want to use this script separately
#$HelperPath = "$ScriptDirectory\Helper Utilities"

$trustedName = [guid]::NewGuid().ToString() #"LCADMS App4" # This name should be unique on the production environment # Change it while running delta scripts

$ExcelValues = Read-FromExcel $ExcelFilePath "Config" ("IsDeployedOnAzure", "ClientSigningCertificatePath") $ErrorLogFile

if($null -ne $ExcelValues)
{
    $ExcelValues = $ExcelValues.split(";");
    if($ExcelValues.length -le 0)
    {
        Write-Log $ErrorLogFile "Error reading values from Excel file. Aborting!"
        return $false
    }

    if("false" -eq $ExcelValues[0].ToLower()) {

        $publicCertPath = $ExcelValues[1]
        $publicCertName = $ExcelValues[1].Substring($ExcelValues[1].LastIndexOf("\") + 1)
        $publicCertName = $publicCertName.Substring(0, $publicCertName.LastIndexOf("."))

	    #Import SharePoint management shell
	    $snapin = Get-PSSnapin | Where-Object { $_.Name -eq "Microsoft.SharePoint.Powershell" }
	    if ($snapin -eq $null) {
	        Show-Message -Message "Loading SharePoint PowerShell Snapin"
            Add-PSSnapin "Microsoft.SharePoint.PowerShell"
            Show-Message -Message "Added PowerShell Snap in..."
        }

        $certificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($publicCertPath)

        #SharePoint treats the certificate as a root authority
        $spAuth = Get-SPTrustedRootAuthority | Where-Object { $_.Name -eq $publicCertName }
        if($spAuth -eq $null) {
            New-SPTrustedRootAuthority -Name $publicCertName -Certificate $certificate  # Update the certificate Name
        }

        #Get the ID of the authorization realm
        $realm = Get-SPAuthenticationRealm

        #Construct a GUID to generate the access token
        $specificIssuerId = [guid]::NewGuid()
		Show-Message -Message "Issuer Id: " $specificIssuerId -Type ([MessageType]::Success)
        Show-Message -Message "Please make a note of the Issuer Id..."

        #Construct an issuer ID in the format that SharePoint requires: specific_issuer_GUID@realm_GUID
        $fullIssuerIdentifier = [string]$specificIssuerId + '@' + $realm

        #Register the certificate as a trusted token issuer.
        New-SPTrustedSecurityTokenIssuer -Name $trustedName -Certificate $certificate -RegisteredIssuerName $fullIssuerIdentifier –IsTrustBroker # Update the certificate name
        iisreset 

        $serviceConfig = Get-SPSecurityTokenServiceConfig
        $serviceConfig.AllowOAuthOverHttp = $true
        $serviceConfig.Update()

        #Update the IssueId
        cd $ScriptDirectory
        cd "Helper Utilities"
        Show-Message -Message ""
        Show-Message -Message "Updating Issuer Id in the Web.config"
        [Environment]::CurrentDirectory = Get-Location
        & "$HelperPath\Microsoft.Legal.MatterCenter.UpdateAppConfig.exe" $specificIssuerId

        If ((Get-Content "$ScriptDirectory\Logs\ErrorLog.txt") -ne $Null) {
			Show-Message -Message "Failed to update Issuer Id..." -Type ([MessageType]::Failure)
            return
        }
        else {
			Show-Message -Message "Successfully updated Issuer Id..." -Type ([MessageType]::Success)
        }

    }
    else {
		Show-Message -Message "Online Deployment..." -Type ([MessageType]::Success)
    }
}
else {
    Write-Log -ErrorMessage:"Error in reading Excel configuration file" -ErrorLogFilePath:$ErrorLogFile
}