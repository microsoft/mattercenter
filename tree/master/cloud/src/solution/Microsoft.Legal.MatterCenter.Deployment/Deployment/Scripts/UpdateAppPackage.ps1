# Script to update the App package with configurations
param
(    
    [parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [bool] $IsDeployedOnAzure,
                  
    [parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [System.Management.Automation.PSCredential] $Credentials
)

# Function to get the parent Content Type ID
function GetContentTypeID(){
    param
    (
        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [bool] $IsDeployedOnAzure,
                  
        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [System.Management.Automation.PSCredential] $Credentials

    )
	Show-Message -Message "Getting parent Content Type ID" -Type ([MessageType]::Success)
    $contentTypeName = "MatterCenter"
    $contentTypeGroup = "_MatterCenter"
    try {
        $clientContext = New-Object Microsoft.SharePoint.Client.ClientContext($ContentTypeHubURL)
        if($IsDeployedOnAzure){
            $credential = New-Object Microsoft.SharePoint.Client.SharePointOnlineCredentials($Credentials.UserName, $Credentials.Password)
        } else {
            $credential = New-Object System.Net.NetworkCredential($Credentials.UserName, $Credentials.Password)
        }
        $clientContext.Credentials = $credential
    
        # Retrieve the ID for parent Content Type
        $web = $clientContext.Web
        $contentTypeCollection = $web.ContentTypes
        $clientContext.Load($contentTypeCollection)
        $clientContext.ExecuteQuery()
        $parentContentType = $contentTypeCollection | Where-Object {$_.Group -eq $contentTypeGroup -and $_.Name -eq $contentTypeName}
        $parentContentTypeId = $parentContentType.Id.StringValue
		Show-Message -Message "Parent Content Type ID retrieved" -Type ([MessageType]::Success)
    } catch [Exception]
    {
        $parentContentTypeId = ""
		Show-Message -Message "Failed to get the parent Content Type ID" -Type ([MessageType]::Failure)
        Write-Log $ErrorLogFile $_.Exception.ToString()
    }
    return $parentContentTypeId
}

# Function to update AppManifest.xml, Elements.xml and Feature.xml files present inside the App package
function UpdateAppFile(){
    param
    (    
        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [string] $appPackagePath,

        [parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()]             
        [string] $parentContentTypeId
    )

    $fileName = Split-Path -Path $appPackagePath -Leaf
    $appManifestFile = "/AppManifest.xml"
    $elementsString = "/elements"
    $featureString = "/feature"
    $pages = "/pages/"
    $sendToOneDrive = "Send To OneDrive"
    $newProdID = [guid]::NewGuid().ToString()
    $newFeatureID = [guid]::NewGuid().ToString()

	try
	{
		$package = [System.IO.Packaging.Package]::Open($appPackagePath, [System.IO.FileMode]::Open)
		$manifestUri = New-Object System.Uri($appManifestFile, [System.UriKind]::Relative)
		$appManifestPartNameUri = [System.IO.Packaging.PackUriHelper]::CreatePartUri($manifestUri)
		$appManifestPart = $package.GetPart($appManifestPartNameUri)
		$appManifestPartStream = $appManifestPart.GetStream()
		$reader = New-Object -Type System.IO.StreamReader -ArgumentList $appManifestPartStream
		$content = $reader.ReadToEnd()
		$xml = [xml] $content
		if($xml.App.ProductID)
		{
			$xml.App.ProductID = "{" + $newProdID + "}"
		}
		if($xml.App.Properties.StartPage -and -1 -lt $xml.App.Properties.StartPage.ToLowerInvariant().IndexOf($pages))
		{
			$previousURL = $xml.App.Properties.StartPage
			$xml.App.Properties.StartPage = $SiteUrl + $previousURL.Substring($previousURL.ToLowerInvariant().IndexOf($pages))
		}
		if($xml.App.AppPrincipal.RemoteWebApplication.ClientId)
		{
			$xml.App.AppPrincipal.RemoteWebApplication.ClientId = $ClientID
		}
 
		$appManifestPartStream.Position = 0
		$appManifestPartStream.SetLength(0)
		$writer = New-Object -TypeName System.IO.StreamWriter -ArgumentList $appManifestPartStream
		$writer.Write($xml.InnerXml)
		$writer.Flush()

		$parts = $package.GetParts()
		foreach ($part in $parts){
			$partNameUri = $part.Uri.OriginalString
			if($partNameUri -match $elementsString -or $partNameUri -match $featureString){
				$partStream = $part.GetStream()
				$partReader = New-Object -Type System.IO.StreamReader -ArgumentList $partStream
				$partContent = $partReader.ReadToEnd()
				$partXml = [xml] $partContent
				if($partNameUri -match $elementsString){
					# Update Elements.xml for SharePoint App
					if($partXml.Elements.CustomAction.UrlAction.Url -and -1 -lt $partXml.Elements.CustomAction.UrlAction.Url.ToLowerInvariant().IndexOf($pages)){
						$previousURL = $partXml.Elements.CustomAction.UrlAction.Url
						$partXml.Elements.CustomAction.UrlAction.Url = $SiteUrl + $previousURL.Substring($previousURL.ToLowerInvariant().IndexOf($pages))
					}
					if($sendToOneDrive -eq $partXml.Elements.CustomAction.Title -and "" -ne $parentContentTypeId){
						$partXml.Elements.CustomAction.RegistrationId = $parentContentTypeId
					}
					# Update Elements.xml for OneDrive Ribbon App
					if($partXml.Elements.CustomAction.CommandUIExtension.CommandUIHandlers.CommandUIHandler.CommandAction -and 0 -lt $partXml.Elements.CustomAction.CommandUIExtension.CommandUIHandlers.CommandUIHandler.CommandAction.Count){
						foreach ($item in $partXml.Elements.CustomAction.CommandUIExtension.CommandUIHandlers.ChildNodes){
							$previousURL = $item.CommandAction
							$item.CommandAction = $SiteUrl + $previousURL.Substring($previousURL.ToLowerInvariant().IndexOf($pages))
						}
					}
				} elseif($partNameUri -match $featureString){
                    # Update Feature.xml for the App
					if($partXml.Feature.Id){
						$partXml.Feature.Id = "{" + $newFeatureID.ToLowerInvariant() + "}"
					}
				}
				$partStream.Position = 0
				$partStream.SetLength(0)
				$partWriter = New-Object -TypeName System.IO.StreamWriter -ArgumentList $partStream
				$partWriter.Write($partXml.InnerXml)
				$partWriter.Flush()
			}
		}
		$package.Close()
		Show-Message -Message "Updated the $fileName package" -Type ([MessageType]::Success)
	} catch [Exception]
    {
        $parentContentTypeId = ""
		Show-Message -Message "Failed to update the $fileName package" -Type ([MessageType]::Failure)
        Write-Log $ErrorLogFile $_.Exception.ToString()
    }
}

$Configurations = (Read-FromExcel $ExcelFilePath "Config" ("ContentTypeHubURL", "UISiteURL", "ClientID") $ErrorLogFile)
$Configurations = $Configurations.Split(";")
if (3 -eq $Configurations.Count){
    $ContentTypeHubURL = $Configurations[0]
    $SiteUrl = $Configurations[1]
    $ClientID = $Configurations[2]

    $UtilityFolder = "\Helper Utilities\"
    $SharePointAppFolder = "\SharePoint App\"

    Add-Type -Path (Get-ChildItem ((Get-Item $ScriptDirectory).FullName + $UtilityFolder + "WindowsBase.dll"))
    $SharePointAppPath = (Get-Item $ScriptDirectory).Parent.FullName + $SharePointAppFolder + "Microsoft.Legal.MatterCenter.app"
    $RibbonAppPath = (Get-Item $ScriptDirectory).Parent.FullName + $SharePointAppFolder + "Microsoft.Legal.MatterCenter.OneDriveRibbon.app"
	# Get the parent Content Type ID
    $ParentContentTypeId = GetContentTypeID -IsDeployedOnAzure $IsDeployedOnAzure -Credentials $Credentials

    # Update SharePoint App
    UpdateAppFile -appPackagePath $SharePointAppPath -parentContentTypeId $ParentContentTypeId
    # Update OneDrive Ribbon App
    UpdateAppFile -appPackagePath $RibbonAppPath -parentContentTypeId $ParentContentTypeId
} else{
    $ErrorMessage = "Incorrect configuration retrieved while updating App packages"
	Show-Message -Message $ErrorMessage -Type ([MessageType]::Failure)
    Write-Log $ErrorMessage
}