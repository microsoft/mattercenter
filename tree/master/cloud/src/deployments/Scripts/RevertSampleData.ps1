# Accept user credentials
$SPCredential = Get-Credential -Message "Enter credentials to access SharePoint tenant."
If ($Null -eq $SPCredential) {
	# Display error message and exit
	Write-Host "Failed to get credentials" -ForegroundColor Red
	return
}

# Function to get the root path of the script
Function ScriptRoot {Split-Path $MyInvocation.ScriptName}
$ScriptDirectory = (ScriptRoot)

# Set the helper files folder path
$HelperPath = "$ScriptDirectory\Helper Utilities"

cd $HelperPath
Write-host ""
Write-host "Deleting sample data"
try {
    & "$HelperPath\Microsoft.Legal.MatterCenter.CreateSampleData.exe" "false" $SPCredential.UserName $SPCredential.GetNetworkCredential().Password
}
catch {        
    Write-Log ("$ScriptDirectory\Logs\RevertLog.txt") ("Could not delete sample data" + $Error[0])
}
cd..