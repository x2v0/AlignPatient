:: $Id: register.bat 1672 2013-01-28 16:46:14Z onuchin $
:: This script registers audio filter
:: Copy this file where PAudioFilter.ax located and run it

@echo off
if "%OS%"=="Windows_NT" goto NT
%WINDIR%\system\regsvr32.exe /s PAudioFilter.ax
goto END
:NT
regsvr32.exe /s PAudioFilter.ax
:END