@echo off
chcp 65001 >nul
title TOLLAN - Subir a GitHub
cd /d "%~dp0"

echo ==========================================
echo   TOLLAN - Subir proyecto a GitHub
echo ==========================================
echo.

where git >nul 2>nul
if errorlevel 1 (
  echo [X] No encontre Git. Instalalo desde https://git-scm.com/download/win
  echo     ^(deja todas las opciones por defecto^) y vuelve a correr este archivo.
  pause
  exit /b 1
)

if exist ".git" goto SUBIR
echo Primero crea un repositorio VACIO y PRIVADO en https://github.com/new
echo   - Nombre: Tollan
echo   - Private
echo   - SIN README, SIN .gitignore, SIN licencia
echo.
set /p REPO="Pega aqui la URL del repo (ej. https://github.com/tuusuario/Tollan.git): "
git init
git branch -M main
git remote add origin "%REPO%"

:SUBIR
git add .
git commit -m "TOLLAN: avance %date% %time%"
echo.
echo Subiendo... si es la primera vez se abrira el navegador para iniciar sesion en GitHub.
git push -u origin main
if errorlevel 1 (
  echo.
  echo [X] No se pudo subir. Revisa la URL del repo o tu sesion de GitHub.
  pause
  exit /b 1
)
echo.
echo [OK] Listo. Invita a tus companeros en: Settings ^> Collaborators del repo.
pause
