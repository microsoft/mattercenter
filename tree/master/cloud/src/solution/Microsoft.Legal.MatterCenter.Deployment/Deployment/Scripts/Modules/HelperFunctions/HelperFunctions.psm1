function Show-Message {
	<# 
		.SYNOPSIS
		Function to display message on console
		.DESCRIPTION
		Function to display message on console
		.EXAMPLE
		Show-Message
		.PARAMETER Message
		Message
		.PARAMETER Type
		Type of Message
		.PARAMETER NewLine
		NewLine
	#>
	[CmdletBinding()]
	param(
		[Parameter(Mandatory=$True)]
		[String]$Message,

		[Parameter(Mandatory=$False)]
		[String]$Type,

		[Parameter(Mandatory=$False)]
		[bool]$Newline = $true
	)

	begin
	{
		$timestamp = Get-Date -Format G
		$Message = $timestamp + " - " + $Message
	}

	process
	{
		switch ($Type)
		{
			([MessageType]::Success)
			{ 
			if($Newline) {
				Write-Host $Message -ForegroundColor Green
			   }
			   else {
				Write-Host $Message -ForegroundColor Green -NoNewline
			   }
			}
			([MessageType]::Warning) 
			{ 
				if($Newline) {
					Write-Host $Message -ForegroundColor Yellow     
				}
				else {
					Write-Host $Message -ForegroundColor Yellow -NoNewline
				}
			}
			([MessageType]::Failure)
			{
				if($Newline) { 
					Write-Host $Message -ForegroundColor Red 
				}
				else {
					Write-Host $Message -ForegroundColor Red -NoNewline
				}
			}
			Default { Write-Host $Message -ForegroundColor White }
		}
		# Write into log file
		if(-not [String]::IsNullOrEmpty($Message)) {
			($Message) | Out-File $LogFile -Append
		}
	}

	end {

	}
}