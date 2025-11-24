import api from './api';

export const provinceService = {
  // Lấy tất cả provinces
  async getAll() {
    const response = await api.get('/provinces');
    return response.data;
  },
};

