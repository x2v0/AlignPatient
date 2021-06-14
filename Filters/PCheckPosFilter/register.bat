:: $Id: register.bat 1234 2012-10-01 13:12:54Z onuchin $
:: This script registers check position filter
:: Copy this file where PCheckPosFilter.dll located and run it

@echo off
if "%OS%"=="Windows_NT" goto NT
%WINDIR%\system\regsvr32.exe /s PCheckPosFilter.dll
goto END
:NT
regsvr32.exe /s PCheckPosFilter.dll
:END