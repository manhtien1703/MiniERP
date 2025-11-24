import api from './api';

export const warehouseService = {
  // Lấy tất cả warehouses
  async getAll() {
    const response = await api.get('/Warehouse');
    return response.data;
  },

  // Lấy warehouse theo ID
  async getById(id) {
    const response = await api.get(`/Warehouse/${id}`);
    return response.data;
  },

  // Tạo warehouse mới
  async create(data) {
    const response = await api.post('/Warehouse', data);
    return response.data;
  },

  // Cập nhật warehouse
  async update(id, data) {
    const response = await api.put(`/Warehouse/${id}`, data);
    return response.data;
  },

  // Xóa warehouse
  async delete(id) {
    await api.delete(`/Warehouse/${id}`);
  },
};

