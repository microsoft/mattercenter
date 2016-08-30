Function Import-AzureTableStorage
{
<#
 	.SYNOPSIS
        This is an advanced function which can be used to import the entities of csv file into a table storage.
    .DESCRIPTION
        This is an advanced function which can be used to import the entities of csv file into a table storage.
    .PARAMETER  StorageAccountName
		Spcifies the name of storage account.
    .PARAMETER  TableName
		Specifies the name of table storage.
    .PARAMETER  Path
		Specifies the file path of the csv to be imported. This parameter is required. 

    .EXAMPLE
        C:\PS> Import-AzureTableStorage -StorageAccountName "storageaccount" -TableName SchemasTable2 -Path C:\Tables\SchemasTable.csv

		Successfully Imported entities of table storage named 'SchemasTable'.

        This command shows how to import the entities of the csv file into a table storage.
#>
    [CmdletBinding(SupportsShouldProcess=$true)]
    Param
    (
	[Parameter(Mandatory=$true)]
        [String]$ResourceGroupName,
        [Parameter(Mandatory=$true)]
        [String]$StorageAccountName,
	    [Parameter(Mandatory=$true)]
        [String]$StorageAccountKey,
        [Parameter(Mandatory=$true)]
        [String]$TableName,
        [Parameter(Mandatory=$true)]
        [String]$Path
	
    )

    #Check if Windows Azure PowerShell Module is avaliable
    If((Get-Module -ListAvailable Azure) -eq $null)
    {
        Write-Warning "Windows Azure PowerShell module not found! Please install from http://www.windowsazure.com/en-us/downloads/#cmd-line-tools"
    }
    Else
    {
        If($StorageAccountName)
        {
            Get-AzureRmStorageAccount -ResourceGroupName $ResourceGroupName  -Name $StorageAccountName -ErrorAction SilentlyContinue `
            -ErrorVariable IsExistStorageError | Out-Null

            #Check if storage account is exist
            #If($IsExistStorageError.Exception -eq $null)
            #{
                If($TableName)
                {
                    #Specify a Windows Azure Storage Library path
                    $StorageLibraryPath = "$env:SystemDrive\Program Files\Microsoft SDKs\Azure\.NET SDK\v2.5\ref\Microsoft.WindowsAzure.Storage.dll"

                    #Getting Azure storage account key
                    $StorageAccountName = $StorageAccountName.ToLower();
                    $Creds = New-Object Microsoft.WindowsAzure.Storage.Auth.StorageCredentials("$StorageAccountName","$StorageAccountKey")
                    $CloudStorageAccount = New-Object Microsoft.WindowsAzure.Storage.CloudStorageAccount($Creds, $true)
                    $CloudTableClient = $CloudStorageAccount.CreateCloudTableClient()
                    $Table = $CloudTableClient.GetTableReference($TableName)

                    #Create a Table Storage
                    Write-Verbose "Creating a table storage named '$TableName'."
                    #Try to create table if it does not exist
                    $Table.CreateIfNotExists() | Out-Null

                    If(Test-Path -Path $Path)
                    {
                        $CsvContents = Import-Csv -Path $Path
                        $CsvHeaders = ($CsvContents[0] | Get-Member -MemberType NoteProperty).Name | Where{$_ -ne "RowKey" -and $_ -ne "PartitionKey"}

                        Foreach($CsvContent in $CsvContents)
                        {
                            $PartitionKey = $CsvContent.PartitionKey
                            $RowKey = $CsvContent.RowKey
                            $Entity = New-Object "Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity" "$PartitionKey", "$RowKey"

                            Foreach($CsvHeader in $CsvHeaders)
                            {
                                $Value = $CsvContent.$CsvHeader
                                $Entity.Properties.Add($CsvHeader, $Value)
                            }
                            Write-Verbose "Inserting the entity into table storage."
                            $result = $Table.Execute([Microsoft.WindowsAzure.Storage.Table.TableOperation]::Insert($Entity))
                        }
                        Write-Host "Successfully Imported entities of table storage named '$TableName'."
                    }
                    Else
                    {
                        Write-Warning "The path does not exist, please check it is correct."
                    }
                }
            #}
            #Else
            #{
             #   Write-Warning "Cannot find storage account '$StorageAccountName' because it does not exist. Please make sure thar the name of storage is correct."
            #}
        }
    }
}

Function Create-AzureTableStorage
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    Param
    (
	    [Parameter(Mandatory=$true)]
        [String]$ResourceGroupName,
        [Parameter(Mandatory=$true)]
        [String]$StorageAccountName,
	    [Parameter(Mandatory=$true)]
        [String]$StorageAccountKey,
        [Parameter(Mandatory=$true)]
        [String]$TableName      
	
    )  

    #Import-Module "$PSScriptRoot\ManageTableStorageWithCsvFile\ManageTableStorageWithCsvFile.psm1"
    $Path = "$PSScriptRoot\ManageTableStorageWithCsvFile\AzureStorageTable.csv"
    #Get-Module -Name ManageTableStorageWithCsvFile
    #Get-Command -Module ManageTableStorageWithCsvFile
    Import-AzureTableStorage -ResourceGroupName $ResourceGroupName -StorageAccountName $StorageAccountName -StorageAccountKey $StorageAccountKey -TableName $TableName -Path $Path
    
}

$Path = "$PSScriptRoot\ManageTableStorageWithCsvFile\AzureStorageTable.csv"

Import-AzureTableStorage -ResourceGroupName $ResourceGroupName -StorageAccountName $storageAccount_name -StorageAccountKey $StorageAccountKey.Item(0).Value -TableName "MatterCenterConfiguration" -Path $Path
#Create-AzureTableStorage -ResourceGroupName $ResourceGroupName -StorageAccountName $storageAccount_name -StorageAccountKey $StorageAccountKey.Item(0).Value -TableName "MatterCenterConfiguration"
