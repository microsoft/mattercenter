param            
(                  
    [parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [bool] $IsDeployedOnAzure

    ,[parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [string] $Username

    ,[parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [string] $Password
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

# Set the helper files folder path, uncomment below line if you want to use this script separately
#$HelperPath = "$ScriptDirectory\Helper Utilities"

$IsValid = Test-Path $ExcelFilePath

if($IsValid -eq $true){
    if($IsDeployedOnAzure)
    {
        Show-Message -Message "Provisioning Site Collection"
        & "$HelperPath\Microsoft.Legal.MatterCenter.CreateSiteCollection.exe" "true" $Username $Password
    }
    else
    {
        $sheet = (ReadSheet-FromExcel $ExcelFilePath "Client_Config" $ErrorLogFile)
        For($count = 2; $count -le $sheet.Length; $count++)
        {
            $siteUrl = $sheet[$count-1][3]
            $exists = (Get-SPSite $siteUrl -ErrorAction SilentlyContinue) -ne $null
            if(-not $exists)
            {
                $template = Get-SPWebTemplate "STS#0"
                New-SPSite $siteUrl -OwnerAlias $Username -CompatibilityLevel 15 -Name $sheet[$count-1][1]  -Template $template
            }
            else
            {
				Show-Message -Message "$siteUrl already exists..." -Type ( [MessageType]::Warning )
            }
        }
    }
}