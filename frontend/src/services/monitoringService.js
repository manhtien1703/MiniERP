import api from './api';

export const monitoringService = {
  // Lấy log mới nhất của device
  async getLatest(deviceId) {
    const response = await api.get(`/Monitoring/${encodeURIComponent(deviceId)}/latest`);
    return response.data;
  },

  // Lấy lịch sử logs
  async getHistory(deviceId, from, to) {
    const response = await api.get(`/Monitoring/${encodeURIComponent(deviceId)}/history`, {
      params: {
        from: from.toISOString(),
        to: to.toISOString(),
      },
    });
    return response.data;
  },

  // Lấy dữ liệu chart đã được aggregate
  async getChartData(deviceId, timeRange = '24h') {
    const response = await api.get(`/Monitoring/${encodeURIComponent(deviceId)}/chart`, {
      params: { timeRange },
    });
    return response.data;
  },
};

