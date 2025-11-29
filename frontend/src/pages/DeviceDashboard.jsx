import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import Layout from '../components/Layout';
import DeviceChart from '../components/DeviceChart';
import { deviceService } from '../services/deviceService';
import { monitoringService } from '../services/monitoringService';
import './DeviceDashboard.css';

const deviceTypeMap = {
  0: 'Cooler',
  1: 'Freezer',
  2: 'Dehumidifier',
};

const deviceTypeIcons = {
  0: '❄️',
  1: '🧊',
  2: '💨',
};

const DeviceDashboard = () => {
  const { deviceId } = useParams();
  const navigate = useNavigate();
  const [device, setDevice] = useState(null);
  const [chartData, setChartData] = useState([]);
  const [latestLog, setLatestLog] = useState(null);
  const [timeRange, setTimeRange] = useState('24h');
  const [loading, setLoading] = useState(true);
  const [chartLoading, setChartLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (deviceId) {
      setLoading(true);
      setError(null);
      loadDevice();
      loadLatestLog();
    }
  }, [deviceId]);

  useEffect(() => {
    if (deviceId) {
      loadChartData();
    }
  }, [deviceId, timeRange]);

  const loadDevice = async () => {
    try {
      console.log('Loading device:', deviceId);
      const data = await deviceService.getById(deviceId);
      console.log('Device loaded:', data);
      setDevice(data);
      setError(null);
    } catch (error) {
      console.error('Error loading device:', error);
      setError(error.response?.data?.detail || error.message || 'Không thể tải thông tin thiết bị');
      setDevice(null);
    } finally {
      setLoading(false);
    }
  };

  const loadLatestLog = async () => {
    try {
      const log = await monitoringService.getLatest(deviceId);
      setLatestLog(log);
    } catch (error) {
      console.error('Error loading latest log:', error);
      // Không set latestLog nếu lỗi, sẽ hiển thị "Chưa có dữ liệu"
    }
  };

  const loadChartData = async () => {
    setChartLoading(true);
    try {
      console.log('Loading chart data for timeRange:', timeRange);
      const data = await monitoringService.getChartData(deviceId, timeRange);
      console.log('Chart data received:', data);
      console.log('Number of data points:', data?.length || 0);
      setChartData(data || []);
    } catch (error) {
      console.error('Error loading chart data:', error);
      console.error('Error details:', error.response?.data || error.message);
      setChartData([]);
    } finally {
      setChartLoading(false);
    }
  };

  const formatTimestamp = (timestamp) => {
    if (!timestamp) return 'N/A';
    const date = new Date(timestamp);
    return date.toLocaleString('vi-VN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
    });
  };

  const formatNumber = (value, decimals = 2) => {
    if (value === null || value === undefined || isNaN(value)) return 'N/A';
    return Number(value).toFixed(decimals);
  };

  const getTemperatureStatus = (temp, deviceType) => {
    if (temp === null || temp === undefined) return 'unknown';

    switch (deviceType) {
      case 0: // Cooler
        if (temp >= 2 && temp <= 10) return 'normal';
        if (temp < 2 || temp > 10) return 'warning';
        return 'danger';
      case 1: // Freezer
        if (temp >= -20 && temp <= -5) return 'normal';
        if (temp < -20 || temp > -5) return 'warning';
        return 'danger';
      case 2: // Dehumidifier
        if (temp >= 15 && temp <= 25) return 'normal';
        if (temp < 15 || temp > 25) return 'warning';
        return 'danger';
      default:
        return 'normal';
    }
  };

  const getHumidityStatus = (humidity) => {
    if (humidity === null || humidity === undefined) return 'unknown';
    if (humidity >= 40 && humidity <= 70) return 'normal';
    if (humidity < 30 || humidity > 80) return 'danger';
    return 'warning';
  };

  if (loading) {
    return (
      <Layout>
        <div className="loading">Đang tải...</div>
      </Layout>
    );
  }

  if (!device && !loading) {
    return (
      <Layout>
        <div className="device-dashboard">
          <div className="error" style={{ padding: '2rem', textAlign: 'center' }}>
            <h2>❌ Không tìm thấy thiết bị</h2>
            <p>Thiết bị với ID "<strong>{deviceId}</strong>" không tồn tại hoặc đã bị xóa.</p>
            {error && (
              <p style={{ color: '#dc3545', marginTop: '0.5rem' }}>Chi tiết: {error}</p>
            )}
            <div style={{ marginTop: '1rem', display: 'flex', gap: '1rem', justifyContent: 'center' }}>
              <button
                onClick={() => navigate('/monitoring')}
                className="back-button"
              >
                ← Quay lại Monitoring
              </button>
              <button
                onClick={() => navigate('/devices')}
                className="back-button"
              >
                ← Danh sách thiết bị
              </button>
            </div>
          </div>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="device-dashboard">
        {/* Header */}
        <div className="device-header-card">
          <button onClick={() => navigate('/devices')} className="back-button">
            ← Quay lại
          </button>

          <div className="device-header-content">
            <div className="device-icon-large">
              {deviceTypeIcons[device.deviceType]}
            </div>
            <div className="device-info">
              <h1>{device.name}</h1>
              <p className="device-id">ID: {device.id}</p>
              <div className="device-meta">
                <span className="device-type">{deviceTypeMap[device.deviceType]}</span>
                <span className={`status ${device.status ? 'active' : 'inactive'}`}>
                  {device.status ? '🟢 Đang hoạt động' : '🔴 Tắt'}
                </span>
              </div>
            </div>
            {device.imageUrl && (
              <div className="device-image">
                <img src={device.imageUrl} alt={device.name} />
              </div>
            )}
          </div>
        </div>

        {/* Real-time Metrics */}
        {latestLog && (
          <div className="metrics-grid">
            <div className={`metric-card temp ${getTemperatureStatus(latestLog.temperature, device.deviceType)}`}>
              <div className="metric-icon">🌡️</div>
              <div className="metric-info">
                <div className="metric-label">Nhiệt độ</div>
                <div className="metric-value">{formatNumber(latestLog.temperature)}°C</div>
                <div className="metric-time">{formatTimestamp(latestLog.timestamp)}</div>
              </div>
            </div>

            <div className={`metric-card humidity ${getHumidityStatus(latestLog.humidity)}`}>
              <div className="metric-icon">💧</div>
              <div className="metric-info">
                <div className="metric-label">Độ ẩm</div>
                <div className="metric-value">{formatNumber(latestLog.humidity)}%</div>
                <div className="metric-time">{formatTimestamp(latestLog.timestamp)}</div>
              </div>
            </div>
          </div>
        )}

        {/* Chart Section */}
        <div className="chart-section">
          <div className="chart-header">
            <h2>📈 Biểu Đồ Nhiệt Độ & Độ Ẩm</h2>
            <div className="time-range-selector">
              {['1h', '6h', '24h', '7d', '30d'].map((range) => (
                <button
                  key={range}
                  className={`time-btn ${timeRange === range ? 'active' : ''}`}
                  onClick={() => setTimeRange(range)}
                >
                  {range === '1h' && '⏱️ 1 giờ'}
                  {range === '6h' && '⏱️ 6 giờ'}
                  {range === '24h' && '📅 24 giờ'}
                  {range === '7d' && '📆 7 ngày'}
                  {range === '30d' && '📆 30 ngày'}
                </button>
              ))}
              <button onClick={loadChartData} className="refresh-btn">
                🔄 Làm mới
              </button>
            </div>
          </div>

          <div className="chart-container">
            {chartLoading ? (
              <div className="loading">Đang tải dữ liệu biểu đồ...</div>
            ) : (
              <DeviceChart
                data={chartData}
                timeRange={timeRange}
                deviceType={device.deviceType}
              />
            )}
          </div>
        </div>
      </div>
    </Layout>
  );
};

export default DeviceDashboard;

