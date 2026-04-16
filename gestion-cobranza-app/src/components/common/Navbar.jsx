import { Search, Bell, ShieldCheck } from "lucide-react";
import useAuth from "../../hooks/useAuth";

const Navbar = ({ notificaciones = 3 }) => {
  const { user } = useAuth();

  const nombreCompleto = user
    ? `${user.nombres ?? ""} ${user.apellidos ?? ""}`.trim()
    : "Usuario";

  return (
    <div className="navbar">
      <div className="navbar-brand">
        <span className="navbar-title">Sistema de Gestión de Cobranza</span>
        <span className="navbar-subtitle">Audicob</span>
      </div>

      <div className="navbar-search">
        <Search size={16} className="navbar-search-icon" />
        <input type="text" placeholder="Buscar clientes..." />
      </div>

      <div className="navbar-actions">
        <div className="navbar-notif">
          <Bell size={20} color="#374151" />
          {notificaciones > 0 && <span className="notif-dot" />}
        </div>
        <div className="navbar-user">
          <div className="navbar-user-info">
            <span className="navbar-username">{nombreCompleto}</span>
            <span className="navbar-rol">{user?.rol || "Rol"}</span>
          </div>
          <ShieldCheck size={28} color="#7c3aed" />
        </div>
      </div>
    </div>
  );
};

export default Navbar;