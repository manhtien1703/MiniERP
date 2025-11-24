import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import Layout from '../components/Layout';
import { warehouseService } from '../services/warehouseService';
import { deviceService } from '../services/deviceService';
import DeviceModal from '../components/DeviceModal';
import { getImageUrl } from '../services/api';
import './WarehouseDetail.css';

const deviceTypeMap = {
  0: 'Cooler',
  1: 'Freezer',
  2: 'Dehumidifier',
};

const WarehouseDetail = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [warehouse, setWarehouse] = useState(null);
  const [devices, setDevices] = useState([]);
  const [filteredDevices, setFilteredDevices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showDeviceModal, setShowDeviceModal] = useState(false);
  const [editingDevice, setEditingDevice] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    loadWarehouseAndDevices();
  }, [id]);

  const loadWarehouseAndDevices = async () => {
    try {
      const [warehouseData, allDevices] = await Promise.all([
        warehouseService.getById(id),
        deviceService.getAll(),
      ]);
      
      setWarehouse(warehouseData);
      // Lọc devices thuộc warehouse này
      const warehouseDevices = allDevices.filter(d => d.warehouseId === id);
      setDevices(warehouseDevices);
      setFilteredDevices(warehouseDevices);
    } catch (error) {
      console.error('Error loading data:', error);
      alert('Không thể tải thông tin kho lạnh');
    } finally {
      setLoading(false);
    }
  };

  const handleAddDevice = () => {
    setEditingDevice(null);
    setShowDeviceModal(true);
  };

  const handleEditDevice = (device) => {
    setEditingDevice(device);
    setShowDeviceModal(true);
  };

  const handleDeleteDevice = async (deviceId) => {
    if (!confirm('Bạn có chắc muốn xóa thiết bị này?')) return;

    try {
      await deviceService.delete(deviceId);
      await loadWarehouseAndDevices();
      alert('Xóa thiết bị thành công!');
    } catch (error) {
      console.error('Error deleting device:', error);
      alert('Không thể xóa thiết bị');
    }
  };

  const handleDeviceSaved = () => {
    setShowDeviceModal(false);
    loadWarehouseAndDevices();
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
      deviceTypeMap[device.deviceType]?.toLowerCase().includes(term)
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

  if (!warehouse) {
    return (
      <Layout>
        <div className="error-message">Không tìm thấy kho lạnh</div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="warehouse-detail-page">
        <div className="page-header">
          <button className="btn-back" onClick={() => navigate('/warehouse')}>
            ← Quay lại
          </button>
          <h1>🏭 {warehouse.name}</h1>
        </div>

        <div className="warehouse-info-card">
          <div className="info-row">
            <span className="label">ID:</span>
            <span className="value">{warehouse.id}</span>
          </div>
          <div className="info-row">
            <span className="label">Địa điểm:</span>
            <span className="value">📍 {warehouse.location}</span>
          </div>
          <div className="info-row">
            <span className="label">Tỉnh/TP:</span>
            <span className="value">🗺️ {warehouse.provinceName}</span>
          </div>
          <div className="info-row">
            <span className="label">Sức chứa:</span>
            <span className="value">{warehouse.capacity} tấn</span>
          </div>
        </div>

        <div className="devices-section">
          <div className="section-header">
            <h2>❄️ Thiết bị ({devices.length})</h2>
            <button className="btn-add" onClick={handleAddDevice}>
              + Thêm thiết bị
            </button>
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
              <div className="empty-devices">
                <p>Chưa có thiết bị nào trong kho này</p>
                <button className="btn-primary" onClick={handleAddDevice}>
                  + Thêm thiết bị đầu tiên
                </button>
              </div>
            ) : (
              <div className="empty-devices">
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
                        src={getImageUrl(device.imageUrl)} 
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
                  <p className="device-type">{deviceTypeMap[device.deviceType]}</p>
                  <p className={`device-status ${device.status ? 'active' : 'inactive'}`}>
                    {device.status ? '🟢 Hoạt động' : '🔴 Ngừng'}
                  </p>
                  
                  <div className="device-actions">
                    <button 
                      className="btn-edit"
                      onClick={() => handleEditDevice(device)}
                    >
                      ✏️ Sửa
                    </button>
                    <button 
                      className="btn-delete"
                      onClick={() => handleDeleteDevice(device.id)}
                    >
                      🗑️ Xóa
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>

      {showDeviceModal && (
        <DeviceModal
          device={editingDevice}
          warehouseId={id}
          onClose={() => setShowDeviceModal(false)}
          onSaved={handleDeviceSaved}
        />
      )}
    </Layout>
  );
};

export default WarehouseDetail;

