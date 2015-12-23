# Get the current directory of the script
Function ScriptRoot {Split-Path $MyInvocation.ScriptName}
$ScriptDirectory = (ScriptRoot)

# Function is used to read values from Excel file
Function Read-FromExcel([string]$ExcelFilePath,[string] $SheetName, [string[]]$Value, [string]$LogFilePath){
      try
      {
          $temp = ""          
          $Assembly = [Reflection.Assembly]::LoadFile(“$ScriptDirectory\Helper Utilities\Microsoft.Legal.MatterCenter.Common.dll”)
          $excelValues = [Microsoft.Legal.MatterCenter.Common.ExcelOperations]::ReadFromExcel($ExcelFilePath,$SheetName)
          for($iterator = 0; $iterator -lt $Value.Length; $iterator++){
              if($iterator -ne 0) {
                   $temp += ";"
               }
              $temp += $excelValues.Item($Value[$iterator])
          }
          return $temp
      }
      catch
      {
            $ErrorMessage = $Error[0].Exception.ErrorRecord.Exception.Message                             
            Write-Log $LogFilePath $ErrorMessage
            return $false
      }
}

# Function to read Excel Sheet
Function ReadSheet-FromExcel([string]$ExcelFilePath,[string] $SheetName, [string]$LogFilePath){
       try{
           $temp = ""          
           $Assembly = [Reflection.Assembly]::LoadFile(“$ScriptDirectory\Helper Utilities\Microsoft.Legal.MatterCenter.Common.dll”)
           $excelValues = [Microsoft.Legal.MatterCenter.Common.ExcelOperations]::ReadSheet($ExcelFilePath,$SheetName)
           return $excelValues
          }
      catch
      {
            $ErrorMessage = $Error[0].Exception.ErrorRecord.Exception.Message                             
            Write-Log $LogFilePath $ErrorMessage
            return $false
      }
}

# Function is used to write to log file
Function Write-Log() 
{
    param(
        
        [parameter(Mandatory=$false)]            
        [ValidateNotNullOrEmpty()] 
        [string] $ErrorLogFilePath
        
       ,[parameter(Mandatory=$true)]            
        [ValidateNotNullOrEmpty()] 
        [string] $ErrorMessage

        )

    Write-Host $ErrorMessage -ForegroundColor Red
    ($ErrorMessage + " occurred at " + (Get-Date -format "dd-MMM-yyyy HH:mm")) | Out-File $ErrorLogFilePath -Append
}

# Function to display to success or error message for steps
Function Trace-ErrorLogFile($SuccessMessage , $FailureMessage, [int]$StepComplete)
{    
    If ((Get-Content $ErrorLogFile) -ne $Null) {
		Write-host $FailureMessage -ForegroundColor Red
        Write-host 'Upgrade failed,' $($global:TotalSteps - $StepComplete) 'steps are remaining.' -ForegroundColor Red        
       Break
	}
	else {
	    Write-host $SuccessMessage -ForegroundColor Green         
	}
}


# Function to check if Matter Center App is present or no. For Delta deployment

Function Check-IsMatterCenterAppExist($Credential, $TenantAdminURL, $appName, $ErrorLogFile)
{
   try
    {   
        Connect-SPOService -Url $TenantAdminURL -Credential $Credential

        $AppInfo = Get-SPOAppInfo -Name $appName

        if($AppInfo -eq $Null){
            Write-Log $ErrorLogFile 'Matter Center App is not installed and upgrade cannot be performed'
            break       
        }
        else
        {
        Write-Host 'Matter Center App is present. Proceeding with upgrade' -ForegroundColor Green 
        }
    }
    catch [exception]
    {
        Write-Log $ErrorLogFile "Failed to get App Instance." 
        break      
    }
}

#Function to get credential object
Function New-PSCredential($UserName, $Password)
{
    $credential = New-Object -TypeName System.Management.Automation.PSCredential -argumentlist $UserName, $(convertto-securestring $Password -asplaintext -force);
    return $credential

}