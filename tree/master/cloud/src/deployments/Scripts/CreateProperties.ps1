# Function for managed properties creation
#xmlFile: the location of XML.
Function ProcessManagedPropertyXml
{
	Param(
		[Parameter(Mandatory=$true)]
		[string]$xmlFile
	)
	#Import SharePoint management shell
	$snapin = Get-PSSnapin | Where-Object { $_.Name -eq "Microsoft.SharePoint.Powershell" }
	if ($snapin -eq $null) {
	    Show-Message -Message "Loading SharePoint PowerShell Snapin"
	    Add-PSSnapin "Microsoft.SharePoint.Powershell"
	} 

	$xmlContent = [xml](Get-Content $xmlFile)
	if($xmlContent.DocumentElement.HasAttribute("ServiceApplication"))
	{
		$serviceApplicationName=$xmlContent.DocumentElement.GetAttribute("ServiceApplication")
		Show-Message -Message "ServiceApplication: $serviceApplicationName"
		$serviceApplication = Get-SPEnterpriseSearchServiceApplication -Identity $serviceApplicationName

		foreach($metadatamapping in $xmlContent.MappingRoot.ManagedProperties.ManagedProperty)
		{
			$managedPropertyName=$metadatamapping.GetAttribute("Name")
			$managedPropertyType=[int]$metadatamapping.GetAttribute("Type")
            $managedPropertyAlias=[string]$metadatamapping.GetAttribute("Alias")
			Show-Message -Message "Process Managed Property Name: $managedPropertyName; ..."
			
			if($metadatamapping.CrawlProperties.CrawlProperty -ne $null)
			{
				#Get ManagedPropertyObject
				$managedPropertyObj = Get-SPEnterpriseSearchMetadataManagedProperty -SearchApplication $serviceApplication -Identity $managedPropertyName -ErrorAction SilentlyContinue
				#if Managed Property doesn't exist, create it.
				if($managedPropertyObj -eq $null)
				{
					$managedPropertyObj=New-SPEnterpriseSearchMetadataManagedProperty -SearchApplication $serviceApplication -Name $managedPropertyName -Type $managedPropertyType -Queryable $true
					Show-Message -Message "Managed Property doesn't exist and created. Managed Property Name: $managedPropertyName" 
				}
                if("" -ne $managedPropertyAlias -and -not $managedPropertyObj.ContainsAlias($managedPropertyAlias))
                {
                    $managedPropertyObj.AddAlias($managedPropertyAlias);
                }
                
				#create mapping.
				foreach($crawlMapping in $metadatamapping.CrawlProperties.CrawlProperty)
				{
					$crawlPropertyName = $crawlMapping.GetAttribute("Name");
					$crawlPropertyCategory = $crawlMapping.GetAttribute("Category");

					#There are a few managed properties that we want to set that don't tie to crawled properties.  'NA' denotes those.
					if($crawlPropertyName -ne "NA") {
						#Get the crawl property.
						$crawlPropertyObjs = Get-SPEnterpriseSearchMetadataCrawledProperty -SearchApplication $serviceApplication -Name $crawlPropertyName -Category $crawlPropertyCategory
					
						if($crawlPropertyObjs.Count -gt 0) {
							Show-Message -Message "Crawl Property found. Crawl Property: $crawlPropertyName"
						}
						else {
							Show-Message -Message "Crawl Property doesn't exist. Creating Crawled Property: $crawlPropertyName"
							#Create the Crawled Prop
							$crawlPropertyObjs = CreateCrawledProperty -ssa $serviceApplication -name $crawlPropertyName -category $crawlPropertyCategory -isTerm $isTerm
						}
					
						#create mapping when crawl property exists and mapping doesn't exist. 
						New-SPEnterpriseSearchMetadataMapping -SearchApplication $serviceApplication -ManagedProperty $managedPropertyObj -CrawledProperty $crawlPropertyObjs                        
						Show-Message -Message "Metadata mapping created. Managed Property: $managedPropertyName; Crawl Property: $crawlPropertyName."
					}
				}
			}
			else { 
				Throw "you don't have Crawl Property defined." 
			}
			
			Show-Message -Message ""
		}
	}else { Throw "Cannot find ServiceApplication."}
}

#Creates a Crawled Property with the passed name, type, and category
function CreateCrawledProperty($ssa, $name, $category, $isTerm, $isNameEnum=$false, $isMappedToContents=$true) {
    #See if the crawled property exists already
	$prop = Get-SPEnterpriseSearchMetadataCrawledProperty -SearchApplication $ssa -name $name -ea SilentlyContinue
	if ($null -eq $prop) {
        #If not Create it
		#Note: VariantType is required but it also gets output as obsolete, not sure why.
		$variantType = 0x1f #Just use the 'Text' type since it shouldn't matter
		#We also need to specify the Propset GUID, which largely corresponds to the Category 
		if($category -eq "Sharepoint" -and $isTerm) {
			#SharePoint taxonomy values have a different GUID but the same category as other SharePoint items
			$propset = New-Object Guid '158d7563-aeff-4dbf-bf16-4a1445f0366c'
		}
		elseif($category -eq "Sharepoint") {
			$propset = New-Object Guid '00130329-0000-0130-c000-000000131346'
		}
		elseif($category -eq "Web") {
			$propset = New-Object Guid 'd1b5d3f0-c0b3-11cf-9a92-00a0c908dbf1'
		}
		else {
			Throw "CreateCrawledProperty()-->Unknown Category:" + $category
		}

		$prop = New-SPEnterpriseSearchMetadataCrawledProperty -Category $category -IsNameEnum $isNameEnum -Name $name -PropSet $propset -SearchApplication $ssa -VariantType $variantType
	}
	
	return $prop
}

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

# Set the XML folder path
$XmlFolderPath = "$ParentDirectory\Static Content\XML"

if("false" -eq $ExcelValues.ToLower()) {
    
	Show-Message -Message "Starting Managed Metadata creation..." -Type ( [MessageType]::Warning )

    cd $ScriptDirectory
    ProcessManagedPropertyXml -xmlFile "$XmlFolderPath\ManagedProperties.xml"
	
	Show-Message -Message "Completed Managed Metadata creation..." -Type ([MessageType]::Success)
}
else {
	Show-Message -Message "Online Deployment..." -Type ([MessageType]::Success)
}