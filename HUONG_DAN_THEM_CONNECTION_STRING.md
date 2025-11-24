# Hướng dẫn Thêm Connection String vào Railway

## ⚠️ Quan trọng: Connection String được thêm vào BACKEND SERVICE, không phải MySQL service!

Bạn hiện đang xem **MySQL service** (có tab "Database", "Backups", "Variables"...). Connection string cần được thêm vào **Backend service** (service chứa code .NET của bạn).

## Các bước cụ thể:

### Bước 1: Đảm bảo đã tạo Backend Service

Bạn cần có **2 services** trong Railway project:
1. ✅ **MySQL service** (bạn đã có - đang xem)
2. ❓ **Backend service** (service chứa code .NET từ GitHub)

**Nếu chưa có Backend service:**

1. Quay lại **Railway Dashboard** (click vào tên project ở trên cùng)
2. Click **"New"** → Chọn **"GitHub Repo"**
3. Chọn repository GitHub chứa code MiniERP của bạn
4. Railway sẽ tự động detect Dockerfile và build
5. Chờ deploy xong (có thể mất vài phút)

### Bước 2: Lấy thông tin từ MySQL Service

1. **Vẫn ở MySQL service** (service bạn đang xem)
2. Click tab **"Variables"** (bên cạnh tab "Database")
3. Copy các giá trị sau:
   - `MYSQLHOST` → Copy giá trị (ví dụ: `containers-us-west-123.railway.app`)
   - `MYSQLPORT` → Thường là `3306`
   - `MYSQLDATABASE` → Thường là `railway`
   - `MYSQLUSER` → Thường là `root`
   - `MYSQL_ROOT_PASSWORD` hoặc `MYSQLPASSWORD` → Copy mật khẩu

**Lưu ý:** Nếu thấy `${{RAILWAY_PRIVATE_DOMAIN}}` trong MYSQLHOST, bạn cần:
- Tìm biến `RAILWAY_PRIVATE_DOMAIN` trong cùng tab Variables
- Hoặc click vào tab "Connect" để xem connection string

### Bước 3: Thêm Connection String vào Backend Service

1. **Quay lại Railway Dashboard** (click vào tên project)
2. Click vào **Backend service** (service chứa code .NET, KHÔNG phải MySQL)
3. Click tab **"Variables"** (tương tự như MySQL service)
4. Click nút **"New Variable"** hoặc **"Raw Editor"**
5. Thêm biến mới:

   **Key (tên biến):**
   ```
   ConnectionStrings__DefaultConnection
   ```
   *(Chú ý: dùng `__` (double underscore) không phải `:`)*

   **Value (giá trị):**
   ```
   server={MYSQLHOST};port=3306;database=railway;user=root;password={MYSQLPASSWORD};
   ```

   **Ví dụ cụ thể:**
   ```
   server=containers-us-west-123.railway.app;port=3306;database=railway;user=root;password=IDNqjCNyxxpOcMYtuytsKqwbyGrZVOIG;
   ```

6. Click **"Add"** hoặc **"Save"**

### Bước 4: Kiểm tra

1. Sau khi thêm biến, Railway sẽ tự động redeploy Backend service
2. Vào tab **"Deployments"** để xem quá trình deploy
3. Click vào deployment mới nhất → xem **Logs**
4. Tìm dòng: `Database connection successful. Running migrations...`
5. Nếu thấy lỗi, kiểm tra lại connection string

## Hình ảnh minh họa:

```
Railway Dashboard
├── Project Name
│   ├── [MySQL Service] ← Bạn đang ở đây
│   │   ├── Tab: Database (hiện tại)
│   │   ├── Tab: Variables ← Lấy thông tin ở đây
│   │   └── Tab: Connect (xem connection info)
│   │
│   └── [Backend Service] ← Cần thêm biến ở đây
│       ├── Tab: Deployments
│       ├── Tab: Variables ← Thêm ConnectionStrings__DefaultConnection ở đây
│       └── Tab: Settings
```

## Nếu không thấy Backend service:

### Cách 1: Deploy từ GitHub
1. Railway Dashboard → Click **"New"**
2. Chọn **"GitHub Repo"**
3. Chọn repository của bạn
4. Railway sẽ tự động build và deploy

### Cách 2: Deploy từ Dockerfile
1. Railway Dashboard → Click **"New"**
2. Chọn **"Empty Service"**
3. Vào Settings → Connect GitHub repo
4. Railway sẽ detect Dockerfile và build

## Troubleshooting:

### "Tôi không thể thêm biến vào Backend service"
- Đảm bảo bạn đang ở **Backend service**, không phải MySQL service
- Kiểm tra bạn có quyền edit service không
- Thử refresh trang và thử lại

### "Tôi không thấy Backend service"
- Bạn cần deploy code lên Railway trước
- Xem lại "Nếu không thấy Backend service" ở trên

### "Connection string không hoạt động"
- Kiểm tra format đúng chưa (dùng `;` để phân cách)
- Đảm bảo giá trị MYSQLHOST là giá trị thực tế, không phải `${{...}}`
- Xem logs của Backend service để biết lỗi cụ thể

## Tóm tắt nhanh:

1. ✅ **MySQL service** → Tab "Variables" → Copy thông tin
2. ✅ **Backend service** → Tab "Variables" → Thêm `ConnectionStrings__DefaultConnection`
3. ✅ Railway tự động redeploy
4. ✅ Kiểm tra logs

