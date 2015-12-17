param            
(                  
    [parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [bool] $IsDeploy

    ,[parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [bool] $IsOfficeApp
)

# Function is used to remove app from specified URL
function removeAppFromOffice()
{
param            
    (                  
        [parameter(Mandatory=$true)]                         
        [ValidateNotNullOrEmpty()]
        [String] $webUrl,

        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $DocLibName,

        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $FolderPath,

        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $FileName

    )

    try
    {
        $context = New-Object Microsoft.SharePoint.Client.ClientContext($webUrl)
        
        if($credentials -eq $null)
        {
            $credentials = Get-Credential
        }

        if($IsDeployedOnAzure)
        {
            $onlinecredential = New-Object Microsoft.SharePoint.Client.SharePointOnlineCredentials($credentials.UserName,$credentials.Password)
        }
        else
        {
            $onlinecredential = New-Object System.Net.NetworkCredential($credentials.UserName,$credentials.Password) 
        }

        $context.Credentials = $onlinecredential 
        $List = $context.Web.Lists.GetByTitle($DocLibName)
        $context.Load($List)
        $context.Load($List.RootFolder)
        $context.Load($List.RootFolder.Files)
        $context.ExecuteQuery()
        
        foreach( $Folder in $List.RootFolder)
        {
            foreach ($files in $Folder.Files) 
            {
                if($FileName -Eq $files.name)
                {
                    $FileToRemove = $files
                }
            }
            
            try
            {
                $FileToRemove.DeleteObject()
                $context.ExecuteQuery()
                Write-Host -f Green $FileToRemove.name " has been deleted"
            }
            catch [Exception]
            {
		        write-host -f red "An Error Occurred while trying to remove the Apps"                
                Write-Log $ErrorLogFile $_.Exception.ToString()
            }
        }
    }
    catch [Exception]
    {
		write-host -f red "Failed to access the app library or context"
        Write-Log $ErrorLogFile $_.Exception.ToString()
    }
    finally {
        $context.Dispose()
    }
}


# Function is used to loop through all app files and calls remove Office App function to remove individual apps
function removeAppsFromOffice()
{
param            
    (                  
        [parameter(Mandatory=$true)]                         
        [ValidateNotNullOrEmpty()]
        [String] $webUrl,

        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $DocLibName,

        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $FolderPath

    )

    #Remove Apps from App Catalog
    Foreach ($File in (dir $FolderPath))
    {
        removeAppFromOffice -webUrl $webUrl -DocLibName $DocLibName -FolderPath $FolderPath -FileName $File.Name        
    }
}

# function is used to add apps to the specified URL
function addAppsToOffice()
{
param            
    (                  
        [parameter(Mandatory=$true)]                         
        [ValidateNotNullOrEmpty()]
        [String] $webUrl,

        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $DocLibName,

        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [String] $FolderPath

    )

    try
    {
        write-host -f yellow "Fetching Client Context"
        $context = New-Object Microsoft.SharePoint.Client.ClientContext($webUrl)

        if($credentials -eq $null)
        {
            $credentials = Get-Credential
        }
    
        if($IsDeployedOnAzure)
        {
            $onlinecredential = New-Object Microsoft.SharePoint.Client.SharePointOnlineCredentials($credentials.UserName,$credentials.Password)
        }
        else
        {
            $onlinecredential = New-Object System.Net.NetworkCredential($credentials.UserName,$credentials.Password) 
        }
    
        $context.Credentials = $onlinecredential
    
        write-host -f yellow "Fetching the Library context"
        $List = $context.Web.Lists.GetByTitle($DocLibName)
    
        $context.Load($List)
        $context.ExecuteQuery()
        write-host -f green "Successfully retrieved the Library context"
        try
        {
            Foreach ($File in (dir $FolderPath))
            {
                $FileStream = New-Object IO.FileStream($File.FullName, [System.IO.FileMode]::Open)
                $FileCreationInfo = New-Object Microsoft.SharePoint.Client.FileCreationInformation
                $FileCreationInfo.Overwrite = $true
                $FileCreationInfo.ContentStream = $FileStream
                $FileCreationInfo.URL = $File
                $Upload = $List.RootFolder.Files.Add($FileCreationInfo)
                $Context.Load($Upload)
                $Context.ExecuteQuery()
                $fileName = split-path $File -Leaf
                Write-Host -f Green $fileName " has been successfully added"
                $FileStream.Dispose()
            }
        }
        catch [Exception]
        {
            write-host -f red "Failed to add an app"
            Write-Log $ErrorLogFile $_.Exception.ToString()
            #revert all changes
            Foreach ($File in (dir $FolderPath))
            {
                removeAppFromOffice -webUrl $webUrl -DocLibName $DocLibName -FolderPath $FolderPath -FileName $File.Name        
            }
        }
    }
    catch [Exception]
    {
		write-host -f red "Failed to access the app library or context"
        Write-Log $ErrorLogFile $_.Exception.ToString()
    }
    finally {
        $context.Dispose()
    }
}

# Get the current directory of the script
Function ScriptRoot {Split-Path $MyInvocation.ScriptName}
$ScriptDirectory = (ScriptRoot)

# Get the parent directory of the script
Function Get-ParentDirectory {Split-Path -Parent(Split-Path $MyInvocation.ScriptName)}
$ParentDirectory = (Get-ParentDirectory)

#Set Excel file path, uncomment below line if you want to use this script seperately
#$ExcelFilePath = "$ParentDirectory\MCDeploymentConfig.xlsx"
    
# Set log file path, uncomment below line if you want to use this script seperately
#$ErrorLogFile = "$ScriptDirectory\ErrorLog.txt"

if($IsOfficeApp)
{
    #App Catalog URL for Office Apps
    $DocLibName = "Apps for Office"

    #Fetch the Folder containing the XML files for Office Apps
    $FolderPath = Join-Path $ParentDirectory "Office App"
}
else
{
    #App Catalog URL for SharePoint Apps
    $DocLibName = "Apps for SharePoint"

    #Fetch the Folder containing the APP files for SharePoint Apps
    $FolderPath = Join-Path $ParentDirectory "SharePoint App"
}

#Fetch the Catalog URL from the Excel File
write-host -f yellow "Fetching the Catalog URL from the Configuration File"
$ExcelValues = Read-FromExcel $ExcelFilePath "Config" ("CatalogSiteURL", "IsDeployedOnAzure") $ErrorLogFile
$ExcelValues = $ExcelValues.Split(";")
if($ExcelValues.length -le 0)
{
    Write-Log $ErrorLogFile "Error reading values from Excel file. Aborting!"
    return $false
}
$webUrl = $ExcelValues[0]
# Set parameter based on value read from Excel
[bool]$IsDeployedOnAzure = if("true" -eq $ExcelValues[1].ToLower()){$true} else {$false}
if($webUrl -ne $null)
{
    write-host -f Green "Successfully retrieved the Catalog URL"
}

# Check whether app is to be added or removed
if ($IsDeploy)
{
    write-host -f yellow "Proceeding to add the apps to the App Catalog"
    #Add Apps to the App Catalog
    addAppsToOffice -webUrl $webUrl -DocLibName $DocLibName -FolderPath $FolderPath
}
else
{
    write-host -f yellow "Proceeding to remove the apps from the App Catalog"
    #Remove all apps from App Catalog
    removeAppsFromOffice -webUrl $webUrl -DocLibName $DocLibName -FolderPath $FolderPath
}