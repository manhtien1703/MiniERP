import { useEffect, useState } from 'react';
import Layout from '../components/Layout';
import { warehouseService } from '../services/warehouseService';
import { deviceService } from '../services/deviceService';
import './Dashboard.css';

const Dashboard = () => {
  const [stats, setStats] = useState({
    totalWarehouses: 0,
    totalDevices: 0,
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadStats();
  }, []);

  const loadStats = async () => {
    try {
      const [warehouses, devices] = await Promise.all([
        warehouseService.getAll(),
        deviceService.getAll(),
      ]);
      
      setStats({
        totalWarehouses: warehouses.length,
        totalDevices: devices.length,
      });
    } catch (error) {
      console.error('Error loading stats:', error);
    } finally {
      setLoading(false);
    }
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
      <div className="dashboard">
        <h1>Dashboard</h1>
        
        <div className="stats-grid">
          <div className="stat-card">
            <div className="stat-icon">🏭</div>
            <div className="stat-info">
              <div className="stat-value">{stats.totalWarehouses}</div>
              <div className="stat-label">Kho lạnh</div>
            </div>
          </div>

          <div className="stat-card">
            <div className="stat-icon">❄️</div>
            <div className="stat-info">
              <div className="stat-value">{stats.totalDevices}</div>
              <div className="stat-label">Thiết bị</div>
            </div>
          </div>
        </div>

        <div className="welcome-section">
          <h2>Chào mừng đến với MiniERP!</h2>
          <p>Hệ thống quản lý kho lạnh và thiết bị làm lạnh</p>
        </div>
      </div>
    </Layout>
  );
};

export default Dashboard;

