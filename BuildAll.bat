@echo off

setlocal EnableDelayedExpansion

echo ==========================================
echo CivilPython Multi-Version Build
echo ==========================================

set ROOT=%~dp0

set RELEASE_DIR=%ROOT%Releases

if not exist "%RELEASE_DIR%" (
    mkdir "%RELEASE_DIR%"
)

REM ==========================================
REM Build Versions
REM ==========================================

set versions=2027

for %%V in (%versions%) do (

    echo.
    echo ==========================================
    echo BUILDING CIVIL 3D %%V
    echo ==========================================

    REM ==========================================
    REM Build Plugin
    REM ==========================================

    dotnet build "%ROOT%CivilPython\CivilPython.csproj" ^
        -c %%V ^
        --property:Platform=AnyCPU

    if errorlevel 1 (
        echo FAILED BUILD %%V
        exit /b 1
    )

    REM ==========================================
    REM Build Installer
    REM ==========================================

    dotnet run ^
        --project "%ROOT%CivilPython.Installer\CivilPython.Installer.csproj" ^
        -- %%V

    if errorlevel 1 (
        echo FAILED INSTALLER %%V
        exit /b 1
    )

    REM ==========================================
    REM Copy MSI To Releases
    REM ==========================================

    copy /Y ^
        "%ROOT%CivilPython.Installer\output\CivilPython%%VInstaller.msi" ^
        "%RELEASE_DIR%\"

    "C:\Program Files\7-zip\7z.exe" a ^
        "%RELEASE_DIR%\CivilPython%%V.bundle.zip" ^
        "%ROOT%CivilPython\CivilPython%%V.bundle"

)

echo.
echo ==========================================
echo BUILD SUCCESSFUL
echo ==========================================

pause