set conf=Release
set vsver=vs2008

set ProgramFiles64bit=C:\Program Files\
set ProgramFiles32bit=C:\Program Files (x86)\

rem If you are running x86 Windows, uncomment the following line:
rem set ProgramFiles32bit=%ProgramFiles64bit%

set GACPATH="%WinDir%\assembly\GAC_MSIL\"
set Gac4path="%WinDir%\Microsoft.NET\assembly\GAC_MSIL\"

if '%vsver%'=='vs2005' goto vs2005
if '%vsver%'=='vs2008' goto vs2008
if '%vsver%'=='vs2010' goto vs2010

:vs2005
rem Visual Studio 2005 paths
set sn="%ProgramFiles64bit%\Microsoft SDKs\Windows\v6.0A\Bin\sn.exe"
set sn="%ProgramFiles32bit%\Microsoft Visual Studio 8\SDK\v2.0\Bin\sn.exe"
set gacutil="%ProgramFiles32bit%\Microsoft Visual Studio 8\SDK\v2.0\Bin\gacutil.exe" 
set msbuild="%WinDir%\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe"
goto end

:vs2008
set sn="%ProgramFiles64bit%\Microsoft SDKs\Windows\v6.0A\Bin\sn.exe"
set gacutil="%ProgramFiles64bit%\Microsoft SDKs\Windows\v6.0A\Bin\gacutil.exe"
set msbuild="%WinDir%\Microsoft.NET\Framework\v3.5\MSBuild.exe"
goto end

:vs2010
set sn="%ProgramFiles32bit%\Microsoft SDKs\Windows\v7.0A\Bin\sn.exe"
set gacutil="%ProgramFiles32bit%\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\gacutil.exe"
set msbuild="%WinDir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" 
goto end

:end
