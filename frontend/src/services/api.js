import axios from 'axios';

// Sử dụng environment variable, fallback về localhost nếu không có
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5001/api';

// Tạo axios instance
const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor để tự động thêm token vào header
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    // Nếu là FormData (file upload), không set Content-Type (axios sẽ tự động set với boundary)
    if (config.data instanceof FormData) {
      delete config.headers['Content-Type'];
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Interceptor để xử lý lỗi 401 (Unauthorized)
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token hết hạn hoặc không hợp lệ
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;

// Export API_BASE_URL để các component khác có thể sử dụng (ví dụ: build image URLs)
// Lưu ý: API_BASE_URL có dạng "http://localhost:5001/api", nhưng cho images cần base URL không có "/api"
export const getBaseUrl = () => {
  const apiUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5001/api';
  // Nếu có "/api" ở cuối, loại bỏ nó
  if (apiUrl.endsWith('/api')) {
    return apiUrl.slice(0, -4); // Loại bỏ "/api"
  }
  return apiUrl.replace(/\/api\/?$/, ''); // Loại bỏ "/api" hoặc "/api/"
};

// Helper function để build image URL từ relative path
export const getImageUrl = (imagePath) => {
  if (!imagePath) return null;
  // Nếu đã là absolute URL (bắt đầu với http:// hoặc https://), trả về nguyên
  if (imagePath.startsWith('http://') || imagePath.startsWith('https://')) {
    return imagePath;
  }
  // Nếu là relative path, thêm base URL
  const baseUrl = getBaseUrl();
  // Đảm bảo có "/" giữa baseUrl và imagePath
  const path = imagePath.startsWith('/') ? imagePath : `/${imagePath}`;
  return `${baseUrl}${path}`;
};

