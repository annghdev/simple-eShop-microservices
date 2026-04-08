# Production compose - test trên Docker Desktop

Tài liệu này hướng dẫn chạy và test nhanh stack production trong `docker-composes/production/` trên Docker Desktop (Windows/PowerShell).

## 1) Điều kiện tiên quyết

- Đã cài Docker Desktop và đang ở trạng thái Running.
- Đang dùng PowerShell tại thư mục gốc repo `EShopMicroservices`.

## 2) Chuẩn bị env

Script `run.ps1` sẽ tự tạo file `.env` từ `.env.example` nếu chưa có.

Nếu muốn tạo thủ công:

```powershell
Copy-Item .\docker-composes\production\.env.example .\docker-composes\production\.env
```

Sau đó đổi các giá trị nhạy cảm trong `.env` (mật khẩu Postgres, Redis, RabbitMQ, Grafana admin).

## 3) Chạy stack

```powershell
.\docker-composes\production\run.ps1
```

Script sẽ thực hiện `docker compose up -d --build`, tự động build các service .NET từ source (không cần image app trên registry riêng) với `--project-directory .\docker-composes`.

## 4) Kiểm tra stack đang chạy

```powershell
docker compose --project-directory .\docker-composes --env-file .\docker-composes\production\.env -f .\docker-composes\production\docker-compose.yaml ps
```

Tất cả container nên ở trạng thái `Up` (Postgres/Redis/RabbitMQ cần healthy trước khi các service phụ thuộc lên đầy đủ).

## 5) Test nhanh từ Docker Desktop + trình duyệt

Mở Docker Desktop và kiểm tra:
- Containers của project `production` đã lên đầy đủ.
- Không có container nào restart liên tục.

Mở trình duyệt:
- APIGateway: `http://localhost:8080`
- Grafana: `http://localhost:3000`
- Prometheus: `http://localhost:9090`
- Jaeger UI: `http://localhost:16686`
- RabbitMQ Management: `http://localhost:15672`

## 6) Smoke test bằng PowerShell

Kiểm tra metrics endpoint:

```powershell
Invoke-WebRequest http://localhost:8080/metrics -UseBasicParsing
Invoke-WebRequest http://localhost:9090/-/healthy -UseBasicParsing
Invoke-WebRequest http://localhost:3100/ready -UseBasicParsing
```

Nếu cần xem log:

```powershell
docker compose --project-directory .\docker-composes --env-file .\docker-composes\production\.env -f .\docker-composes\production\docker-compose.yaml logs -f --tail 200
```

## 7) Dừng stack

```powershell
.\docker-composes\production\down.ps1
```

Lệnh trên sẽ `down --remove-orphans`.

## 8) Một số lỗi thường gặp

- Docker daemon chưa chạy: mở Docker Desktop trước khi run script.
- Port bị trùng: đổi các biến `*_PORT` trong `.env`.
- Build fail do thiếu resource: tăng CPU/RAM cho Docker Desktop.
