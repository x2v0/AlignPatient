:: $Id: $
:: To  build - pass debug or release at the command line, e.g. pbuild.bat debug


call defines.bat

set conf=release
if not "%~1" == ""  set conf="%1"

%msbuild%  ..\BaseClasses\baseclasses.sln /t:Build /p:Configuration=%conf%
%msbuild%  .\RtpFilters.sln /t:Build /p:Configuration=%conf%

pause