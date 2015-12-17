# This script is used to publish UI and Service to IIS

# Get the current directory of the script
Function ScriptRoot {Split-Path $MyInvocation.ScriptName}
$ScriptDirectory = (ScriptRoot)

# Get the parent directory of the script
Function Get-ParentDirectory {Split-Path -Parent(Split-Path $MyInvocation.ScriptName)}
$ParentDirectory = (Get-ParentDirectory)

# Function is used to publish files to IIS
Function Publish-Files()
{
    param(
        [parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string] $SourcePath

       ,[parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string] $DestinationPath
        )

        Show-Message -Message "Copying all files from $SourcePath to $DestinationPath"

		# Copy files after checking whether source and destination folders exist
        if((Check-Existance -Path $SourcePath) -and (Check-Existance -Path $DestinationPath))
        {            
            Get-Childitem "$SourcePath" | % { 
            copy-item $_.FullName -Destination "$DestinationPath\$_" -Recurse -Force
            }

            return $true;
        }
        else
        {
            Write-Log ($ErrorLogFile) "Either the Source Path or the Destination Path is invalid"
            return $false;
        }

}

# Function is used to check whether provided path exists or not
Function Check-Existance()
{
    param(
        [parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string] $Path
        )

    return Test-Path $path
}

#Set Excel file path, uncomment below line if you want to use this script separately
#$ExcelFilePath = "$ParentDirectory\MCDeploymentConfig.xlsx"
    
# Set log file path, uncomment below line if you want to use this script separately
#$ErrorLogFile = "$ScriptDirectory\Logs\ErrorLog.txt"

Show-Message -Message "Reading parameters from Excel file..."    
$ExcelValues = (Read-FromExcel $ExcelFilePath "Config" ("UIPublishLocation", "ServicePublishLocation", "IsDeployedOnAzure") ($ErrorLogFile))
$ExcelValues = $ExcelValues.Split(";")
if($ExcelValues.length -le 0)
{
    Write-Log $ErrorLogFile "Error reading values from Excel file. Aborting!"
    return $false
}
$UIPublishLocation = $ExcelValues[0]   
$ServicePublishLocation = $ExcelValues[1]  
$IsValid = $false

# If IsDeployedOnAzure parameter is false then proceed with publish activity else do nothing

if ("false" -eq $ExcelValues[2].ToLowerInvariant()) 
{    
    Show-Message -Message "Publishing files to IIS"
    $IsValid = Publish-Files -SourcePath ($ParentDirectory + "\Service Publish") -DestinationPath $ServicePublishLocation
    if(!($IsValid))
    {
        Write-Log ($ErrorLogFile) "Publish to $ServicePublishLocation failed" 
        return
    }
    $IsValid = Publish-Files -SourcePath ($ParentDirectory + "\Web Publish") -DestinationPath $UIPublishLocation
    if(!($IsValid))
    {
        Write-Log ($ErrorLogFile) "Publish to $UIPublishLocation failed" 
        return
    }   
}
else
{
	Show-Message -Message "Skipping publish step as the script is ran for Azure" -Type ([MessageType]::Warning) 
}


