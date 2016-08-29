Function Create-MatterCenterWebJob
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    Param
    (
	    [Parameter(Mandatory=$true)]
        [String]$WebSiteName,
        [Parameter(Mandatory=$true)]
        [String]$UserName,
	    [Parameter(Mandatory=$true)]
        [String]$PassWord
    )  

    #Import-Module "$PSScriptRoot\ManageTableStorageWithCsvFile\ManageTableStorageWithCsvFile.psm1"
    $webJobBinariesPath = "$PSScriptRoot\WebJob\MatterCenterWebJobs.zip"
	$jobId = [GUID]::NewGuid()
	$job = New-AzureWebsiteJob -Name $WebSiteName `
							-JobName "MatterCenterWebJobs" `
							-JobType Continuous `
							-JobFile $webJobBinariesPath;
	$jobCollection = New-AzureSchedulerJobCollection `
					-Location 'West US' `
					-JobCollectionName $jobId;
	$authPair = "$($UserName):$($PassWord)";
	$pairBytes = [System.Text.Encoding]::UTF8.GetBytes($authPair);
	$encodedPair = [System.Convert]::ToBase64String($pairBytes);
	New-AzureSchedulerHttpJob `
	  -JobCollectionName $jobCollection[0].JobCollectionName `
	  -JobName "test" `
	  -Method POST `
	  -URI "$($job.Url)\run" `
	  -Location 'West US' `
	  -StartTime "2014-01-01" `
	  -Interval 1 `
	  -Frequency Minute `
	  -EndTime "2015-01-01" `
	  -Headers @{ `
		"Content-Type" = "text/plain"; `
		"Authorization" = "Basic $encodedPair"; `
	  };
    
}