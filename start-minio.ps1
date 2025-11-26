# Script để khởi động MinIO bằng Docker

Write-Host "Starting MinIO server..." -ForegroundColor Green

# Tạo thư mục để lưu dữ liệu MinIO
$minioDataPath = ".\minio-data"
if (-not (Test-Path $minioDataPath)) {
    New-Item -ItemType Directory -Path $minioDataPath | Out-Null
    Write-Host "Created MinIO data directory: $minioDataPath" -ForegroundColor Yellow
}

# Kiểm tra xem Docker có sẵn không
try {
    docker --version | Out-Null
    Write-Host "Docker is available" -ForegroundColor Green
} catch {
    Write-Host "ERROR: Docker is not installed or not running!" -ForegroundColor Red
    Write-Host "Please install Docker Desktop from: https://www.docker.com/products/docker-desktop" -ForegroundColor Yellow
    Write-Host "Or use MinIO binary instead (see alternative method)" -ForegroundColor Yellow
    exit 1
}

# Kiểm tra xem MinIO container đã chạy chưa
$existingContainer = docker ps -a --filter "name=minio" --format "{{.Names}}"
if ($existingContainer -eq "minio") {
    Write-Host "MinIO container already exists. Starting it..." -ForegroundColor Yellow
    docker start minio
} else {
    Write-Host "Creating and starting MinIO container..." -ForegroundColor Yellow
    
    # Chạy MinIO container
    docker run -d `
        --name minio `
        -p 9000:9000 `
        -p 9001:9001 `
        -e "MINIO_ROOT_USER=minioadmin" `
        -e "MINIO_ROOT_PASSWORD=minioadmin" `
        -v "${PWD}\minio-data:/data" `
        quay.io/minio/minio server /data --console-address ":9001"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "MinIO started successfully!" -ForegroundColor Green
        Write-Host "MinIO API: http://localhost:9000" -ForegroundColor Cyan
        Write-Host "MinIO Console: http://localhost:9001" -ForegroundColor Cyan
        Write-Host "Username: minioadmin" -ForegroundColor Cyan
        Write-Host "Password: minioadmin" -ForegroundColor Cyan
    } else {
        Write-Host "Failed to start MinIO!" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""
Write-Host "MinIO is running. You can access:" -ForegroundColor Green
Write-Host "  - API: http://localhost:9000" -ForegroundColor White
Write-Host "  - Console UI: http://localhost:9001" -ForegroundColor White
Write-Host ""
Write-Host "To stop MinIO: docker stop minio" -ForegroundColor Yellow
Write-Host "To remove MinIO: docker rm minio" -ForegroundColor Yellow


