# Railway Auto-Deploy - Tự động Deploy khi có code mới

## ✅ Railway tự động làm gì?

### 1. Auto-Detect Dockerfile
- Railway tự động tìm Dockerfile trong repository
- Nếu có Dockerfile ở root → Railway tự động sử dụng
- Nếu Dockerfile ở thư mục khác → Cần cấu hình path trong Settings

### 2. Auto-Build
- Railway tự động build Docker image khi:
  - Lần đầu connect GitHub repo
  - Mỗi khi có push mới lên GitHub
  - Khi bạn manually trigger deploy

### 3. Auto-Deploy
- ✅ **Mặc định BẬT** - Railway tự động deploy mỗi khi có code mới
- Railway theo dõi branch bạn đã chọn (thường là `main` hoặc `master`)
- Mỗi commit mới → Railway tự động build và deploy

## 🔄 Quy trình Auto-Deploy

```
1. Bạn push code lên GitHub
   ↓
2. GitHub webhook gửi thông báo cho Railway
   ↓
3. Railway tự động detect code mới
   ↓
4. Railway tự động build Docker image
   ↓
5. Railway tự động deploy application mới
   ↓
6. Application chạy với code mới
```

## ⚙️ Cấu hình Auto-Deploy

### Kiểm tra Auto-Deploy đã bật:

1. Vào Railway Dashboard
2. Click vào **Backend service**
3. Tab **"Settings"** → Tìm phần **"Source"**
4. Kiểm tra:
   - ✅ **Auto Deploy:** ON (bật) hoặc OFF (tắt)
   - **Branch:** Branch được theo dõi (thường là `main`)
   - **Root Directory:** Thư mục chứa Dockerfile (thường là `/`)

### Bật/Tắt Auto-Deploy:

**Để bật Auto-Deploy:**
- Trong phần **"Source"** → Bật toggle **"Auto Deploy"**

**Để tắt Auto-Deploy:**
- Trong phần **"Source"** → Tắt toggle **"Auto Deploy"**
- Lúc này bạn cần manually trigger deploy từ tab "Deployments"

### Thay đổi Branch theo dõi:

1. Settings → Source → **Branch**
2. Chọn branch khác (ví dụ: `develop`, `staging`)
3. Railway sẽ chỉ auto-deploy khi push vào branch này

## 🚀 Cách Trigger Deploy

### Cách 1: Auto-Deploy (Mặc định)
- Chỉ cần push code lên GitHub
- Railway tự động deploy

```bash
git add .
git commit -m "Update code"
git push origin main  # Railway tự động deploy
```

### Cách 2: Manual Deploy
- Vào Backend service → Tab **"Deployments"**
- Click nút **"Deploy"** hoặc **"Redeploy"**
- Chọn commit muốn deploy

### Cách 3: Deploy từ GitHub
- Vào GitHub repository
- Vào tab **"Actions"** (nếu có Railway integration)
- Trigger workflow

## 📊 Xem trạng thái Deploy

1. **Vào Backend service** → Tab **"Deployments"**
2. Xem danh sách các deployments:
   - ✅ **Success** - Deploy thành công
   - 🔄 **Building** - Đang build
   - ❌ **Failed** - Deploy thất bại
   - ⏳ **Queued** - Đang chờ build

3. **Click vào deployment** để xem:
   - Build logs
   - Deploy logs
   - Thời gian build/deploy
   - Lỗi (nếu có)

## 🔍 Troubleshooting

### "Railway không auto-deploy khi tôi push code"

**Kiểm tra:**
1. Auto-Deploy có đang bật không? (Settings → Source → Auto Deploy)
2. Branch bạn push có đúng branch được theo dõi không?
3. GitHub webhook có hoạt động không?
   - Settings → Source → Xem "Last Webhook Event"

**Giải pháp:**
- Kiểm tra Settings → Source → Auto Deploy phải là **ON**
- Đảm bảo push đúng branch (thường là `main`)
- Thử manual deploy một lần để test

### "Railway không detect Dockerfile"

**Kiểm tra:**
1. Dockerfile có trong repository không?
2. Dockerfile có ở root directory không?
   - Nếu không, cần chỉ định trong Settings → Root Directory

**Giải pháp:**
- Đảm bảo Dockerfile ở root của repository
- Hoặc cấu hình Root Directory trong Settings

### "Build thất bại"

**Kiểm tra:**
1. Xem logs trong Deployment
2. Kiểm tra Dockerfile có đúng syntax không
3. Kiểm tra dependencies có đủ không

**Giải pháp:**
- Xem chi tiết lỗi trong logs
- Fix lỗi trong code/Dockerfile
- Push lại code

## 💡 Best Practices

1. **Luôn kiểm tra logs** sau mỗi deployment
2. **Test trên branch riêng** trước khi merge vào main
3. **Monitor resource usage** để tránh vượt quota
4. **Backup database** trước khi deploy thay đổi lớn
5. **Sử dụng environment variables** cho các cấu hình

## 📝 Tóm tắt

- ✅ Railway **tự động detect Dockerfile**
- ✅ Railway **tự động build** khi có code mới
- ✅ Railway **tự động deploy** mỗi khi push lên GitHub (nếu bật)
- ✅ Chỉ cần push code → Railway làm phần còn lại
- ⚙️ Có thể bật/tắt auto-deploy trong Settings

