:: $Id: $
:: Clean output folders


for /D /R %%i in (bin*) do (rd /s /q "%%i")
for /D /R %%i in (obj*) do (rd /s /q "%%i")
for /D /R %%i in (redist*) do (rd /s /q "%%i")
for /D /R %%i in (debug*) do (rd /s /q "%%i")
for /D /R %%i in (release*) do (rd /s /q "%%i")
for /D /R %%i in (precompiledweb*) do (rd /s /q "%%i")
for /D /R %%i in (x64*) do (rd /s /q "%%i")

:: Clean generated files

attrib -r -h -s *.suo
del /s /q *.aps
del /s /q *_h.h
del /s /q *.ncb
del /s /q *.suo
del /s /q *.sln.cache
del /s /q *.user

pause