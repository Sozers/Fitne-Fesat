@echo off
rem Fitne&Fesat: tek bilgisayarda birkac "telefon" penceresiyle deneme.
rem Kullanim: deneme.bat [oyuncu sayisi, varsayilan 6, en cok 15]
rem Ilk pencere odayi kurar, digerleri 127.0.0.1 adresine katilir.
setlocal enabledelayedexpansion
set EXE=%~dp0..\Builds\Windows\FitneFesat.exe
if not exist "%EXE%" (
  echo Once Windows surumunu derleyin: Unity'de "Fitne ve Fesat > Windows Denemesi Derle".
  pause
  exit /b 1
)
set N=%1
if "%N%"=="" set N=6
set NAMES=Ali Ayse Mehmet Zeynep Hasan Elif Mustafa Fatma Ahmet Emine Huseyin Hatice Ibrahim Meryem Osman
set /a i=0
for %%n in (%NAMES%) do (
  if !i! LSS %N% (
    set /a "x=(!i! %% 6) * 310"
    set /a "y=40 + (!i! / 6) * 300"
    if !i!==0 (
      start "" "%EXE%" -screen-fullscreen 0 -screen-width 300 -screen-height 534 -ad %%n -oda-kur -pencere !x!,!y!
      timeout /t 3 /nobreak >nul
    ) else (
      start "" "%EXE%" -screen-fullscreen 0 -screen-width 300 -screen-height 534 -ad %%n -katil 127.0.0.1 -pencere !x!,!y!
      timeout /t 1 /nobreak >nul
    )
    set /a i+=1
  )
)
endlocal
