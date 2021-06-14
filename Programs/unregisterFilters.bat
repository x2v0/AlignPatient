:: $Id: unregisterFilters.bat 1237 2012-10-02 11:34:53Z onuchin $
:: This script unregisters DirectShow filters

@echo off
if "%OS%"=="Windows_NT" goto NT
%WINDIR%\system\regsvr32.exe /U /s PCheckPosFilter.dll
%WINDIR%\system\regsvr32.exe /U /s RtpFilters.ax
goto END
:NT
regsvr32.exe /U /s PCheckPosFilter.dll
regsvr32.exe /U /s RtpFilters.ax
:END