import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import './Navbar.css';

const Navbar = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="navbar">
      <div className="navbar-brand">
        <Link to="/">🏭 MiniERP</Link>
      </div>
      
      <div className="navbar-menu">
        <Link to="/" className="nav-link">Dashboard</Link>
        <Link to="/warehouse" className="nav-link">Kho lạnh</Link>
        <Link to="/devices" className="nav-link">Thiết bị</Link>
        <Link to="/monitoring" className="nav-link">Giám sát</Link>
      </div>

      <div className="navbar-user">
        <span className="user-name">👤 {user?.fullName}</span>
        <button onClick={handleLogout} className="btn-logout">
          Đăng xuất
        </button>
      </div>
    </nav>
  );
};

export default Navbar;

