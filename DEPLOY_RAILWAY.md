# Hướng dẫn Deploy Backend lên Railway

## Yêu cầu
- Tài khoản GitHub đã có code
- Tài khoản Railway (đăng ký tại https://railway.app)

## Bước 1: Chuẩn bị Database trên Railway

1. Đăng nhập vào Railway: https://railway.app
2. Tạo project mới
3. Click **"New"** → Chọn **"Database"** → Chọn **"MySQL"**
4. Railway sẽ tự động tạo MySQL database và cung cấp connection string

## Bước 2: Lưu Connection String

Sau khi tạo MySQL database, Railway sẽ tự động tạo các biến môi trường. Bạn sẽ thấy các biến như:
- `MYSQLHOST` = RAILWAY_PRIVATE_DOMAIN
- `MYSQLPORT` = 3306
- `MYSQLDATABASE` = railway (hoặc tên bạn đặt)
- `MYSQLUSER` = root
- `MYSQLPASSWORD` = [mật khẩu tự động]

**Connection string format cho .NET:**
```
server={MYSQLHOST};port={MYSQLPORT};database={MYSQLDATABASE};user={MYSQLUSER};password={MYSQLPASSWORD};
```

## Bước 3: Deploy Backend Application

1. Trong Railway project, click **"New"** → Chọn **"GitHub Repo"**
2. Chọn repository GitHub của bạn (repository chứa code MiniERP)
3. Railway sẽ:
   - ✅ Tự động detect Dockerfile trong root directory
   - ✅ Tự động build Docker image
   - ✅ Tự động deploy application
   - ✅ **Tự động redeploy mỗi khi bạn push code mới lên GitHub** (auto-deploy)

**Lưu ý quan trọng:**
- Railway tự động kết nối với GitHub repository của bạn
- **Auto-deploy được bật mặc định** - mỗi khi bạn push code mới, Railway sẽ tự động build và deploy
- Đảm bảo Dockerfile nằm ở root của repository (hoặc chỉ định path trong Railway settings)

**Kiểm tra auto-deploy đã bật:**
- Vào Backend service → Tab **"Settings"**
- Tìm phần **"Source"** → Đảm bảo **"Auto Deploy"** đang bật (ON)
- Nếu muốn tắt auto-deploy, bạn có thể tắt ở đây

## Bước 4: Cấu hình Environment Variables

Sau khi deploy backend, vào service backend → tab **"Variables"**, thêm các biến sau:

### Bắt buộc - Connection String:

**Cách 1: Sử dụng Reference từ MySQL Service (Khuyến nghị)**

Nếu MySQL service và Backend service trong cùng một Railway project, bạn có thể reference các biến từ MySQL service:

```
ConnectionStrings__DefaultConnection=server=${{MySQL.MYSQLHOST}};port=${{MySQL.MYSQLPORT}};database=${{MySQL.MYSQLDATABASE}};user=${{MySQL.MYSQLUSER}};password=${{MySQL.MYSQLPASSWORD}};
```

**Lưu ý:** 
- Thay `MySQL` bằng tên service MySQL của bạn (nếu khác)
- Railway sử dụng dấu `__` (double underscore) thay vì `:` trong configuration key

**Cách 2: Copy giá trị trực tiếp (Nếu reference không hoạt động)**

1. Vào MySQL service → tab **"Variables"**
2. Copy các giá trị:
   - `MYSQLHOST` (ví dụ: `xxx.railway.internal`)
   - `MYSQLPORT` (thường là `3306`)
   - `MYSQLDATABASE` (ví dụ: `railway`)
   - `MYSQLUSER` (thường là `root`)
   - `MYSQLPASSWORD` (ví dụ: `IDNqjCNyxxpOcMYtuytsKqwbyGrZVOIG`)

3. Thêm vào Backend service variables:

```
ConnectionStrings__DefaultConnection=server=xxx.railway.internal;port=3306;database=railway;user=root;password=IDNqjCNyxxpOcMYtuytsKqwbyGrZVOIG;
```

**Ví dụ cụ thể với giá trị bạn đã cung cấp:**

Dựa trên các biến MySQL của bạn:
- `MYSQL_DATABASE` = `railway`
- `MYSQL_USER` = `root`
- `MYSQL_ROOT_PASSWORD` = `IDNqjCNyxxpOcMYtuytsKqwbyGrZVOIG`
- `MYSQLHOST` = `${{RAILWAY_PRIVATE_DOMAIN}}` (bạn cần xem giá trị thực tế)
- `MYSQLPORT` = `3306`

**Cách làm:**

1. **Lấy giá trị thực tế của `RAILWAY_PRIVATE_DOMAIN`:**
   - Vào MySQL service trên Railway
   - Tab **"Variables"** → Tìm `MYSQLHOST` hoặc `RAILWAY_PRIVATE_DOMAIN`
   - Copy giá trị thực tế (sẽ có dạng như: `containers-us-west-123.railway.app` hoặc `xxx.railway.internal`)

2. **Tạo connection string:**

   **Nếu sử dụng giá trị trực tiếp:**
   ```
   ConnectionStrings__DefaultConnection=server=containers-us-west-xxx.railway.app;port=3306;database=railway;user=root;password=IDNqjCNyxxpOcMYtuytsKqwbyGrZVOIG;
   ```
   
   *(Thay `containers-us-west-xxx.railway.app` bằng giá trị thực tế của `MYSQLHOST`)*

   **HOẶC nếu MySQL service có tên cụ thể (ví dụ: "MySQL"):**
   ```
   ConnectionStrings__DefaultConnection=server=${{MySQL.MYSQLHOST}};port=3306;database=railway;user=root;password=${{MySQL.MYSQL_ROOT_PASSWORD}};
   ```

3. **Thêm vào Backend service:**
   - Vào Backend service → Tab **"Variables"**
   - Click **"New Variable"**
   - Key: `ConnectionStrings__DefaultConnection`
   - Value: Connection string bạn vừa tạo
   - Click **"Add"**

**⚠️ Lưu ý quan trọng:**
- Railway có thể sử dụng `RAILWAY_PRIVATE_DOMAIN` (internal domain) hoặc một domain khác
- Nếu connection string không hoạt động, thử:
  - Kiểm tra lại giá trị `MYSQLHOST` trong MySQL service variables
  - Đảm bảo Backend và MySQL service trong cùng một Railway project
  - Xem logs của Backend service để biết lỗi kết nối cụ thể

### JWT Configuration (Bắt buộc):

```
Jwt__SecretKey=YourSuperSecretKeyForJWT_MustBeLongEnough_32Chars_Production_ChangeThis!
Jwt__Issuer=MiniERP
Jwt__Audience=MiniERPUsers
```

**⚠️ QUAN TRỌNG:** Đổi `Jwt__SecretKey` thành một giá trị bảo mật ngẫu nhiên dài ít nhất 32 ký tự!

### Optional (nếu cần):

```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
```

## Bước 5: Cấu hình CORS

Sau khi backend deploy thành công, Railway sẽ cung cấp một URL như:
`https://your-app-name.railway.app`

1. Copy URL này
2. Cập nhật CORS trong `src/Web/Program.cs` để cho phép frontend Vercel:

```csharp
policy.WithOrigins(
    "https://mini-erp-gilt.vercel.app",
    "https://your-app-name.vercel.app"  // Thêm các domain khác nếu có
)
```

Sau đó commit và push lên GitHub:
```bash
git add .
git commit -m "Update CORS configuration"
git push origin main
```

Railway sẽ **tự động detect** code mới và **tự động redeploy** (thường mất 2-5 phút).

## Bước 6: Lấy Backend URL

1. Vào service backend trên Railway
2. Tab **"Settings"** → **"Domains"**
3. Railway sẽ tự động cung cấp domain: `https://your-app-name.railway.app`
4. Hoặc bạn có thể tạo custom domain

**Backend URL sẽ là:** `https://your-app-name.railway.app`

## Bước 7: Cấu hình Frontend (Vercel)

1. Vào Vercel dashboard
2. Chọn project frontend
3. Vào **Settings** → **Environment Variables**
4. Thêm biến:

```
VITE_API_BASE_URL=https://your-app-name.railway.app/api
```

5. Redeploy frontend để áp dụng thay đổi

## Bước 8: Kiểm tra

1. Mở browser và truy cập: `https://your-app-name.railway.app/swagger`
   - Nếu thấy Swagger UI → Backend hoạt động tốt ✅

2. Mở frontend Vercel và test đăng nhập
   - Nếu đăng nhập được → Kết nối thành công ✅

## Troubleshooting

### Database Migration Failed
- Kiểm tra connection string đúng format chưa
- Kiểm tra MySQL service đã running chưa
- Xem logs trong Railway để biết lỗi cụ thể

### CORS Error
- Đảm bảo đã thêm Vercel URL vào CORS policy
- Kiểm tra URL frontend chính xác (có https:// và không có trailing slash)

### Static Files (Images) không hiển thị
- Railway có thể cần cấu hình thêm để serve static files
- Kiểm tra path `/uploads/devices/` có đúng không

### Port Error
- Railway tự động set PORT env variable
- Đảm bảo code đã cập nhật để sử dụng `$PORT`

## Lưu ý quan trọng

1. **Security:**
   - Đổi JWT Secret Key thành giá trị bảo mật
   - Không commit connection string vào git
   - Sử dụng Railway Variables để quản lý secrets

2. **Database:**
   - Railway MySQL có thể giới hạn storage, cần upgrade plan nếu cần
   - Backup database định kỳ

3. **Performance:**
   - Railway free tier có thể có giới hạn
   - Monitor resource usage

4. **Static Files:**
   - Files upload lên `/wwwroot/uploads/` sẽ bị mất khi redeploy
   - Cân nhắc sử dụng cloud storage (AWS S3, Cloudinary) cho production

## Kết quả

Sau khi hoàn thành:
- ✅ Backend API: `https://your-app-name.railway.app`
- ✅ Frontend: `https://mini-erp-gilt.vercel.app`
- ✅ Database: MySQL trên Railway
- ✅ Tất cả kết nối và hoạt động bình thường

