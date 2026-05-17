$ErrorActionPreference = 'Stop'
$root      = Split-Path $PSScriptRoot -Parent
$project   = Join-Path $root 'GoodnightComputer\GoodnightComputer.csproj'
$issScript = Join-Path $PSScriptRoot 'GNC.iss'

# Refresh PATH from registry so installs done in this session are found
$env:Path = [System.Environment]::GetEnvironmentVariable('Path','Machine') + ';' + [System.Environment]::GetEnvironmentVariable('Path','User')

# Find ISCC.exe — check common paths then fall back to PATH
$iscc = 'C:\Program Files (x86)\Inno Setup 6\ISCC.exe'
if (-not (Test-Path $iscc)) { $iscc = "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe" }
if (-not (Test-Path $iscc)) {
    $onPath = Get-Command ISCC.exe -ErrorAction SilentlyContinue
    if ($onPath) { $iscc = $onPath.Source }
}

Write-Host '==> Publishing application...' -ForegroundColor Cyan
dotnet publish $project --configuration Release --runtime win-x64 --self-contained true /p:PublishSingleFile=true /p:EnableCompressionInSingleFile=false
if ($LASTEXITCODE -ne 0) { throw 'dotnet publish failed.' }

Write-Host '==> Compiling installer...' -ForegroundColor Cyan

if (-not (Test-Path $iscc)) {
    Write-Host 'Inno Setup not found - installing via winget...' -ForegroundColor Yellow
    winget install --id JRSoftware.InnoSetup --silent --accept-package-agreements --accept-source-agreements
    $env:Path = [System.Environment]::GetEnvironmentVariable('Path','Machine') + ';' + [System.Environment]::GetEnvironmentVariable('Path','User')
    $iscc = 'C:\Program Files (x86)\Inno Setup 6\ISCC.exe'
    if (-not (Test-Path $iscc)) {
        $onPath = Get-Command ISCC.exe -ErrorAction SilentlyContinue
        if ($onPath) { $iscc = $onPath.Source } else { throw 'Inno Setup not found. Install from https://jrsoftware.org/isinfo.php' }
    }
    Write-Host 'Inno Setup installed.' -ForegroundColor Green
}

Push-Location $PSScriptRoot
& $iscc $issScript
Pop-Location

if ($LASTEXITCODE -ne 0) { throw 'Inno Setup compilation failed.' }

Write-Host '==> Done! Installer written to installer\output\' -ForegroundColor Green