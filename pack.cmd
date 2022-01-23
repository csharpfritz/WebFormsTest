@echo off

if not exist .\WebFormsTest\bin\release\*.dll goto :missing

:: Clean up the existing nuget working folder
rd /s /q .nuget
:: md .nuget\lib\net40
md dist

:: Copy the DLLs to the correct folder
:: copy .\WebFormsTest\bin\%1\Fritz.WebFormsTest.dll .\.nuget\lib\net40

:: Pack the nupkg
.\packages\NuGet.CommandLine.6.0.0\tools\nuget pack WebFormsTest.nuspec -OutputDirectory dist

goto :end

:missing
echo The release configuration is not available.  Check that the project has been
echo compiled properly first.
goto :end

:end

pause