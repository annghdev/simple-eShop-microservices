Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent (Split-Path -Parent $scriptDir)
$composeFile = Join-Path $scriptDir "docker-compose.yaml"
$envFile = Join-Path $scriptDir ".env"

if (-not (Test-Path $composeFile)) {
    throw "Cannot find docker-compose.yaml at '$composeFile'."
}

function Assert-DockerReady {
    try {
        docker info | Out-Null
    }
    catch {
        throw "Docker daemon is not available. Start Docker Desktop first."
    }
}

function Invoke-Compose([string[]]$ComposeArgs) {
    if (Test-Path $envFile) {
        docker compose --env-file $envFile -f $composeFile @ComposeArgs
    }
    else {
        docker compose -f $composeFile @ComposeArgs
    }
}

Assert-DockerReady
Push-Location $repoRoot
try {
    Write-Host "Stopping production stack..." -ForegroundColor Yellow
    Invoke-Compose @("down", "--remove-orphans")
    Write-Host "Production stack stopped." -ForegroundColor Green
}
finally {
    Pop-Location
}
