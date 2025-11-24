import api from './api';

export const monitoringService = {
  // Lấy log mới nhất của device
  async getLatest(deviceId) {
    const response = await api.get(`/Monitoring/${deviceId}/latest`);
    return response.data;
  },

  // Lấy lịch sử logs
  async getHistory(deviceId, from, to) {
    const response = await api.get(`/Monitoring/${deviceId}/history`, {
      params: {
        from: from.toISOString(),
        to: to.toISOString(),
      },
    });
    return response.data;
  },
};

