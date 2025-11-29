import api from './api';

export const deviceService = {
  // Lấy tất cả devices
  async getAll() {
    const response = await api.get('/Device');
    return response.data;
  },

  // Lấy device theo ID
  async getById(id) {
    const response = await api.get(`/Device/${encodeURIComponent(id)}`);
    return response.data;
  },

  // Tạo device mới
  async create(data) {
    const response = await api.post('/Device', data);
    return response.data;
  },

  // Cập nhật device
  async update(id, data) {
    const response = await api.put(`/Device/${encodeURIComponent(id)}`, data);
    return response.data;
  },

  // Xóa device
  async delete(id) {
    await api.delete(`/Device/${encodeURIComponent(id)}`);
  },
};

