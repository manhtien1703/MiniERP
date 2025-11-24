import { useState, useEffect, useRef } from 'react';
import { deviceService } from '../services/deviceService';
import { uploadService } from '../services/uploadService';
import { getImageUrl } from '../services/api';
import './Modal.css';

const DeviceModal = ({ device, warehouseId, onClose, onSaved }) => {
  const [formData, setFormData] = useState({
    name: '',
    deviceType: 0,
    status: true,
    imageUrl: '',
  });
  const [selectedFile, setSelectedFile] = useState(null);
  const [previewUrl, setPreviewUrl] = useState(null);
  const [uploading, setUploading] = useState(false);
  const [loading, setLoading] = useState(false);
  const fileInputRef = useRef(null);

  useEffect(() => {
    if (device) {
      setFormData({
        name: device.name,
        deviceType: device.deviceType,
        status: device.status,
        imageUrl: device.imageUrl || '',
      });
      if (device.imageUrl) {
        setPreviewUrl(getImageUrl(device.imageUrl));
      }
    }
  }, [device]);

  const handleFileChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      // Kiểm tra loại file
      if (!file.type.startsWith('image/')) {
        alert('Vui lòng chọn file ảnh');
        return;
      }

      // Kiểm tra kích thước (5MB)
      if (file.size > 5 * 1024 * 1024) {
        alert('File quá lớn. Kích thước tối đa: 5MB');
        return;
      }

      setSelectedFile(file);
      
      // Tạo preview URL
      const reader = new FileReader();
      reader.onloadend = () => {
        setPreviewUrl(reader.result);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleUploadImage = async () => {
    if (!selectedFile) return;

    setUploading(true);
    try {
      const response = await uploadService.uploadDeviceImage(selectedFile);
      setFormData({ ...formData, imageUrl: response.url });
      setSelectedFile(null);
      alert('Upload ảnh thành công!');
    } catch (error) {
      console.error('Error uploading image:', error);
      alert(error.response?.data?.error || 'Lỗi khi upload ảnh');
    } finally {
      setUploading(false);
    }
  };

  const handleRemoveImage = () => {
    setSelectedFile(null);
    setPreviewUrl(formData.imageUrl ? getImageUrl(formData.imageUrl) : null);
    setFormData({ ...formData, imageUrl: '' });
    if (fileInputRef.current) {
      fileInputRef.current.value = '';
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      let imageUrl = formData.imageUrl || null;

      // Nếu có file mới được chọn nhưng chưa upload, tự động upload trước
      if (selectedFile && !imageUrl) {
        setUploading(true);
        try {
          const response = await uploadService.uploadDeviceImage(selectedFile);
          imageUrl = response.url;
          setFormData({ ...formData, imageUrl: imageUrl });
          setSelectedFile(null);
        } catch (error) {
          console.error('Error uploading image:', error);
          alert(error.response?.data?.error || 'Lỗi khi upload ảnh. Vui lòng thử lại.');
          setLoading(false);
          setUploading(false);
          return;
        } finally {
          setUploading(false);
        }
      }

      const payload = {
        name: formData.name,
        deviceType: formData.deviceType,
        warehouseId: warehouseId,
        imageUrl: imageUrl,
      };

      if (device) {
        // Update
        await deviceService.update(device.id, {
          name: formData.name,
          deviceType: formData.deviceType,
          status: formData.status,
          imageUrl: imageUrl,
        });
        alert('Cập nhật thiết bị thành công!');
      } else {
        // Create - không cần ID, backend tự động tạo
        await deviceService.create(payload);
        alert('Thêm thiết bị thành công!');
      }
      onSaved();
    } catch (error) {
      console.error('Error saving device:', error);
      alert(error.response?.data?.detail || error.response?.data?.error || 'Có lỗi xảy ra');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>{device ? '✏️ Sửa thiết bị' : '➕ Thêm thiết bị mới'}</h2>
          <button className="btn-close" onClick={onClose}>×</button>
        </div>

        <form onSubmit={handleSubmit}>
          {device && (
            <div className="form-group">
              <label>ID thiết bị</label>
              <input
                type="text"
                value={device.id}
                disabled
                readOnly
              />
              <small>ID tự động tạo, không thể thay đổi</small>
            </div>
          )}

          <div className="form-group">
            <label>Tên thiết bị *</label>
            <input
              type="text"
              value={formData.name}
              onChange={(e) => setFormData({...formData, name: e.target.value})}
              placeholder="VD: Máy làm lạnh HN-01"
              required
            />
          </div>

          <div className="form-group">
            <label>Loại thiết bị *</label>
            <select
              value={formData.deviceType}
              onChange={(e) => setFormData({...formData, deviceType: parseInt(e.target.value)})}
              required
            >
              <option value={0}>Cooler (Làm lạnh)</option>
              <option value={1}>Freezer (Đông lạnh)</option>
              <option value={2}>Dehumidifier (Hút ẩm)</option>
            </select>
          </div>

          <div className="form-group">
            <label>Ảnh thiết bị</label>
            <input
              ref={fileInputRef}
              type="file"
              accept="image/*"
              onChange={handleFileChange}
              style={{ marginBottom: '0.5rem' }}
            />
            <small>Chọn ảnh từ thiết bị (JPG, PNG, GIF, WEBP - tối đa 5MB)</small>
            
            {previewUrl && (
              <div className="image-preview">
                <img src={previewUrl} alt="Preview" onError={(e) => e.target.style.display = 'none'} />
                <div className="image-actions">
                  {selectedFile && !formData.imageUrl && (
                    <small style={{color: '#666', fontStyle: 'italic'}}>
                      💡 Ảnh sẽ được tự động upload khi bạn lưu thiết bị
                    </small>
                  )}
                  <button
                    type="button"
                    onClick={handleRemoveImage}
                    className="btn-remove-image"
                  >
                    🗑️ Xóa ảnh
                  </button>
                </div>
              </div>
            )}
            
            {!previewUrl && formData.imageUrl && (
              <div className="image-preview">
                <img 
                  src={getImageUrl(formData.imageUrl)} 
                  alt="Current" 
                  onError={(e) => e.target.style.display = 'none'} 
                />
                <div className="image-actions">
                  <button
                    type="button"
                    onClick={handleRemoveImage}
                    className="btn-remove-image"
                  >
                    🗑️ Xóa ảnh
                  </button>
                </div>
              </div>
            )}
          </div>

          <div className="form-group">
            <label className="checkbox-label">
              <input
                type="checkbox"
                checked={formData.status}
                onChange={(e) => setFormData({...formData, status: e.target.checked})}
              />
              <span>Hoạt động</span>
            </label>
          </div>

          <div className="modal-actions">
            <button type="button" className="btn-cancel" onClick={onClose}>
              Hủy
            </button>
            <button type="submit" className="btn-submit" disabled={loading}>
              {loading ? 'Đang lưu...' : (device ? 'Cập nhật' : 'Thêm mới')}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default DeviceModal;

