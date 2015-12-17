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
          for($i = 0; $i -lt $Value.Length; $i++){
              if($i -ne 0) {
                   $temp += ";"
               }
              $temp += $excelValues.Item($Value[$i])
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
    ($ErrorMessage + " occurred at" + (Get-Date -format "dd-MMM-yyyy HH:mm")) | Out-File $ErrorLogFilePath -Append
}