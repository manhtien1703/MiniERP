# Hướng dẫn tạo Connection String cho Railway

## Dựa trên các biến MySQL của bạn

Từ các biến bạn đã cung cấp:

```
MYSQL_DATABASE="railway"
MYSQL_ROOT_PASSWORD="IDNqjCNyxxpOcMYtuytsKqwbyGrZVOIG"
MYSQLHOST="${{RAILWAY_PRIVATE_DOMAIN}}"
MYSQLPORT="3306"
MYSQLUSER="root"
```

## Connection String cần tạo:

### Bước 1: Lấy giá trị thực tế của MYSQLHOST

1. Vào Railway Dashboard
2. Click vào MySQL service
3. Tab **"Variables"**
4. Tìm `MYSQLHOST` hoặc `RAILWAY_PRIVATE_DOMAIN`
5. Copy giá trị thực tế (KHÔNG copy `${{RAILWAY_PRIVATE_DOMAIN}}`, mà copy giá trị mà nó resolve thành)

Ví dụ giá trị thực tế có thể là:
- `containers-us-west-123.railway.app`
- `xxx.railway.internal`
- Hoặc một domain khác

### Bước 2: Tạo Connection String

**Format:**
```
server={MYSQLHOST};port={MYSQLPORT};database={MYSQLDATABASE};user={MYSQLUSER};password={MYSQLPASSWORD};
```

**Với giá trị của bạn:**
```
ConnectionStrings__DefaultConnection=server={GIÁ_TRỊ_MYSQLHOST_THỰC_TẾ};port=3306;database=railway;user=root;password=IDNqjCNyxxpOcMYtuytsKqwbyGrZVOIG;
```

**Ví dụ cụ thể (thay {GIÁ_TRỊ_MYSQLHOST_THỰC_TẾ} bằng giá trị thực tế):**
```
ConnectionStrings__DefaultConnection=server=containers-us-west-123.railway.app;port=3306;database=railway;user=root;password=IDNqjCNyxxpOcMYtuytsKqwbyGrZVOIG;
```

### Bước 3: Thêm vào Backend Service

**⚠️ QUAN TRỌNG:** Connection string được thêm vào **BACKEND SERVICE**, KHÔNG phải MySQL service!

1. **Quay lại Railway Dashboard** (click vào tên project ở trên cùng)
2. Click vào **Backend service** (service chứa code .NET của bạn, KHÔNG phải MySQL service)
   - Nếu chưa có Backend service, bạn cần deploy code từ GitHub trước (xem Bước 3 trong DEPLOY_RAILWAY.md)
3. Tab **"Variables"** (bên cạnh tab "Deployments")
4. Click **"New Variable"** hoặc **"Raw Editor"**
5. Thêm biến mới:

**Key:**
```
ConnectionStrings__DefaultConnection
```

**Value:**
```
server=containers-us-west-123.railway.app;port=3306;database=railway;user=root;password=IDNqjCNyxxpOcMYtuytsKqwbyGrZVOIG;
```

*(Nhớ thay `containers-us-west-123.railway.app` bằng giá trị thực tế của MYSQLHOST)*

## Cách 2: Sử dụng Railway Variable Reference (Nếu cả 2 services trong cùng project)

Nếu MySQL service và Backend service trong cùng một Railway project, bạn có thể reference trực tiếp:

**Key:**
```
ConnectionStrings__DefaultConnection
```

**Value:**
```
server=${{MySQL.MYSQLHOST}};port=3306;database=railway;user=root;password=${{MySQL.MYSQL_ROOT_PASSWORD}};
```

**Lưu ý:** 
- Thay `MySQL` bằng tên service MySQL thực tế của bạn
- Railway sẽ tự động resolve các giá trị khi runtime

## Kiểm tra Connection String

Sau khi thêm connection string, Railway sẽ tự động redeploy. Kiểm tra logs:

1. Vào Backend service → Tab **"Deployments"** → Click deployment mới nhất
2. Xem logs, tìm dòng: `Database connection successful. Running migrations...`
3. Nếu thấy lỗi kết nối, kiểm tra lại:
   - Format connection string đúng chưa
   - Giá trị MYSQLHOST đúng chưa
   - MySQL service đang running chưa

