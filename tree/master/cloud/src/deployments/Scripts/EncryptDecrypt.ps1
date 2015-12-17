param            
(                  
    [parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [bool] $ToEncrypt

   ,[parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [String] $ErrorLogPath
)

Function Encrypt()
{
    for($i = 0; $i -lt $OfficeSolutionList.length; $i++) 
    {
        $configFilePath = Join-Path $ParentDirectory $OfficeSolutionList[$i]
        if(Test-Path($configFilePath)) #Check for existence of folder
        {
            [string]$msg = & aspnet_regiis.exe -pef "appSettings"  $configFilePath #Call to encrypt the config files
            if($msg.Contains("Succeeded"))
            {
				Show-Message -Message "Encryption successful" -Type ([MessageType]::Success)
            }
            else
            {
				Show-Message -Message "Encryption failed" -Type ([MessageType]::Failure)
                Write-Log -ErrorMessage:$msg -ErrorLogFilePath:$ErrorLogPath
            }
        }
    }
}

Function Decrypt()
{
    for($i = 0; $i -lt $OfficeSolutionList.length; $i++) 
    {
        $configFilePath = Join-Path $ParentDirectory $OfficeSolutionList[$i]
        if(Test-Path($configFilePath)) #Check for existence of folder
        {
            [string]$msg = & aspnet_regiis.exe -pdf "appSettings"  $configFilePath #Call to decrypt the config files
            if($msg.Contains("Succeeded"))
            {
				Show-Message -Message "Decryption successful" -Type ([MessageType]::Success)
            }
            else
            {
				Show-Message -Message "Decryption failed. See ErrorLog for details" -Type ([MessageType]::Failure)
                Write-Log -ErrorMessage:$msg -ErrorLogFilePath:$ErrorLogPath
            }
        }
    }
}


# Includes all the solution folder names which contains the config files
$OfficeSolutionList = @(
                        "Service Publish"                       
                       ,"Web Publish"
                      )

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

$ExcelValues = Read-FromExcel $ExcelFilePath "Config" ("IsDeployedOnAzure") "$ErrorLogFile"
if($ExcelValues.length -le 0)
{
    Write-Log $ErrorLogFile "Error reading values from Excel file. Aborting!"
    return $false
}

if("false" -eq $ExcelValues.ToLower())
{
    if($ToEncrypt)
    {
        Encrypt
    }
    else
    {
        Decrypt
    }
}
else
{
	Show-Message -Message "Skipping Encryption for Online Deployment..." -Type ([MessageType]::Success)
}