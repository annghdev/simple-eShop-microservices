Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent (Split-Path -Parent $scriptDir)
$composeFile = Join-Path $scriptDir "docker-compose.yaml"
$envFile = Join-Path $scriptDir ".env"
$envExampleFile = Join-Path $scriptDir ".env.example"

if (-not (Test-Path $composeFile)) {
    throw "Cannot find docker-compose.yaml at '$composeFile'."
}

if (-not (Test-Path $envExampleFile)) {
    throw "Cannot find .env.example at '$envExampleFile'."
}

if (-not (Test-Path $envFile)) {
    Copy-Item $envExampleFile $envFile
    Write-Host "Created .env from .env.example. Update secrets before using in real production." -ForegroundColor Yellow
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
    docker compose --env-file $envFile -f $composeFile @ComposeArgs
}

Assert-DockerReady
Push-Location $repoRoot
try {
    Write-Host "Building and starting production stack..." -ForegroundColor Yellow
    Invoke-Compose @("up", "-d", "--build")

    Write-Host ""
    Write-Host "Production stack is running:" -ForegroundColor Green
    Write-Host "  APIGateway: http://localhost:8080"
    Write-Host "  Grafana:    http://localhost:3000"
    Write-Host "  Prometheus: http://localhost:9090"
    Write-Host "  Jaeger:     http://localhost:16686"
}
finally {
    Pop-Location
}
