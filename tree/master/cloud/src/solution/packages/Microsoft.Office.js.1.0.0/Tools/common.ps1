

# Get the project item for the scripts folder
try {
    $scriptsFolderProjectItem = $project.ProjectItems.Item("Scripts")
    $projectScriptsFolderPath = $scriptsFolderProjectItem.FileNames(1)
}
catch {
    # No Scripts folder
    Write-Host "No scripts folder found"
}


$packageScriptsFolder = Join-Path $installPath Content\Scripts\Office
$officeVersionFolder = $packageScriptsFolder | Get-ChildItem | where {$_.PsIsContainer} | Split-Path -Leaf
$officeVersionFolder -match "((\d+\.)(\d+))"
$officeVersion = $matches[1]

$officeBetaRegEx = "[Oo][Ff][Ff][Ii][Cc][Ee]/[Oo][Ff][Ff][Ii][Cc][Ee]\.[Jj][Ss]"
    
$officeintellisensefile = "_officeintellisense.js"
$officeLocalRegEx = "[Oo][Ff][Ff][Ii][Cc][Ee]/((\d+\.)(\d+))/[Oo][Ff][Ff][Ii][Cc][Ee]\.[Jj][Ss]"
$outlookCDNRegEx = "[Hh][Tt][Tt][Pp][Ss]://[Aa][Pp][Pp][Ss][Ff][Oo][Rr][Oo][Ff][Ff][Ii][Cc][Ee]\.[Mm][Ii][Cc][Rr][Oo][Ss][Oo][Ff][Tt]\.[Cc][Oo][Mm]/[Ll][Ii][Bb]/(\d+\.)(\d+)/[Hh][Oo][Ss][Tt][Ee][Dd]/[Oo][Uu][Tt][Ll][Oo][Oo][Kk]-15\.[Dd][Ee][Bb][Uu][Gg]\.[Jj][Ss]"
$officeCDNRegEx = "[Hh][Tt][Tt][Pp][Ss]://[Aa][Pp][Pp][Ss][Ff][Oo][Rr][Oo][Ff][Ff][Ii][Cc][Ee]\.[Mm][Ii][Cc][Rr][Oo][Ss][Oo][Ff][Tt]\.[Cc][Oo][Mm]/[Ll][Ii][Bb]/(\d+\.)(\d+)/[Hh][Oo][Ss][Tt][Ee][Dd]/[Oo][Ff][Ff][Ii][Cc][Ee]\.[Jj][Ss]"

$newOfficeLocalPath = "office/$officeVersion/office.js"
$newOutlookCDNPath = "https://appsforoffice.microsoft.com/lib/$officeVersion/hosted/outlook-15.debug.js"
$newOfficeCDNPath = "https://appsforoffice.microsoft.com/lib/$officeVersion/hosted/office.js"

$officeCommentRegEx = "/\* Required to correctly initalize Office.js for intellisense \*/"
$onlineCommentRegEx = "/\* Use online copy of Office.js for intellisense \*/"
$offlineCommentRegEx = "/\* Use offline copy of Office.js for intellisense \*/"

$officeComment = "/* Required to correctly initalize Office.js for intellisense */"
$onlineComment = "/* Use online copy of Office.js for intellisense */"
$offlineComment = "/* Use offline copy of Office.js for intellisense */"
    

function AddOrUpdate-Reference($scriptsFolderProjectItem, $regExPattern, $newFullPath , $commentOut) {
    try {
        $referencesFileProjectItem = $scriptsFolderProjectItem.ProjectItems.Item("_references.js")
    }
    catch {
        # _references.js file not found
        return
    }

    if ($referencesFileProjectItem -eq $null) {
        # _references.js file not found
        return
    }

    $referencesFilePath = $referencesFileProjectItem.FileNames(1)
    $referencesTempFilePath = Join-Path $env:TEMP "_references.tmp.js"    

  
    $addCondition = Select-String $referencesFilePath -pattern $regExPattern -quiet
    #Write-Host "Add condition check $addCondition"
    if ($addCondition -eq $false) {
        #Write-Host "No existing reference found"
	# File has no existing matching reference line
        # Add the full reference line to the beginning of the file
        if ($regExPattern -eq $officeintellisensefile) {
        $officeComment | Add-Content $referencesTempFilePath -Encoding UTF8
	#Write-Host "Add Comment for intellisense"
        }
        elseif ($regExPattern -eq $officeLocalRegEx) {
        $offlineComment | Add-Content $referencesTempFilePath -Encoding UTF8
	#Write-Host "Add comment for Local Office.js reference"
        }
        elseif ($regExPattern -eq $outlookCDNRegEx) {
        $onlineComment | Add-Content $referencesTempFilePath -Encoding UTF8
	#Write-Host "Add comment for CDN reference"
        }
        if ($commentOut -eq "True"){
        "// /// <reference path=""$newFullPath"" />" | Add-Content $referencesTempFilePath -Encoding UTF8
       	#Write-Host "Add Comment to $newFullPath"
	}
        else {
        "/// <reference path=""$newFullPath"" />" | Add-Content $referencesTempFilePath -Encoding UTF8
	#Write-Host "Add Reference to $newFullPath"
        }
         Get-Content $referencesFilePath | Add-Content $referencesTempFilePath
    }
    else {
        #Write-Host "Existing reference found"
	# Loop through file and replace old file name with new file name
        Get-Content $referencesFilePath | ForEach-Object { $_ -replace $regExPattern, $newFullPath } > $referencesTempFilePath
    }


    # Copy over the new _references.js file
    Copy-Item $referencesTempFilePath $referencesFilePath -Force
    Remove-Item $referencesTempFilePath -Force
}


function Remove-Reference($scriptsFolderProjectItem , $regExPattern) {
    try {
        $referencesFileProjectItem = $scriptsFolderProjectItem.ProjectItems.Item("_references.js")
    }
    catch {
        # _references.js file not found
        return
    }

    if ($referencesFileProjectItem -eq $null) {
        return
    }

    $referencesFilePath = $referencesFileProjectItem.FileNames(1)
    $referencesTempFilePath = Join-Path $env:TEMP "_references.tmp.js"
   
    $removeCondition = Select-String $referencesFilePath -pattern $regExPattern -quiet
    #Write-Host "Remove condition check $removeCondition"
    if ($removeCondition -eq $True) {
        #Write-Host "Removing Reference $regExPattern"
	# Delete the line referencing the file
        Get-Content $referencesFilePath | ForEach-Object { if (-not ($_ -match $regExPattern)) { $_ } } > $referencesTempFilePath

        # Copy over the new _references.js file
        Copy-Item $referencesTempFilePath $referencesFilePath -Force
        Remove-Item $referencesTempFilePath -Force
    }
}


# SIG # Begin signature block
# MIIaTgYJKoZIhvcNAQcCoIIaPzCCGjsCAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUf3AKO+0w5LskNqXDy1C4OCuS
# qHagghU2MIIEqTCCA5GgAwIBAgITMwAAAIhZDjxRH+JqZwABAAAAiDANBgkqhkiG
# 9w0BAQUFADB5MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4G
# A1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSMw
# IQYDVQQDExpNaWNyb3NvZnQgQ29kZSBTaWduaW5nIFBDQTAeFw0xMjA3MjYyMDUw
# NDFaFw0xMzEwMjYyMDUwNDFaMIGDMQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2Fz
# aGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENv
# cnBvcmF0aW9uMQ0wCwYDVQQLEwRNT1BSMR4wHAYDVQQDExVNaWNyb3NvZnQgQ29y
# cG9yYXRpb24wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCzdHTQgjyH
# p5rUjrIEQoCXJS7kQc6TYzZfE/K0eJiAxih+zIoT7z03jDsJoNgUxVxe2KkdfwHB
# s5gbUHfs/up8Rc9/4SEOxYTKnw9rswk4t3TEVx6+8EioeVrfDpscmqi8yFK1DGmP
# hM5xVXv/CSC/QHc3ITB0W5Xfd8ug5cFyEgY98shVbK/B+2oWJ8j1s2Hj2c4bDx70
# 5M1MNGw+RxHnAitfFHoEB/XXPYvbZ31XPjXrbY0BQI0ah5biD3dMibo4nPuOApHb
# Ig/l0DapuDdF0Cr8lo3BYHEzpYix9sIEMIdbw9cvsnkR2ItlYqKKEWZdfn8FenOK
# H3qF5c0oENE9AgMBAAGjggEdMIIBGTATBgNVHSUEDDAKBggrBgEFBQcDAzAdBgNV
# HQ4EFgQUJls+W12WX+L3d4h/XkVTWKguW7gwDgYDVR0PAQH/BAQDAgeAMB8GA1Ud
# IwQYMBaAFMsR6MrStBZYAck3LjMWFrlMmgofMFYGA1UdHwRPME0wS6BJoEeGRWh0
# dHA6Ly9jcmwubWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3RzL01pY0NvZFNp
# Z1BDQV8wOC0zMS0yMDEwLmNybDBaBggrBgEFBQcBAQROMEwwSgYIKwYBBQUHMAKG
# Pmh0dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMvTWljQ29kU2lnUENB
# XzA4LTMxLTIwMTAuY3J0MA0GCSqGSIb3DQEBBQUAA4IBAQAP3kBJiJHRMTejRDhp
# smor1JH7aIWuWLseDI9W+pnXypcnTOiFjnlpLOS9lj/lcGaXlTBlKa3Gyqz1D3mo
# Z79p9A+X4woPv+6WdimyItAzxv+LSa2usv2/JervJ1DA6xn4GmRqoOEXWa/xz+yB
# qInosdIUBuNqbXRSZNqWlCpcaWsf7QWZGtzoZaqIGxWVGtOkUZb9VZX4Y42fFAyx
# nn9KBP/DZq0Kr66k3mP68OrDs7Lrh9vFOK22c9J4ZOrsIVtrO9ZEIvSBUqUrQymL
# DKEqcYJCy6sbftSlp6333vdGms5DOegqU+3PQOR3iEK/RxbgpTZq76cajTo9MwT2
# JSAjMIIEujCCA6KgAwIBAgIKYQKSSgAAAAAAIDANBgkqhkiG9w0BAQUFADB3MQsw
# CQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9u
# ZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSEwHwYDVQQDExhNaWNy
# b3NvZnQgVGltZS1TdGFtcCBQQ0EwHhcNMTIwMTA5MjIyNTU5WhcNMTMwNDA5MjIy
# NTU5WjCBszELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNV
# BAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjENMAsG
# A1UECxMETU9QUjEnMCUGA1UECxMebkNpcGhlciBEU0UgRVNOOkI4RUMtMzBBNC03
# MTQ0MSUwIwYDVQQDExxNaWNyb3NvZnQgVGltZS1TdGFtcCBTZXJ2aWNlMIIBIjAN
# BgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAzWPD96K1R9n5OZRTrGuPpnk4IfTR
# bj0VOBbBcyyZj/vgPFvhokyLsquLtPJKx7mTUNEm9YdTsHp180cPFytnLGTrYOdK
# jOCLXsRWaTc6KgRdFwHIv6m308mro5GogeM/LbfY5MR4AHk5z/3HZOIjEnieDHYn
# SY+arA504wZVVUnI7aF8cEVhfrJxFh7hwUG50tIy6VIk8zZQBNfdbzxJ1QvUdkD8
# ZWUTfpVROtX/uJqnV2tLFeU3WB/cAA3FrurfgUf58FKu5s9arOAUSqZxlID6/bAj
# MGDpg2CsDiQe/xHy56VVYpXun3+eKdbNSwp2g/BDBN8GSSDyU1pEsFF6OQIDAQAB
# o4IBCTCCAQUwHQYDVR0OBBYEFM0ZrGFNlGcr9q+UdVnb8FgAg6E6MB8GA1UdIwQY
# MBaAFCM0+NlSRnAK7UD7dvuzK7DDNbMPMFQGA1UdHwRNMEswSaBHoEWGQ2h0dHA6
# Ly9jcmwubWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3RzL01pY3Jvc29mdFRp
# bWVTdGFtcFBDQS5jcmwwWAYIKwYBBQUHAQEETDBKMEgGCCsGAQUFBzAChjxodHRw
# Oi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpL2NlcnRzL01pY3Jvc29mdFRpbWVTdGFt
# cFBDQS5jcnQwEwYDVR0lBAwwCgYIKwYBBQUHAwgwDQYJKoZIhvcNAQEFBQADggEB
# AFEc1t82HdyAvAKnxpnfFsiQBmkVmjK582QQ0orzYikbeY/KYKmzXcTkFi01jESb
# 8fRcYaRBrpqLulDRanlqs2KMnU1RUAupjtS/ohDAR9VOdVKJHj+Wao8uQBQGcu4/
# cFmSXYXtg5n6goSe5AMBIROrJ9bMcUnl2h3/bzwJTtWNZugMyX/uMRQCN197aeyJ
# PkV/JUTnHxrWxRrDSuTh8YSY50/5qZinGEbshGzsqQMK/Xx6Uh2ca6SoD5iSpJJ4
# XCt4432yx9m2cH3fW3NTv6rUZlBL8Mk7lYXlwUplnSVYULsgVJF5OhsHXGpXKK8x
# x5/nwx3uR/0n13/PdNxlxT8wggW8MIIDpKADAgECAgphMyYaAAAAAAAxMA0GCSqG
# SIb3DQEBBQUAMF8xEzARBgoJkiaJk/IsZAEZFgNjb20xGTAXBgoJkiaJk/IsZAEZ
# FgltaWNyb3NvZnQxLTArBgNVBAMTJE1pY3Jvc29mdCBSb290IENlcnRpZmljYXRl
# IEF1dGhvcml0eTAeFw0xMDA4MzEyMjE5MzJaFw0yMDA4MzEyMjI5MzJaMHkxCzAJ
# BgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25k
# MR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xIzAhBgNVBAMTGk1pY3Jv
# c29mdCBDb2RlIFNpZ25pbmcgUENBMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIB
# CgKCAQEAsnJZXBkwZL8dmmAgIEKZdlNsPhvWb8zL8epr/pcWEODfOnSDGrcvoDLs
# /97CQk4j1XIA2zVXConKriBJ9PBorE1LjaW9eUtxm0cH2v0l3511iM+qc0R/14Hb
# 873yNqTJXEXcr6094CholxqnpXJzVvEXlOT9NZRyoNZ2Xx53RYOFOBbQc1sFumdS
# jaWyaS/aGQv+knQp4nYvVN0UMFn40o1i/cvJX0YxULknE+RAMM9yKRAoIsc3Tj2g
# Mj2QzaE4BoVcTlaCKCoFMrdL109j59ItYvFFPeesCAD2RqGe0VuMJlPoeqpK8kbP
# Nzw4nrR3XKUXno3LEY9WPMGsCV8D0wIDAQABo4IBXjCCAVowDwYDVR0TAQH/BAUw
# AwEB/zAdBgNVHQ4EFgQUyxHoytK0FlgByTcuMxYWuUyaCh8wCwYDVR0PBAQDAgGG
# MBIGCSsGAQQBgjcVAQQFAgMBAAEwIwYJKwYBBAGCNxUCBBYEFP3RMU7TJoqV4Zhg
# O6gxb6Y8vNgtMBkGCSsGAQQBgjcUAgQMHgoAUwB1AGIAQwBBMB8GA1UdIwQYMBaA
# FA6sgmBAVieX5SUT/CrhClOVWeSkMFAGA1UdHwRJMEcwRaBDoEGGP2h0dHA6Ly9j
# cmwubWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3RzL21pY3Jvc29mdHJvb3Rj
# ZXJ0LmNybDBUBggrBgEFBQcBAQRIMEYwRAYIKwYBBQUHMAKGOGh0dHA6Ly93d3cu
# bWljcm9zb2Z0LmNvbS9wa2kvY2VydHMvTWljcm9zb2Z0Um9vdENlcnQuY3J0MA0G
# CSqGSIb3DQEBBQUAA4ICAQBZOT5/Jkav629AsTK1ausOL26oSffrX3XtTDst10Ot
# C/7L6S0xoyPMfFCYgCFdrD0vTLqiqFac43C7uLT4ebVJcvc+6kF/yuEMF2nLpZwg
# LfoLUMRWzS3jStK8cOeoDaIDpVbguIpLV/KVQpzx8+/u44YfNDy4VprwUyOFKqSC
# HJPilAcd8uJO+IyhyugTpZFOyBvSj3KVKnFtmxr4HPBT1mfMIv9cHc2ijL0nsnlj
# VkSiUc356aNYVt2bAkVEL1/02q7UgjJu/KSVE+Traeepoiy+yCsQDmWOmdv1ovoS
# JgllOJTxeh9Ku9HhVujQeJYYXMk1Fl/dkx1Jji2+rTREHO4QFRoAXd01WyHOmMcJ
# 7oUOjE9tDhNOPXwpSJxy0fNsysHscKNXkld9lI2gG0gDWvfPo2cKdKU27S0vF8jm
# cjcS9G+xPGeC+VKyjTMWZR4Oit0Q3mT0b85G1NMX6XnEBLTT+yzfH4qerAr7EydA
# reT54al/RrsHYEdlYEBOsELsTu2zdnnYCjQJbRyAMR/iDlTd5aH75UcQrWSY/1AW
# Lny/BSF64pVBJ2nDk4+VyY3YmyGuDVyc8KKuhmiDDGotu3ZrAB2WrfIWe/YWgyS5
# iM9qqEcxL5rc43E91wB+YkfRzojJuBj6DnKNwaM9rwJAav9pm5biEKgQtDdQCNbD
# PTCCBgcwggPvoAMCAQICCmEWaDQAAAAAABwwDQYJKoZIhvcNAQEFBQAwXzETMBEG
# CgmSJomT8ixkARkWA2NvbTEZMBcGCgmSJomT8ixkARkWCW1pY3Jvc29mdDEtMCsG
# A1UEAxMkTWljcm9zb2Z0IFJvb3QgQ2VydGlmaWNhdGUgQXV0aG9yaXR5MB4XDTA3
# MDQwMzEyNTMwOVoXDTIxMDQwMzEzMDMwOVowdzELMAkGA1UEBhMCVVMxEzARBgNV
# BAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jv
# c29mdCBDb3Jwb3JhdGlvbjEhMB8GA1UEAxMYTWljcm9zb2Z0IFRpbWUtU3RhbXAg
# UENBMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAn6Fssd/bSJIqfGsu
# GeG94uPFmVEjUK3O3RhOJA/u0afRTK10MCAR6wfVVJUVSZQbQpKumFwwJtoAa+h7
# veyJBw/3DgSY8InMH8szJIed8vRnHCz8e+eIHernTqOhwSNTyo36Rc8J0F6v0LBC
# BKL5pmyTZ9co3EZTsIbQ5ShGLieshk9VUgzkAyz7apCQMG6H81kwnfp+1pez6CGX
# fvjSE/MIt1NtUrRFkJ9IAEpHZhEnKWaol+TTBoFKovmEpxFHFAmCn4TtVXj+AZod
# UAiFABAwRu233iNGu8QtVJ+vHnhBMXfMm987g5OhYQK1HQ2x/PebsgHOIktU//kF
# w8IgCwIDAQABo4IBqzCCAacwDwYDVR0TAQH/BAUwAwEB/zAdBgNVHQ4EFgQUIzT4
# 2VJGcArtQPt2+7MrsMM1sw8wCwYDVR0PBAQDAgGGMBAGCSsGAQQBgjcVAQQDAgEA
# MIGYBgNVHSMEgZAwgY2AFA6sgmBAVieX5SUT/CrhClOVWeSkoWOkYTBfMRMwEQYK
# CZImiZPyLGQBGRYDY29tMRkwFwYKCZImiZPyLGQBGRYJbWljcm9zb2Z0MS0wKwYD
# VQQDEyRNaWNyb3NvZnQgUm9vdCBDZXJ0aWZpY2F0ZSBBdXRob3JpdHmCEHmtFqFK
# oKWtTHNY9AcTLmUwUAYDVR0fBEkwRzBFoEOgQYY/aHR0cDovL2NybC5taWNyb3Nv
# ZnQuY29tL3BraS9jcmwvcHJvZHVjdHMvbWljcm9zb2Z0cm9vdGNlcnQuY3JsMFQG
# CCsGAQUFBwEBBEgwRjBEBggrBgEFBQcwAoY4aHR0cDovL3d3dy5taWNyb3NvZnQu
# Y29tL3BraS9jZXJ0cy9NaWNyb3NvZnRSb290Q2VydC5jcnQwEwYDVR0lBAwwCgYI
# KwYBBQUHAwgwDQYJKoZIhvcNAQEFBQADggIBABCXisNcA0Q23em0rXfbznlRTQGx
# LnRxW20ME6vOvnuPuC7UEqKMbWK4VwLLTiATUJndekDiV7uvWJoc4R0Bhqy7ePKL
# 0Ow7Ae7ivo8KBciNSOLwUxXdT6uS5OeNatWAweaU8gYvhQPpkSokInD79vzkeJku
# DfcH4nC8GE6djmsKcpW4oTmcZy3FUQ7qYlw/FpiLID/iBxoy+cwxSnYxPStyC8jq
# cD3/hQoT38IKYY7w17gX606Lf8U1K16jv+u8fQtCe9RTciHuMMq7eGVcWwEXChQO
# 0toUmPU8uWZYsy0v5/mFhsxRVuidcJRsrDlM1PZ5v6oYemIp76KbKTQGdxpiyT0e
# bR+C8AvHLLvPQ7Pl+ex9teOkqHQ1uE7FcSMSJnYLPFKMcVpGQxS8s7OwTWfIn0L/
# gHkhgJ4VMGboQhJeGsieIiHQQ+kr6bv0SMws1NgygEwmKkgkX1rqVu+m3pmdyjpv
# vYEndAYR7nYhv5uCwSdUtrFqPYmhdmG0bqETpr+qR/ASb/2KMmyy/t9RyIwjyWa9
# nR2HEmQCPS2vWY+45CHltbDKY7R4VAXUQS5QrJSwpXirs6CWdRrZkocTdSIvMqgI
# bqBbjCW/oO+EyiHW6x5PyZruSeD3AWVviQt9yGnI5m7qp5fOMSn/DsVbXNhNG6HY
# +i+ePy5VFmvJE6P9MYIEgjCCBH4CAQEwgZAweTELMAkGA1UEBhMCVVMxEzARBgNV
# BAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jv
# c29mdCBDb3Jwb3JhdGlvbjEjMCEGA1UEAxMaTWljcm9zb2Z0IENvZGUgU2lnbmlu
# ZyBQQ0ECEzMAAACIWQ48UR/iamcAAQAAAIgwCQYFKw4DAhoFAKCBpDAZBgkqhkiG
# 9w0BCQMxDAYKKwYBBAGCNwIBBDAcBgorBgEEAYI3AgELMQ4wDAYKKwYBBAGCNwIB
# FTAjBgkqhkiG9w0BCQQxFgQUFheGCfrhKq7eo4Y6FFgNXCSkExIwRAYKKwYBBAGC
# NwIBDDE2MDSgFoAUAEMAbwBtAG0AbwBuACAAUABTADGhGoAYaHR0cDovL3d3dy5t
# aWNyb3NvZnQuY29tMA0GCSqGSIb3DQEBAQUABIIBAJQa+7M612EnfsUvB3OoG0Eh
# stWbD3ORhFhgIg5BAECyJ+Y5opDaHBa8tAHTkpe6tzryS6olpciUCM8eaHK6n5Va
# BwgEWwcNoO23dPtHG1rbD0QpiGk4AmD0+CMasD0DvYA2bPV4VMKXFiIMYzSeAiIn
# BwqbNIf/KjExaGycTstJEz4DlGnpJIgsMEapGFSorut7n19LGVDqNqXqcDR5htde
# LubLmwZyQdir2bmNBoGLe0EV3fWNDYMk++UqWWd7XBeHnB7kxCY7WepUF0cHXEjc
# aPL5y2jeGO/d3aYa1E4uJaCF+xCwNghn9DwttSC/PyzVsDLJrylpc0aORx7RwvWh
# ggIfMIICGwYJKoZIhvcNAQkGMYICDDCCAggCAQEwgYUwdzELMAkGA1UEBhMCVVMx
# EzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoT
# FU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEhMB8GA1UEAxMYTWljcm9zb2Z0IFRpbWUt
# U3RhbXAgUENBAgphApJKAAAAAAAgMAkGBSsOAwIaBQCgXTAYBgkqhkiG9w0BCQMx
# CwYJKoZIhvcNAQcBMBwGCSqGSIb3DQEJBTEPFw0xMjEwMDUxOTM0NTBaMCMGCSqG
# SIb3DQEJBDEWBBRDCuuS9WAFOxjYPragaXHf84KOkDANBgkqhkiG9w0BAQUFAASC
# AQA1tlTPdzNWEZzZGiITTnwe56EqlJ6FoVzansG4bDwREdvXss6oAhFxIU1LAaHn
# 4B3T2h2zFDgqALXq0Q8S1fhCmPDoARxHaM9unBgaI2j0NFVSdqmC2Iar7k5K4hLK
# rFbW774B3LQgZO8Z6dh9SkJWr1JQbLO7WM9r4SaS6B31cA1KF3oVEKY8FLr4sVVF
# EwuPCK4z2oKlYRRvEefqL5KBORSdjBfn+aP+piw60kIVmxIO2hfmvLGlrV1OLue4
# I2tMIO9k6hLOfi2Ja7k4dyNtRsMUyKSw0AYr/LO0b5krZTwC/i+tzjVX4xfCuTiN
# ioAuG58MJwyVWTrOTOvA74y1
# SIG # End signature block
