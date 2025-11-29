# MiniERP - Hệ thống Quản lý Kho Lạnh

Hệ thống quản lý kho lạnh với các tính năng quản lý kho, thiết bị, và giám sát nhiệt độ/độ ẩm theo thời gian thực.

## 🎯 Tính năng

- Quản lý kho lạnh (CRUD)
- Quản lý thiết bị IoT với upload ảnh
- Giám sát nhiệt độ/độ ẩm real-time
- Authentication với JWT
- Lưu trữ ảnh trên MinIO

## 🛠️ Công nghệ

**Backend:** .NET 8.0, ASP.NET Core Web API, Entity Framework Core, MySQL, JWT, MinIO  
**Frontend:** React 19, Vite, Axios  
**Storage:** MySQL 8.0, MinIO (S3-compatible)

## 📋 Yêu cầu

- .NET SDK 8.0+
- Node.js 18+
- MySQL 8.0+
- Docker Desktop

## 🚀 Cài đặt và Chạy

### 1. Clone và cài đặt

```bash
git clone <repository-url>
cd MiniERP
```

### 2. Database

Tạo database MySQL:
```sql
CREATE DATABASE MiniERP CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

Cấu hình connection string trong `src/Web/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=MiniERP;user=root;password=YOUR_PASSWORD;"
  }
}
```

### 3. Cài đặt dependencies

**Backend:**
```bash
cd src/Web
dotnet restore
```

**Frontend:**
```bash
cd frontend
npm install
```

### 4. Khởi động MinIO

```powershell
.\start-minio.ps1
```

Hoặc chạy Docker:
```bash
docker run -d --name minio -p 9000:9000 -p 9001:9001 \
  -e "MINIO_ROOT_USER=minioadmin" \
  -e "MINIO_ROOT_PASSWORD=minioadmin" \
  -v "${PWD}/minio-data:/data" \
  quay.io/minio/minio server /data --console-address ":9001"
```

### 5. Chạy dự án

**Backend:**
```bash
cd src/Web
dotnet run
```
- API: https://localhost:5001
- Swagger: https://localhost:5001/swagger

**Frontend:**
```bash
cd frontend
npm run dev
```
- UI: http://localhost:5173

## 📡 API Endpoints

### Authentication
- `POST /api/Auth/login` - Đăng nhập

### Warehouse
- `GET /api/Warehouse` - Danh sách kho
- `GET /api/Warehouse/{id}` - Chi tiết kho
- `POST /api/Warehouse` - Tạo kho
- `DELETE /api/Warehouse/{id}` - Xóa kho

### Device
- `GET /api/Device` - Danh sách thiết bị
- `GET /api/Device/{id}` - Chi tiết thiết bị
- `POST /api/Device` - Tạo thiết bị
- `PUT /api/Device/{id}` - Cập nhật thiết bị
- `DELETE /api/Device/{id}` - Xóa thiết bị

### Upload
- `POST /api/Upload/device-image` - Upload ảnh thiết bị

### Monitoring
- `GET /api/Monitoring/{deviceId}/latest` - Dữ liệu mới nhất
- `GET /api/Monitoring/{deviceId}/history` - Lịch sử dữ liệu

Xem chi tiết tại: https://localhost:5001/swagger

## 🔐 Authentication

**Default users:**
- Username: `admin`, Password: `123456`
- Username: `user1`, Password: `123456`

Sử dụng JWT token sau khi login, thêm vào header:
```
Authorization: Bearer <token>
```

## 📁 Cấu trúc dự án

```
MiniERP/
├── src/
│   ├── Domain/          # Entities
│   ├── Application/     # Business logic
│   ├── Infrastructure/  # Data access, repositories
│   └── Web/             # API controllers
├── frontend/            # React app
├── minio-data/          # MinIO storage
└── start-minio.ps1      # MinIO startup script
```

## 📝 Lưu ý

- Database tự động migrate khi backend khởi động
- MinIO Console: http://localhost:9001 (minioadmin/minioadmin)
- Xem API documentation tại `/swagger`
- Link demo: https://youtu.be/8dI1dor2hv8
- Link demo chức năng hiển thị biểu đồ nhiệt độ & độ ẩm: https://youtu.be/5etMJYL5tpQ
