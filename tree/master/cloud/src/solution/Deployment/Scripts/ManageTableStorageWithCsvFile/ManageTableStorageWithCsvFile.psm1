#--------------------------------------------------------------------------------- 
#The sample scripts are not supported under any Microsoft standard support 
#program or service. The sample scripts are provided AS IS without warranty  
#of any kind. Microsoft further disclaims all implied warranties including,  
#without limitation, any implied warranties of merchantability or of fitness for 
#a particular purpose. The entire risk arising out of the use or performance of  
#the sample scripts and documentation remains with you. In no event shall 
#Microsoft, its authors, or anyone else involved in the creation, production, or 
#delivery of the scripts be liable for any damages whatsoever (including, 
#without limitation, damages for loss of business profits, business interruption, 
#loss of business information, or other pecuniary loss) arising out of the use 
#of or inability to use the sample scripts or documentation, even if Microsoft 
#has been advised of the possibility of such damages 
#--------------------------------------------------------------------------------- 

#requires -Version 3.0

Function Export-AzureTableStorage
{
<#
 	.SYNOPSIS
        This is an advanced function which can be used to export the entity of azure table storage.
    .DESCRIPTION
        This is an advanced function which can be used to export the entity of azure table storage.
    .PARAMETER  StorageAccountName
		Spcifies the name of storage account.
    .PARAMETER  TableName
		Specifies the name of table storage.
    .PARAMETER  Path
		Specifies the path to the item that will save it to a csv file.
    .EXAMPLE
        C:\PS> Export-AzureTableStorage -StorageAccountName "storageaccount" -TableName SchemasTable -Path C:\Tables\

		Successfully export the table storage to csv file.

        This command shows how to export the entities of table storage and saves them to a csv file named 'ShemasTable'.
#>
    [CmdletBinding(SupportsShouldProcess=$true)]
    Param
    (
        [Parameter(Mandatory=$true)]
        [String]$StorageAccountName,
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
            Get-AzureRmStorageAccount -ResourceGroupName "test26aug"  -Name $StorageAccountName -verbose -ErrorAction SilentlyContinue `
            -ErrorVariable IsExistStorageError | Out-Null

            #Check if storage account is exist
            If($IsExistStorageError.Exception -eq $null)
            {
                If($TableName)
                {
                    Get-AzureStorageTable -Name $TableName -ErrorAction SilentlyContinue `
                    -ErrorVariable IsExistTableError | Out-Null

                    #Check if table is exist
                    If($IsExistTableError.Exception -eq $null)
                    {
                        #Specify a Windows Azure Storage Library path
                        $StorageLibraryPath = "$env:SystemDrive\Program Files\Microsoft SDKs\Azure\.NET SDK\v2.5\ref\Microsoft.WindowsAzure.Storage.dll"

                        #Getting Azure storage account key
                        $Keys = Get-AzureStorageKey -StorageAccountName $StorageAccountName
                        $StorageAccountKey = "yDOE9PA2C/IDy0EL50eMOY3AnUkBN7qXn4AtXDYtJg6Y9xnZtokBugYhKoVn3fMfliYtm/uOU+ry5mGNaJdwIA=="

                        #Loading Windows Azure Storage Library for .NET.
                        Write-Verbose -Message "Loading Windows Azure Storage Library from $StorageLibraryPath"
                        [Reflection.Assembly]::LoadFile("$StorageLibraryPath") | Out-Null

                        $Creds = New-Object Microsoft.WindowsAzure.Storage.Auth.StorageCredentials("$StorageAccountName","$StorageAccountKey")
                        $CloudStorageAccount = New-Object Microsoft.WindowsAzure.Storage.CloudStorageAccount($Creds, $true)
                        $CloudTableClient = $CloudStorageAccount.CreateCloudTableClient()
                        $Table = $CloudTableClient.GetTableReference($TableName)

                        $Query = New-Object "Microsoft.WindowsAzure.Storage.Table.TableQuery"
                        $Datas = $Table.ExecuteQuery($Query)
                        
                        $ExportObjs = @()
                        
                        Foreach($Data in $Datas)
                        {
  
                           $Obj = New-Object PSObject

                           $Obj | Add-Member -Name PartitionKey -Value $Data.PartitionKey -MemberType NoteProperty
                           $Obj | Add-Member -Name RowKey -Value $Data.RowKey -MemberType NoteProperty 

                           $Data.Properties.Keys | Foreach{$Value = $data.Properties[$_].PropertyAsObject;
                           $Obj | Add-Member -Name $_ -Value $value -MemberType NoteProperty; }

                           $ExportObjs += $Obj
                        } 

                        #Export the entities of table storage to csv file. 
                        $ExportObjs | Export-Csv "$Path\$TableName.csv" -NoTypeInformation
                        Write-Host "Successfully export the table storage to csv file."

                    }
                    Else
                    {
                        Write-Warning "Cannot find blob '$TableName' because it does not exist. Please make sure thar the name of table is correct."
                    }
                }
            }
            Else
            {
                Write-Warning "Cannot find storage account '$StorageAccountName' because it does not exist. Please make sure thar the name of storage is correct."
            }
        }
    }
}



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
            If($IsExistStorageError.Exception -eq $null)
            {
                If($TableName)
                {
                    #Specify a Windows Azure Storage Library path
                    $StorageLibraryPath = "$env:SystemDrive\Program Files\Microsoft SDKs\Azure\.NET SDK\v2.5\ref\Microsoft.WindowsAzure.Storage.dll"

                    #Getting Azure storage account key
                    
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
            }
            Else
            {
                Write-Warning "Cannot find storage account '$StorageAccountName' because it does not exist. Please make sure thar the name of storage is correct."
            }
        }
    }
}