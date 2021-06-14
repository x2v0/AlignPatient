:: $Id: pclean.bat 1672 2013-01-28 16:46:14Z onuchin $
:: Clean output folders

for /D /R %%i in (bin*) do (rd /s /q "%%i")
for /D /R %%i in (obj*) do (rd /s /q "%%i")
for /D /R %%i in (redist*) do (rd /s /q "%%i")
for /D /R %%i in (debug*) do (rd /s /q "%%i")
for /D /R %%i in (release*) do (rd /s /q "%%i")
for /D /R %%i in (precompiledweb*) do (rd /s /q "%%i")
for /D /R %%i in (x64*) do (rd /s /q "%%i")
for /D /R %%i in (_ReSharper*) do (rd /s /q "%%i")


:: Clean generated files

attrib /s -r -h -s *.suo
attrib /s -r -h -s Thumbs.db

del /s /q *.aps
del /s /q *_h.h
del /s /q *.ncb
del /s /q *.suo
del /s /q *.sln.cache
del /s /q *.user
del /s /q *.DotSettings
del /s /q Thumbs.db
del /s /q *_C.*

pause