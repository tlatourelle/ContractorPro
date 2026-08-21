param(
    [switch]$SkipDocker,
    [switch]$SkipFrontend
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Require-Command {
    param([Parameter(Mandatory = $true)][string]$Name)

    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        throw "Required command '$Name' was not found in PATH."
    }
}

$repoRoot = Split-Path -Parent $PSScriptRoot

Write-Host "==> ContractorPro setup + test"
Write-Host "Repo root: $repoRoot"

Require-Command dotnet
Require-Command npm

Push-Location $repoRoot
try {
    if (-not $SkipDocker) {
        if (Get-Command docker -ErrorAction SilentlyContinue) {
            Write-Host "==> Starting PostgreSQL with docker compose"
            docker compose up -d
        }
        else {
            Write-Host "==> Docker not found in PATH. Skipping docker compose startup."
            Write-Host "==> Ensure local PostgreSQL is running, or re-run with -SkipDocker."
        }
    }
    else {
        Write-Host "==> SkipDocker set, not starting docker compose"
    }

    Write-Host "==> Restoring .NET solution"
    dotnet restore ContractorPro.sln

    Write-Host "==> Building .NET solution"
    dotnet build ContractorPro.sln --verbosity minimal

    Write-Host "==> Running .NET tests"
    dotnet test ContractorPro.sln --verbosity minimal

    if (-not $SkipFrontend) {
        Write-Host "==> Installing frontend dependencies"
        Push-Location (Join-Path $repoRoot "src/ContractorPro.Web")
        try {
            if (Test-Path "package-lock.json") {
                npm ci
            }
            else {
                npm install
            }

            Write-Host "==> Running frontend lint"
            npm run lint

            Write-Host "==> Running frontend build"
            npm run build
        }
        finally {
            Pop-Location
        }
    }
    else {
        Write-Host "==> SkipFrontend set, not running npm lint/build"
    }

    Write-Host "==> Setup + test complete"
}
finally {
    Pop-Location
}
