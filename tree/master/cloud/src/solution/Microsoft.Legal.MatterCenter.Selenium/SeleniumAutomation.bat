call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcvarsall.bat" x86_amd64
MSTest.exe /testcontainer:"TestResults\Microsoft.Legal.MatterCenter.Selenium.dll" /category:E2E /resultsfile:TestReport.trx
TestReportGenerator.exe
ECHO "Successfully completed verification."
PAUSE