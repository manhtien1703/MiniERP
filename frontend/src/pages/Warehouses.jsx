import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Layout from '../components/Layout';
import { warehouseService } from '../services/warehouseService';
import { provinceService } from '../services/provinceService';
import WarehouseModal from '../components/WarehouseModal';
import './Warehouses.css';

const Warehouses = () => {
  const navigate = useNavigate();
  const [warehouses, setWarehouses] = useState([]);
  const [filteredWarehouses, setFilteredWarehouses] = useState([]);
  const [provinces, setProvinces] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingWarehouse, setEditingWarehouse] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [warehouseData, provinceData] = await Promise.all([
        warehouseService.getAll(),
        provinceService.getAll(),
      ]);
      setWarehouses(warehouseData);
      setFilteredWarehouses(warehouseData);
      setProvinces(provinceData);
    } catch (error) {
      console.error('Error loading data:', error);
      alert('Không thể tải danh sách kho lạnh');
    } finally {
      setLoading(false);
    }
  };

  const handleAdd = () => {
    setEditingWarehouse(null);
    setShowModal(true);
  };

  const handleEdit = (e, warehouse) => {
    e.stopPropagation();
    setEditingWarehouse(warehouse);
    setShowModal(true);
  };

  const handleDelete = async (e, id) => {
    e.stopPropagation();
    if (!confirm('Bạn có chắc muốn xóa kho lạnh này?')) return;

    try {
      await warehouseService.delete(id);
      await loadData();
      alert('Xóa kho lạnh thành công!');
    } catch (error) {
      console.error('Error deleting warehouse:', error);
      alert('Không thể xóa kho lạnh');
    }
  };

  const handleSaved = () => {
    setShowModal(false);
    loadData();
  };

  const handleWarehouseClick = (id) => {
    navigate(`/warehouse/${id}`);
  };

  const handleSearch = (e) => {
    const term = e.target.value.toLowerCase();
    setSearchTerm(term);
    
    if (!term) {
      setFilteredWarehouses(warehouses);
      return;
    }

    const filtered = warehouses.filter(warehouse =>
      warehouse.name.toLowerCase().includes(term) ||
      warehouse.id.toLowerCase().includes(term) ||
      warehouse.location.toLowerCase().includes(term) ||
      warehouse.provinceName?.toLowerCase().includes(term)
    );
    setFilteredWarehouses(filtered);
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
      <div className="warehouses-page">
        <div className="page-header">
          <h1>Quản lý Kho lạnh</h1>
          <button className="btn-add" onClick={handleAdd}>
            + Thêm kho lạnh
          </button>
        </div>

        <div className="search-container">
          <input
            type="text"
            placeholder="🔍 Tìm kiếm theo tên, ID, địa điểm..."
            value={searchTerm}
            onChange={handleSearch}
            className="search-input"
          />
        </div>

        {filteredWarehouses.length === 0 ? (
          warehouses.length === 0 ? (
            <div className="empty-state">
              <p>Chưa có kho lạnh nào</p>
              <button className="btn-primary" onClick={handleAdd}>
                + Thêm kho lạnh đầu tiên
              </button>
            </div>
          ) : (
            <div className="empty-state">
              <p>Không tìm thấy kho lạnh nào phù hợp</p>
            </div>
          )
        ) : (
          <div className="warehouses-grid">
            {filteredWarehouses.map((warehouse) => (
              <div 
                key={warehouse.id} 
                className="warehouse-card"
                onClick={() => handleWarehouseClick(warehouse.id)}
              >
                <div className="warehouse-icon">🏭</div>
                <h3>{warehouse.name}</h3>
                <p className="warehouse-id">ID: {warehouse.id}</p>
                <p className="warehouse-location">📍 {warehouse.location}</p>
                <p className="warehouse-province">🗺️ {warehouse.provinceName}</p>
                <div className="warehouse-capacity">
                  <span className="label">Sức chứa:</span>
                  <span className="value">{warehouse.capacity} tấn</span>
                </div>
                
                <div className="warehouse-actions">
                  <button 
                    className="btn-edit"
                    onClick={(e) => handleEdit(e, warehouse)}
                  >
                    ✏️ Sửa
                  </button>
                  <button 
                    className="btn-delete"
                    onClick={(e) => handleDelete(e, warehouse.id)}
                  >
                    🗑️ Xóa
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {showModal && (
        <WarehouseModal
          warehouse={editingWarehouse}
          provinces={provinces}
          onClose={() => setShowModal(false)}
          onSaved={handleSaved}
        />
      )}
    </Layout>
  );
};

export default Warehouses;

