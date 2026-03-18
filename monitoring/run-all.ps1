param(
    [switch]$Down,
    [switch]$Logs,
    [switch]$Status
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$composeFile = Join-Path $scriptDir "docker-compose.yml"

if (-not (Test-Path $composeFile)) {
    throw "Cannot find docker-compose.yml at '$composeFile'."
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
    docker compose -f $composeFile @ComposeArgs
}

Assert-DockerReady

if ($Down) {
    Write-Host "Stopping monitoring stack..." -ForegroundColor Yellow
    Invoke-Compose @("down")
    Write-Host "Monitoring stack stopped." -ForegroundColor Green
    exit 0
}

if ($Logs) {
    Write-Host "Streaming monitoring logs..." -ForegroundColor Cyan
    Invoke-Compose @("logs", "-f", "--tail", "200")
    exit 0
}

if ($Status) {
    Invoke-Compose @("ps")
    exit 0
}

Write-Host "Starting monitoring stack..." -ForegroundColor Yellow
Invoke-Compose @("up", "-d")

Write-Host ""
Write-Host "Monitoring stack is running:" -ForegroundColor Green
Write-Host "  Grafana:    http://localhost:3000   (admin/admin)"
Write-Host "  Prometheus: http://localhost:9090"
Write-Host "  Jaeger:     http://localhost:16686"
Write-Host "  Loki API:   http://localhost:3100/ready"
