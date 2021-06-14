:: $Id: registerFilters.bat 4344 2016-08-22 08:08:56Z onuchin $
:: This script registers DirectShow filters


@echo off
if "%OS%"=="Windows_NT" goto NT
%WINDIR%\system\regsvr32.exe /s PCheckPosFilter.dll
%WINDIR%\system\regsvr32.exe /s RtpFilters.ax
goto END
:NT
regsvr32.exe /s PCheckPosFilter.dll
regsvr32.exe /U /s RtpFilters.ax
:END
pause