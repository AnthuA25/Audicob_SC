import { Search, ShieldCheck } from "lucide-react";
import useAuth from "../../hooks/useAuth";
import NotificacionesBell from "../notificaciones/NotificacionesBell";

const Navbar = () => {
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
        <NotificacionesBell rol={user?.rol} />
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