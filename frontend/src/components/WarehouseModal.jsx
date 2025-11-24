import { useState, useEffect } from 'react';
import { warehouseService } from '../services/warehouseService';
import './Modal.css';

const WarehouseModal = ({ warehouse, provinces, onClose, onSaved }) => {
  const [formData, setFormData] = useState({
    name: '',
    location: '',
    capacity: '',
    provinceId: '',
  });
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (warehouse) {
      setFormData({
        name: warehouse.name,
        location: warehouse.location,
        capacity: warehouse.capacity,
        provinceId: warehouse.provinceId,
      });
    } else if (provinces.length > 0) {
      setFormData(prev => ({ ...prev, provinceId: provinces[0].id }));
    }
  }, [warehouse, provinces]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      const payload = {
        name: formData.name,
        location: formData.location,
        capacity: parseInt(formData.capacity),
        provinceId: parseInt(formData.provinceId),
      };

      if (warehouse) {
        // Update
        await warehouseService.update(warehouse.id, payload);
        alert('Cập nhật kho lạnh thành công!');
      } else {
        // Create - không cần ID, backend tự động tạo
        await warehouseService.create(payload);
        alert('Thêm kho lạnh thành công!');
      }
      onSaved();
    } catch (error) {
      console.error('Error saving warehouse:', error);
      alert(error.response?.data?.detail || 'Có lỗi xảy ra');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>{warehouse ? '✏️ Sửa kho lạnh' : '➕ Thêm kho lạnh mới'}</h2>
          <button className="btn-close" onClick={onClose}>×</button>
        </div>

        <form onSubmit={handleSubmit}>
          {warehouse && (
            <div className="form-group">
              <label>ID kho</label>
              <input
                type="text"
                value={warehouse.id}
                disabled
                readOnly
              />
              <small>ID tự động tạo, không thể thay đổi</small>
            </div>
          )}

          <div className="form-group">
            <label>Tên kho *</label>
            <input
              type="text"
              value={formData.name}
              onChange={(e) => setFormData({...formData, name: e.target.value})}
              placeholder="VD: Kho lạnh Hà Nội 1"
              required
            />
          </div>

          <div className="form-group">
            <label>Địa điểm *</label>
            <input
              type="text"
              value={formData.location}
              onChange={(e) => setFormData({...formData, location: e.target.value})}
              placeholder="VD: 123 Đường Láng, Đống Đa, Hà Nội"
              required
            />
          </div>

          <div className="form-group">
            <label>Sức chứa (tấn) *</label>
            <input
              type="number"
              value={formData.capacity}
              onChange={(e) => setFormData({...formData, capacity: e.target.value})}
              placeholder="VD: 500"
              min="1"
              required
            />
          </div>

          <div className="form-group">
            <label>Tỉnh/Thành phố *</label>
            <select
              value={formData.provinceId}
              onChange={(e) => setFormData({...formData, provinceId: e.target.value})}
              required
            >
              {provinces.map(province => (
                <option key={province.id} value={province.id}>
                  {province.name}
                </option>
              ))}
            </select>
          </div>

          <div className="modal-actions">
            <button type="button" className="btn-cancel" onClick={onClose}>
              Hủy
            </button>
            <button type="submit" className="btn-submit" disabled={loading}>
              {loading ? 'Đang lưu...' : (warehouse ? 'Cập nhật' : 'Thêm mới')}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default WarehouseModal;

