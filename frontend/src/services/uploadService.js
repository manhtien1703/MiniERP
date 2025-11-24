import api from './api';

export const uploadService = {
  // Upload ảnh thiết bị
  async uploadDeviceImage(file) {
    const formData = new FormData();
    formData.append('file', file);

    // Axios sẽ tự động set Content-Type với boundary khi gửi FormData
    const response = await api.post('/upload/device-image', formData);

    return response.data;
  },

  // Xóa ảnh thiết bị
  async deleteDeviceImage(fileName) {
    const response = await api.delete(`/upload/device-image/${fileName}`);
    return response.data;
  },
};
