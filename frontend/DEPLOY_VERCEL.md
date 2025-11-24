# 🚀 HƯỚNG DẪN DEPLOY LÊN VERCEL

## ⚠️ LƯU Ý QUAN TRỌNG

**Vercel chỉ hỗ trợ deploy FRONTEND (React).**
- ✅ **Frontend (React)** → Deploy trên **Vercel**
- ❌ **Backend (.NET 8)** → Không hỗ trợ trên Vercel
- 🔄 **Backend** → Cần deploy trên nền tảng khác (Azure, Railway, Render, AWS, etc.)

---

## 📋 BƯỚC 1: CHUẨN BỊ BACKEND

Trước khi deploy frontend, bạn cần deploy backend trước:

### Option 1: Azure App Service (Khuyến nghị)
1. Tạo Azure App Service
2. Deploy .NET backend
3. Lấy URL backend (VD: `https://minierp-api.azurewebsites.net`)

### Option 2: Railway
1. Kết nối GitHub repo
2. Deploy .NET backend
3. Lấy URL backend (VD: `https://minierp-api.railway.app`)

### Option 3: Render
1. Tạo Web Service
2. Deploy .NET backend
3. Lấy URL backend (VD: `https://minierp-api.onrender.com`)

**Lưu ý:** Backend cần:
- CORS cho phép domain Vercel của bạn
- Database MySQL đã được setup
- Environment variables đã được cấu hình

---

## 📦 BƯỚC 2: DEPLOY FRONTEND LÊN VERCEL

### Cách 1: Deploy qua Vercel CLI (Khuyến nghị)

1. **Cài đặt Vercel CLI:**
   ```bash
   npm install -g vercel
   ```

2. **Đăng nhập Vercel:**
   ```bash
   vercel login
   ```

3. **Di chuyển vào thư mục frontend:**
   ```bash
   cd frontend
   ```

4. **Deploy:**
   ```bash
   vercel
   ```

5. **Theo hướng dẫn:**
   - Set up and deploy? **Y**
   - Which scope? Chọn account của bạn
   - Link to existing project? **N**
   - Project name? `minierp-frontend`
   - Directory? `./`
   - Override settings? **N**

6. **Thêm Environment Variable:**
   ```bash
   vercel env add VITE_API_BASE_URL
   ```
   - Value: URL backend của bạn (VD: `https://minierp-api.azurewebsites.net/api`)
   - Environment: Production, Preview, Development (chọn tất cả)

7. **Deploy production:**
   ```bash
   vercel --prod
   ```

### Cách 2: Deploy qua GitHub

1. **Push code lên GitHub:**
   ```bash
   git init
   git add .
   git commit -m "Initial commit"
   git remote add origin https://github.com/yourusername/minierp.git
   git push -u origin main
   ```

2. **Truy cập Vercel Dashboard:**
   - Vào https://vercel.com
   - Click **"Add New Project"**
   - Import GitHub repository của bạn

3. **Cấu hình Project:**
   - **Framework Preset:** Vite
   - **Root Directory:** `frontend`
   - **Build Command:** `npm run build`
   - **Output Directory:** `dist`

4. **Thêm Environment Variables:**
   - Vào **Settings** > **Environment Variables**
   - Thêm:
     - **Name:** `VITE_API_BASE_URL`
     - **Value:** URL backend của bạn (VD: `https://minierp-api.azurewebsites.net/api`)
     - **Environment:** Production, Preview, Development (chọn tất cả)

5. **Deploy:**
   - Click **"Deploy"**
   - Chờ build xong
   - Vercel sẽ tự động deploy mỗi khi bạn push code mới

---

## 🔧 BƯỚC 3: CẤU HÌNH BACKEND CORS

Sau khi có URL Vercel, cần cập nhật CORS trong backend:

1. **Cập nhật `src/Web/Program.cs`:**
   ```csharp
   builder.Services.AddCors(options =>
   {
       options.AddPolicy("AllowFrontend", policy =>
       {
           policy.WithOrigins(
               "http://localhost:5173",
               "http://localhost:5174",
               "http://localhost:5175",
               "http://localhost:5177",
               "https://your-vercel-app.vercel.app" // Thêm URL Vercel của bạn
           )
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials();
       });
   });
   ```

2. **Redeploy backend** với CORS mới

---

## 📝 BƯỚC 4: KIỂM TRA

1. **Truy cập URL Vercel** (VD: `https://minierp-frontend.vercel.app`)
2. **Kiểm tra:**
   - ✅ Login hoạt động
   - ✅ API calls thành công
   - ✅ Real-time monitoring hoạt động
   - ✅ Image upload hoạt động

---

## 🐛 TROUBLESHOOTING

### Lỗi: API calls failed (CORS)
- **Giải pháp:** Kiểm tra CORS trong backend đã bao gồm URL Vercel chưa

### Lỗi: 401 Unauthorized
- **Giải pháp:** Kiểm tra `VITE_API_BASE_URL` đã đúng chưa

### Lỗi: Build failed
- **Giải pháp:** Kiểm tra:
  - `package.json` có đúng scripts chưa
  - `vite.config.js` đã đúng chưa
  - Dependencies đã install đủ chưa

---

## 📚 TÀI LIỆU THAM KHẢO

- [Vercel Documentation](https://vercel.com/docs)
- [Vite Deployment Guide](https://vitejs.dev/guide/static-deploy.html#vercel)
- [Vercel CLI](https://vercel.com/docs/cli)

---

## 🎯 NEXT STEPS

Sau khi deploy thành công:

1. ✅ Share link demo với nhà tuyển dụng
2. ✅ Cập nhật README với link live demo
3. ✅ Thêm vào CV/Portfolio

**Good luck! 🚀**


