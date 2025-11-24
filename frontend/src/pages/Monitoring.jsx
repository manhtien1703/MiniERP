import { useEffect, useState } from 'react';
import Layout from '../components/Layout';
import { deviceService } from '../services/deviceService';
import { monitoringService } from '../services/monitoringService';
import './Monitoring.css';

const deviceTypeMap = {
  0: 'Cooler',
  1: 'Freezer',
  2: 'Dehumidifier',
};

const Monitoring = () => {
  const [devicesData, setDevicesData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [lastUpdate, setLastUpdate] = useState(new Date());

  useEffect(() => {
    loadDevicesWithLogs();

    // Auto refresh mỗi 5 giây
    const interval = setInterval(() => {
      loadDevicesWithLogs();
    }, 5000);

    return () => clearInterval(interval);
  }, []);

  const loadDevicesWithLogs = async () => {
    try {
      const devices = await deviceService.getAll();

      // Lấy log mới nhất cho mỗi device
      const devicesWithLogs = await Promise.all(
        devices.map(async (device) => {
          try {
            const latestLog = await monitoringService.getLatest(device.id);
            return {
              ...device,
              log: latestLog,
            };
          } catch (error) {
            return {
              ...device,
              log: null,
            };
          }
        })
      );

      setDevicesData(devicesWithLogs);

      // Tìm timestamp mới nhất từ tất cả các device logs
      const latestTimestamp = devicesWithLogs
        .map(d => d.log?.timestamp)
        .filter(Boolean)
        .sort((a, b) => new Date(b) - new Date(a))[0];

      // Nếu có dữ liệu, sử dụng timestamp mới nhất, không thì dùng thời gian hiện tại
      setLastUpdate(latestTimestamp ? new Date(latestTimestamp) : new Date());
    } catch (error) {
      console.error('Error loading monitoring data:', error);
    } finally {
      setLoading(false);
    }
  };

  const getTemperatureStatus = (temp, deviceType) => {
    // Đảm bảo temp là số
    const temperature = typeof temp === 'number' ? temp : parseFloat(temp);
    if (isNaN(temperature)) {
      console.warn('Invalid temperature:', temp);
      return 'danger';
    }

    // Xử lý deviceType có thể là số (0, 1, 2) hoặc string ("Cooler", "Freezer", "Dehumidifier")
    let type;
    if (typeof deviceType === 'number') {
      type = deviceType;
    } else if (typeof deviceType === 'string') {
      // Map string enum name to number
      const typeMap = {
        'Cooler': 0,
        'Freezer': 1,
        'Dehumidifier': 2
      };
      type = typeMap[deviceType];
      if (type === undefined) {
        // Thử parse như số nếu không match
        type = parseInt(deviceType);
      }
    } else {
      type = parseInt(deviceType);
    }

    if (isNaN(type) || (type !== 0 && type !== 1 && type !== 2)) {
      console.warn('Invalid deviceType:', deviceType, 'parsed as:', type);
      return 'danger';
    }

    if (type === 0) {
      // Cooler: 2-10°C là bình thường
      if (temperature >= 2 && temperature <= 10) return 'normal';
      // 10-15°C là cảnh báo (hơi nóng)
      if (temperature > 10 && temperature <= 15) return 'warning';
      // < 2°C (quá lạnh) hoặc > 15°C (quá nóng) là nguy hiểm
      return 'danger';
    } else if (type === 1) {
      // Freezer: -20 to -5°C là bình thường
      if (temperature >= -20 && temperature <= -5) return 'normal';
      // -5 to 0°C là cảnh báo (hơi ấm)
      if (temperature > -5 && temperature <= 0) return 'warning';
      // < -20°C (quá lạnh) hoặc > 0°C (quá ấm) là nguy hiểm
      return 'danger';
    } else if (type === 2) {
      // Dehumidifier: 15-25°C là bình thường
      if (temperature >= 15 && temperature <= 25) return 'normal';
      // 25-30°C là cảnh báo (hơi nóng)
      if (temperature > 25 && temperature <= 30) return 'warning';
      // < 15°C (quá lạnh) hoặc > 30°C (quá nóng) là nguy hiểm
      return 'danger';
    } else {
      console.warn('Unknown deviceType:', type, 'temperature:', temperature);
      return 'danger';
    }
  };

  const getHumidityStatus = (humidity) => {
    if (humidity >= 40 && humidity <= 60) return 'normal';
    if ((humidity >= 30 && humidity < 40) || (humidity > 60 && humidity <= 70)) return 'warning';
    return 'danger';
  };

  const formatTimestamp = (timestamp) => {
    if (!timestamp) return 'N/A';

    // Parse timestamp - backend trả về UTC time (có "Z")
    const date = new Date(timestamp);

    // Kiểm tra nếu date không hợp lệ
    if (isNaN(date.getTime())) return 'N/A';

    // Sử dụng Intl.DateTimeFormat để format theo múi giờ VN (UTC+7)
    const formatter = new Intl.DateTimeFormat('en-US', {
      timeZone: 'Asia/Ho_Chi_Minh',
      hour12: false,
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    });

    // Sử dụng formatToParts để lấy các phần tử riêng lẻ
    const parts = formatter.formatToParts(date);

    // Tìm các phần tử với null checking để tránh lỗi runtime
    const hourPart = parts.find(p => p.type === 'hour');
    const minutePart = parts.find(p => p.type === 'minute');
    const secondPart = parts.find(p => p.type === 'second');
    const dayPart = parts.find(p => p.type === 'day');
    const monthPart = parts.find(p => p.type === 'month');
    const yearPart = parts.find(p => p.type === 'year');

    // Nếu thiếu bất kỳ phần nào, fallback về format mặc định
    if (!hourPart || !minutePart || !secondPart || !dayPart || !monthPart || !yearPart) {
      // Fallback: sử dụng format thông thường
      return date.toLocaleString('vi-VN', {
        timeZone: 'Asia/Ho_Chi_Minh',
        hour12: false,
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit'
      });
    }

    const hour = hourPart.value;
    const minute = minutePart.value;
    const second = secondPart.value;
    const day = dayPart.value;
    const month = monthPart.value;
    const year = yearPart.value;

    // Format: HH:mm:ss DD/MM/YYYY
    return `${hour}:${minute}:${second} ${day}/${month}/${year}`;
  };

  if (loading) {
    return (
      <Layout>
        <div className="loading">Đang tải...</div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="monitoring-page">
        <div className="page-header">
          <h1>📊 Giám sát Real-time</h1>
          <div className="last-update">
            ⏱️ Cập nhật lần cuối: {formatTimestamp(lastUpdate)}
            <div className="auto-refresh">🔄 Tự động làm mới mỗi 5 giây</div>
          </div>
        </div>

        {devicesData.length === 0 ? (
          <div className="empty-state">
            <p>Chưa có thiết bị nào để giám sát</p>
          </div>
        ) : (
          <div className="monitoring-grid">
            {devicesData.map((device) => (
              <div key={device.id} className="device-monitor-card">
                <div className="device-header">
                  <div className="device-icon">
                    {device.deviceType === 0 ? '❄️' : device.deviceType === 1 ? '🧊' : '💨'}
                  </div>
                  <div className="device-info">
                    <h3>{device.name}</h3>
                    <p className="device-type">{deviceTypeMap[device.deviceType]}</p>
                    <p className="device-id">ID: {device.id}</p>
                  </div>
                  <div className={`device-status-badge ${device.status ? 'active' : 'inactive'}`}>
                    {device.status ? '🟢 ON' : '🔴 OFF'}
                  </div>
                </div>

                {device.log ? (
                  <div className="sensor-data">
                    <div className={`sensor-item temp ${getTemperatureStatus(device.log.temperature, device.deviceType)}`}>
                      <div className="sensor-icon">🌡️</div>
                      <div className="sensor-info">
                        <div className="sensor-label">Nhiệt độ</div>
                        <div className="sensor-value">{device.log.temperature}°C</div>
                      </div>
                    </div>

                    <div className={`sensor-item humidity ${getHumidityStatus(device.log.humidity)}`}>
                      <div className="sensor-icon">💧</div>
                      <div className="sensor-info">
                        <div className="sensor-label">Độ ẩm</div>
                        <div className="sensor-value">{device.log.humidity}%</div>
                      </div>
                    </div>

                    <div className="timestamp">
                      <small>📅 {formatTimestamp(device.log.timestamp)}</small>
                    </div>
                  </div>
                ) : (
                  <div className="no-data">
                    <p>⚠️ Chưa có dữ liệu cảm biến</p>
                  </div>
                )}
              </div>
            ))}
          </div>
        )}

        <div className="legend">
          <h3>Chú thích màu sắc:</h3>
          <div className="legend-items">
            <div className="legend-item">
              <span className="legend-color normal"></span>
              <span>Bình thường</span>
            </div>
            <div className="legend-item">
              <span className="legend-color warning"></span>
              <span>Cảnh báo</span>
            </div>
            <div className="legend-item">
              <span className="legend-color danger"></span>
              <span>Nguy hiểm</span>
            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
};

export default Monitoring;
