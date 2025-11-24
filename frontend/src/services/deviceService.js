import api from './api';

export const deviceService = {
  // Lấy tất cả devices
  async getAll() {
    const response = await api.get('/device');
    return response.data;
  },

  // Lấy device theo ID
  async getById(id) {
    const response = await api.get(`/device/${id}`);
    return response.data;
  },

  // Tạo device mới
  async create(data) {
    const response = await api.post('/device', data);
    return response.data;
  },

  // Cập nhật device
  async update(id, data) {
    const response = await api.put(`/device/${id}`, data);
    return response.data;
  },

  // Xóa device
  async delete(id) {
    await api.delete(`/device/${id}`);
  },
};

