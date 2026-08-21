param(
    [switch]$KeepDocker,
    [switch]$Force,
    [int]$DockerShutdownTimeoutSeconds = 120
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Stop-MatchingProcess {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string[]]$MatchPatterns,
        [switch]$ForceKill
    )

    $stopped = @()
    $procs = Get-CimInstance Win32_Process -Filter "Name = '$Name'"

    foreach ($proc in $procs) {
        $cmd = $proc.CommandLine
        if ([string]::IsNullOrWhiteSpace($cmd)) {
            continue
        }

        $matched = $false
        foreach ($pattern in $MatchPatterns) {
            if ($cmd -match $pattern) {
                $matched = $true
                break
            }
        }

        if (-not $matched) {
            continue
        }

        $process = Get-Process -Id $proc.ProcessId -ErrorAction SilentlyContinue
        if ($null -eq $process) {
            continue
        }

        if ($ForceKill) {
            Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        }
        else {
            Stop-Process -Id $process.Id -ErrorAction SilentlyContinue
        }

        $stopped += $process.Id
    }

    return $stopped
}

function Test-DockerDaemonRunning {
    cmd /c "docker info >nul 2>nul"
    return $LASTEXITCODE -eq 0
}

function Stop-DockerDesktop {
    param([switch]$ForceKill)

    $names = @("Docker Desktop", "com.docker.backend", "com.docker.build")
    $stopped = @()

    foreach ($name in $names) {
        $procs = @(Get-Process -Name $name -ErrorAction SilentlyContinue)
        foreach ($proc in $procs) {
            if ($ForceKill) {
                Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
            }
            else {
                Stop-Process -Id $proc.Id -ErrorAction SilentlyContinue
            }
            $stopped += $proc.Id
        }
    }

    return $stopped
}

function Wait-ForContainerStopped {
    param(
        [string]$ContainerName = "contractorpro-db",
        [int]$TimeoutSeconds = 120
    )

    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $deadline) {
        # Suppress stderr and check exit code
        $null = & docker inspect $ContainerName 2>$null
        if ($LASTEXITCODE -ne 0) {
            # Container doesn't exist or error occurred (expected after compose down)
            return
        }

        $running = & docker inspect -f "{{.State.Running}}" $ContainerName 2>$null
        if ($LASTEXITCODE -ne 0 -or $running -ne "true") {
            return
        }

        Start-Sleep -Seconds 2
    }

    throw "Container '$ContainerName' did not stop within $TimeoutSeconds seconds."
}

function Wait-ForDockerDesktopStopped {
    param([int]$TimeoutSeconds = 120)

    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $deadline) {
        $procs = @(Get-Process -Name "Docker Desktop", "com.docker.backend", "com.docker.build" -ErrorAction SilentlyContinue)
        if ($procs.Count -eq 0) {
            return
        }

        Start-Sleep -Seconds 2
    }

    throw "Docker Desktop processes did not stop within $TimeoutSeconds seconds."
}

function Stop-TrackedShellProcesses {
    param(
        [Parameter(Mandatory = $true)][string]$RuntimeDirectory,
        [switch]$ForceKill
    )

    $stopped = @()
    $pidFiles = @("api-shell.pid", "web-shell.pid")

    foreach ($pidFile in $pidFiles) {
        $fullPath = Join-Path $RuntimeDirectory $pidFile
        if (-not (Test-Path $fullPath)) {
            continue
        }

        $rawPid = Get-Content -Path $fullPath -ErrorAction SilentlyContinue
        $trackedPid = 0
        if (-not [int]::TryParse($rawPid, [ref]$trackedPid)) {
            Remove-Item -Path $fullPath -ErrorAction SilentlyContinue
            continue
        }

        $proc = Get-Process -Id $trackedPid -ErrorAction SilentlyContinue
        if ($null -ne $proc) {
            if ($ForceKill) {
                Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
            }
            else {
                Stop-Process -Id $proc.Id -ErrorAction SilentlyContinue
            }
            $stopped += $proc.Id
        }

        Remove-Item -Path $fullPath -ErrorAction SilentlyContinue
    }

    return $stopped
}

$repoRoot = Split-Path -Parent $PSScriptRoot
$runtimeDir = Join-Path $PSScriptRoot ".runtime"

Write-Host "==> ContractorPro local stop"
Write-Host "Repo root: $repoRoot"

$shellIds = @(Stop-TrackedShellProcesses -RuntimeDirectory $runtimeDir -ForceKill:$Force)
if ($shellIds.Count -gt 0) {
    Write-Host "==> Closed shell window process IDs: $($shellIds -join ', ')"
}
else {
    Write-Host "==> No tracked shell windows found"
}

# Stop API process launched by run-local.ps1
$apiIds = @(Stop-MatchingProcess -Name "dotnet.exe" -MatchPatterns @(
    "dotnet\s+run\s+--project\s+src/ContractorPro.Api",
    "dotnet\s+run\s+--project\s+src\\ContractorPro.Api"
) -ForceKill:$Force)

if ($apiIds.Count -gt 0) {
    Write-Host "==> Stopped API process IDs: $($apiIds -join ', ')"
}
else {
    Write-Host "==> No matching API dotnet process found"
}

# Stop frontend dev server process launched by run-local.ps1
$webIds = @(Stop-MatchingProcess -Name "node.exe" -MatchPatterns @(
    "npm\s+run\s+dev",
    "vite"
) -ForceKill:$Force)

if ($webIds.Count -gt 0) {
    Write-Host "==> Stopped web process IDs: $($webIds -join ', ')"
}
else {
    Write-Host "==> No matching web node process found"
}

if (-not $KeepDocker) {
    if (Get-Command docker -ErrorAction SilentlyContinue) {
        Push-Location $repoRoot
        try {
            if (Test-DockerDaemonRunning) {
                Write-Host "==> Stopping docker compose services"
                docker compose down
                Wait-ForContainerStopped -TimeoutSeconds $DockerShutdownTimeoutSeconds
            }
            else {
                Write-Host "==> Docker is installed but daemon is not running, skipping compose shutdown"
            }
        }
        finally {
            Pop-Location
        }

        $dockerShellIds = @(Stop-DockerDesktop -ForceKill:$Force)
        if ($dockerShellIds.Count -gt 0) {
            Write-Host "==> Stopped Docker Desktop process IDs: $($dockerShellIds -join ', ')"
            Wait-ForDockerDesktopStopped -TimeoutSeconds $DockerShutdownTimeoutSeconds
            Write-Host "==> Docker Desktop processes have exited"
        }
        else {
            Write-Host "==> No Docker Desktop processes found"
        }
    }
    else {
        Write-Host "==> Docker command not found, skipping compose shutdown"
    }
}
else {
    Write-Host "==> KeepDocker set, docker compose left running"
}

Write-Host "==> Local stop complete"
