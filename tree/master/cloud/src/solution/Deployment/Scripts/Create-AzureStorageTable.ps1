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

    Import-Module "$PSScriptRoot\ManageTableStorageWithCsvFile\ManageTableStorageWithCsvFile.psm1"
    $Path = "$PSScriptRoot\ManageTableStorageWithCsvFile\AzureStorageTable.csv"
    Get-Module -Name ManageTableStorageWithCsvFile
    Get-Command -Module ManageTableStorageWithCsvFile
    Import-AzureTableStorage -ResourceGroupName $ResourceGroupName -StorageAccountName $StorageAccountName -StorageAccountKey $StorageAccountKey -TableName $TableName -Path $Path
    
}