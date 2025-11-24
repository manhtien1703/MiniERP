import { useEffect, useState } from 'react';
import Layout from '../components/Layout';
import { deviceService } from '../services/deviceService';
import './Devices.css';

const deviceTypeMap = {
  0: 'Cooler',
  1: 'Freezer',
  2: 'Dehumidifier',
};

const Devices = () => {
  const [devices, setDevices] = useState([]);
  const [filteredDevices, setFilteredDevices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    loadDevices();
  }, []);

  const loadDevices = async () => {
    try {
      const data = await deviceService.getAll();
      setDevices(data);
      setFilteredDevices(data);
    } catch (error) {
      console.error('Error loading devices:', error);
      alert('Không thể tải danh sách thiết bị');
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = (e) => {
    const term = e.target.value.toLowerCase();
    setSearchTerm(term);
    
    if (!term) {
      setFilteredDevices(devices);
      return;
    }

    const filtered = devices.filter(device =>
      device.name.toLowerCase().includes(term) ||
      device.id.toLowerCase().includes(term) ||
      deviceTypeMap[device.deviceType]?.toLowerCase().includes(term) ||
      device.warehouseName?.toLowerCase().includes(term)
    );
    setFilteredDevices(filtered);
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
      <div className="devices-page">
        <div className="page-header">
          <h1>Quản lý Thiết bị</h1>
        </div>

        <div className="search-container">
          <input
            type="text"
            placeholder="🔍 Tìm kiếm thiết bị..."
            value={searchTerm}
            onChange={handleSearch}
            className="search-input"
          />
        </div>

        {filteredDevices.length === 0 ? (
          devices.length === 0 ? (
            <div className="empty-state">
              <p>Chưa có thiết bị nào</p>
            </div>
          ) : (
            <div className="empty-state">
              <p>Không tìm thấy thiết bị nào phù hợp</p>
            </div>
          )
        ) : (
          <div className="devices-grid">
            {filteredDevices.map((device) => (
              <div key={device.id} className="device-card">
                <div className="device-image-container">
                  {device.imageUrl ? (
                    <img 
                      src={device.imageUrl.startsWith('http') 
                        ? device.imageUrl 
                        : `https://localhost:5001${device.imageUrl}`} 
                      alt={device.name} 
                      className="device-image" 
                      onError={(e) => {
                        e.target.style.display = 'none';
                        if (e.target.nextSibling) {
                          e.target.nextSibling.style.display = 'block';
                        }
                      }} 
                    />
                  ) : null}
                  <div className="device-icon" style={{display: device.imageUrl ? 'none' : 'block'}}>❄️</div>
                  <div className={`status-dot ${device.status ? 'active' : 'inactive'}`}></div>
                </div>
                <h3>{device.name}</h3>
              <p className="device-id">ID: {device.id}</p>
              <div className="device-info">
                <div className="info-row">
                  <span className="label">Loại:</span>
                  <span className="value">{deviceTypeMap[device.deviceType]}</span>
                </div>
                <div className="info-row">
                  <span className="label">Trạng thái:</span>
                  <span className={`status ${device.status ? 'active' : 'inactive'}`}>
                    {device.status ? '🟢 Hoạt động' : '🔴 Ngừng'}
                  </span>
                </div>
                {device.warehouseName && (
                  <div className="info-row">
                    <span className="label">Kho:</span>
                    <span className="value">{device.warehouseName}</span>
                  </div>
                )}
              </div>
            </div>
          ))}
          </div>
        )}
      </div>
    </Layout>
  );
};

export default Devices;

