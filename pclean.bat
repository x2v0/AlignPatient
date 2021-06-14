:: $Id: pclean.bat 2055 2014-01-31 07:10:11Z onuchin $
:: Author: Valeriy Onuchin   29.12.2010
::
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
del /s /q *.sln.cache
del /s /q *.user
del /s /q *.DotSettings
del /s /q Thumbs.db
del /s /q *.pch 
del /s /q *.opt
del /s /q *.plg
del /s /q *.bsc
del /s /q *.bak
del /s /q *.pdb
::del *.sql /s /q
::del *.mdb /s /q
::del *.lib /s /q
del *.exp /s /q
del *.ilk /s /q
del *.idb /s /q
del *.aps /s /q
del *.suo /s /q /a:h
::del *.o /s /q


:: clean ACLiC generated files
del /s /q *.d
del /s /q *.rootmap
del /s /q *_C.dll.manifest
del /s /q *_C.exp
del /s /q *_C.lib
del /s /q *_C.pdb
del /s /q *_C.def

del /q Programs\Doctor\*.dll
del /q Programs\Patient\*.dll

pause
