:: $Id: pbuild.bat 1672 2013-01-28 16:46:14Z onuchin $
:: To build - pass debug or release at the command line, e.g. pbuild.bat debug


call defines.bat

set conf=release
if not "%~1" == ""  set conf="%1"

%msbuild%  ..\BaseClasses\baseclasses.sln /t:Build /p:Configuration=%conf%
%msbuild%  .\PAudioFilter.sln /t:Build /p:Configuration=%conf%

pause