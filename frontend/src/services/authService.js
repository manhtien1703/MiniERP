import api from './api';

export const authService = {
  // Đăng nhập
  async login(username, password) {
    const response = await api.post('/Auth/login', {
      username,
      password,
    });
    
    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('user', JSON.stringify({
        username: response.data.username,
        fullName: response.data.fullName,
      }));
    }
    
    return response.data;
  },

  // Đăng xuất
  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  },

  // Lấy thông tin user hiện tại
  getCurrentUser() {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  },

  // Kiểm tra đã đăng nhập chưa
  isAuthenticated() {
    return !!localStorage.getItem('token');
  },
};

