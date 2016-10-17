copy TestReportGenerator.exe.config web.config

call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcvarsall.bat" x86_amd64

aspnet_regiis.exe -pdf "appSettings" ""

del /f TestReportGenerator.exe.config

copy web.config App.config

del /f TestReportGenerator.exe.config

