param(
    [switch]$SkipDocker,
    [switch]$SkipMigrations,
    [switch]$SkipNpmInstall,
    [int]$DockerTimeoutSeconds = 240,
    [int]$PostgresTimeoutSeconds = 120
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Require-Command {
    param([Parameter(Mandatory = $true)][string]$Name)

    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        throw "Required command '$Name' was not found in PATH."
    }
}

function Test-DockerDaemonRunning {
    cmd /c "docker info >nul 2>nul"
    return $LASTEXITCODE -eq 0
}

function Get-DockerDesktopPath {
    $candidates = @(
        "$env:ProgramFiles\Docker\Docker\Docker Desktop.exe",
        "$env:ProgramFiles(x86)\Docker\Docker\Docker Desktop.exe"
    )

    foreach ($candidate in $candidates) {
        if (Test-Path $candidate) {
            return $candidate
        }
    }

    return $null
}

function Ensure-DockerDaemon {
    param(
        [int]$TimeoutSeconds = 240
    )

    if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
        throw "Docker CLI was not found in PATH. Install Docker Desktop first."
    }

    if (Test-DockerDaemonRunning) {
        Write-Host "==> Docker daemon is already running"
        return
    }

    $desktopPath = Get-DockerDesktopPath
    if ($null -eq $desktopPath) {
        throw "Docker Desktop executable was not found. Install Docker Desktop or run with -SkipDocker."
    }

    if (-not (Get-Process -Name "Docker Desktop" -ErrorAction SilentlyContinue)) {
        Write-Host "==> Starting Docker Desktop"
        Start-Process -FilePath $desktopPath | Out-Null
    }
    else {
        Write-Host "==> Docker Desktop is running, waiting for daemon"
    }

    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $deadline) {
        if (Test-DockerDaemonRunning) {
            Write-Host "==> Docker daemon is ready"
            return
        }

        Start-Sleep -Seconds 2
    }

    throw "Docker daemon did not become ready within $TimeoutSeconds seconds. Open Docker Desktop and retry."
}

function Wait-ForPostgresReady {
    param(
        [string]$ContainerName = "contractorpro-db",
        [int]$TimeoutSeconds = 120
    )

    Write-Host "==> Waiting for PostgreSQL container health ($ContainerName)"

    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $deadline) {
        $running = (docker inspect -f "{{.State.Running}}" $ContainerName 2>$null)
        $health = (docker inspect -f "{{if .State.Health}}{{.State.Health.Status}}{{else}}none{{end}}" $ContainerName 2>$null)

        if ($running -eq "true" -and ($health -eq "healthy" -or $health -eq "none")) {
            Write-Host "==> PostgreSQL container is ready"
            return
        }

        Start-Sleep -Seconds 2
    }

    throw "PostgreSQL container '$ContainerName' did not become ready within $TimeoutSeconds seconds."
}

function Ensure-DotnetEf {
    param([Parameter(Mandatory = $true)][string]$RepoRoot)

    Push-Location $RepoRoot
    try {
        $manifestPathConfig = Join-Path $RepoRoot ".config/dotnet-tools.json"
        $manifestPathRoot = Join-Path $RepoRoot "dotnet-tools.json"

        if (-not (Test-Path $manifestPathConfig) -and -not (Test-Path $manifestPathRoot)) {
            Write-Host "==> Creating local dotnet tool manifest"
            dotnet new tool-manifest
        }

        Write-Host "==> Restoring local dotnet tools"
        dotnet tool restore

        $dotnetEfOutput = dotnet tool list --local | Out-String
        if ($dotnetEfOutput -notmatch "\bdotnet-ef\b") {
            Write-Host "==> Installing local dotnet-ef tool"
            dotnet tool install --local dotnet-ef
        }
    }
    finally {
        Pop-Location
    }
}

function Ensure-UserSecrets {
    param([Parameter(Mandatory = $true)][string]$RepoRoot)

    $apiProjectDir = Join-Path $RepoRoot "src/ContractorPro.Api"
    
    Push-Location $apiProjectDir
    try {
        Write-Host "==> Setting database connection string in user secrets"
        dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=contractorpro;Username=postgres;Password=postgres"
    }
    finally {
        Pop-Location
    }
}

$repoRoot = Split-Path -Parent $PSScriptRoot
$webRoot = Join-Path $repoRoot "src/ContractorPro.Web"
$runtimeDir = Join-Path $PSScriptRoot ".runtime"

Write-Host "==> ContractorPro local run"
Write-Host "Repo root: $repoRoot"

Require-Command dotnet
Require-Command npm

Push-Location $repoRoot
try {
    if (-not $SkipDocker) {
        Ensure-DockerDaemon -TimeoutSeconds $DockerTimeoutSeconds
        Write-Host "==> Starting PostgreSQL with docker compose"
        docker compose up -d
        Wait-ForPostgresReady -TimeoutSeconds $PostgresTimeoutSeconds
    }
    else {
        Write-Host "==> SkipDocker set, not starting docker compose"
    }

    if (-not $SkipMigrations) {
        Ensure-DotnetEf -RepoRoot $repoRoot
        Ensure-UserSecrets -RepoRoot $repoRoot
        Write-Host "==> Applying EF migrations"
        Push-Location (Join-Path $repoRoot "src/ContractorPro.Api")
        try {
            $env:ASPNETCORE_ENVIRONMENT = "Development"
            dotnet ef database update -p ../ContractorPro.Infrastructure
            if ($LASTEXITCODE -ne 0) {
                throw "EF migrations failed. Ensure PostgreSQL is running (Docker Desktop or local Postgres), then retry."
            }
        }
        finally {
            $env:ASPNETCORE_ENVIRONMENT = ""
            Pop-Location
        }
    }
    else {
        Write-Host "==> SkipMigrations set, not applying migrations"
    }

    if (-not $SkipNpmInstall) {
        Write-Host "==> Installing frontend dependencies"
        Push-Location $webRoot
        try {
            if (Test-Path "package-lock.json") {
                npm ci
            }
            else {
                npm install
            }
            if ($LASTEXITCODE -ne 0) {
                throw "npm install failed. Check your npm/node installation."
            }
        }
        finally {
            Pop-Location
        }
    }
    else {
        Write-Host "==> SkipNpmInstall set, not installing npm dependencies"
    }

    Write-Host "==> Launching API and frontend in new PowerShell windows"

    $apiCommand = "`$env:ASPNETCORE_ENVIRONMENT = 'Development'; Set-Location '$repoRoot'; dotnet run --project src/ContractorPro.Api"
    $webCommand = "Set-Location '$webRoot'; npm run dev"

    $apiShell = Start-Process powershell -ArgumentList @('-NoExit', '-Command', $apiCommand) -PassThru
    $webShell = Start-Process powershell -ArgumentList @('-NoExit', '-Command', $webCommand) -PassThru

    if (-not (Test-Path $runtimeDir)) {
        New-Item -ItemType Directory -Path $runtimeDir | Out-Null
    }

    Set-Content -Path (Join-Path $runtimeDir "api-shell.pid") -Value $apiShell.Id -NoNewline
    Set-Content -Path (Join-Path $runtimeDir "web-shell.pid") -Value $webShell.Id -NoNewline

    Write-Host "==> Local app started"
    Write-Host "API: http://localhost:5000"
    Write-Host "Web: http://localhost:5173/app/login"
}
finally {
    Pop-Location
}
