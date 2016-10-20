cd..

copy App.config web.config

call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcvarsall.bat" x86_amd64

aspnet_regiis.exe -pef "credSettings" "" -prov "DataProtectionConfigurationProvider"

del /f App.config

copy web.config App.config

del /f web.config

PAUSE

	