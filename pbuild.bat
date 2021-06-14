:: $Id: pbuild.bat 4345 2016-08-22 08:37:30Z onuchin $
:: Author: Valeriy Onuchin   29.12.2010
::
:: To  build - pass debug or release at the command line, e.g. pbuild.bat debug

call defines.bat

set conf=release
if not "%~1" == ""  set conf=%1

%msbuild% /nologo  .\Network\UdpTcp\Network.sln /t:Build /p:Configuration=%conf%
%msbuild% /nologo  .\Common\PCommon.sln /t:Build /p:Configuration=%conf% 
%msbuild% /nologo  .\Network\Rtp\PRtp.sln /t:Build /p:Configuration=%conf%

%msbuild% /nologo  .\Filters\BaseClasses\baseclasses.sln /t:Build /p:Configuration=%conf%
%msbuild% /nologo  .\Filters\PCheckPosFilter\PCheckPosFilter.sln /t:Build /p:Configuration=%conf%
%msbuild% /nologo  .\Filters\RtpFilters\RtpFilters.sln /t:Build /p:Configuration=%conf%
%msbuild% /nologo  .\Filters\PAudioFilter\PAudioFilter.sln /t:Build /p:Configuration=%conf%
%msbuild% /nologo  .\Filters\DmoEnum\DmoEnum.sln  /t:Build /p:Configuration=%conf%

%msbuild% /nologo  .\DShow\PDShow.sln /t:Build /p:Configuration=%conf%

::%msbuild% /nologo  .\ROOT.NET\ROOT.NET.sln /t:Build /p:Configuration=%conf%


%msbuild% /nologo  .\Programs\Doctor\DoctorDisplay.csproj /t:Build /p:Configuration=%conf% /p:Platform=x86
%msbuild% /nologo  .\Programs\Patient\PatientDisplay.csproj /t:Build /p:Configuration=%conf% /p:Platform=x86


cd .\3rd-party\ROOT.NET
call .\pbuild.bat %conf%
cd ..\..\


if %conf%==release (
del /s /q *.pdb
)

pause

