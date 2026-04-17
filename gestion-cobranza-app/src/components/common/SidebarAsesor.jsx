import { NavLink, useNavigate } from "react-router-dom";
import {
  LayoutDashboard,
  Users,
  CreditCard,
  Bell,
  FileText,
  LogOut,
} from "lucide-react";
import useAuth from "../../hooks/useAuth";
import { ROUTES } from "../../constants/routes";

const SidebarAsesor = () => {
  const { logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate(ROUTES.LOGIN);
  };

  return (
    <div className="sidebar">
      <nav className="sidebar-nav">
        <NavLink
          to={ROUTES.DASHBOARD_ASESOR}
          className={({ isActive }) =>
            isActive ? "sidebar-link active" : "sidebar-link"
          }
        >
          <LayoutDashboard size={18} /> Dashboard
        </NavLink>

        <NavLink
          to={ROUTES.MIS_CLIENTES}
          className={({ isActive }) =>
            isActive ? "sidebar-link active" : "sidebar-link"
          }
        >
          <Users size={18} /> Mis clientes
        </NavLink>

        <NavLink
          to={ROUTES.PAGOS}
          className={({ isActive }) =>
            isActive ? "sidebar-link active" : "sidebar-link"
          }
        >
          <CreditCard size={18} /> Pagos
        </NavLink>

        <NavLink
          to={ROUTES.ALERTAS}
          className={({ isActive }) =>
            isActive ? "sidebar-link active" : "sidebar-link"
          }
        >
          <Bell size={18} /> Alertas
        </NavLink>

        <NavLink
          to={ROUTES.REPORTES}
          className={({ isActive }) =>
            isActive ? "sidebar-link active" : "sidebar-link"
          }
        >
          <FileText size={18} /> Reportes
        </NavLink>
      </nav>

      <button className="sidebar-logout" onClick={handleLogout}>
        <LogOut size={18} /> Cerrar Sesión
      </button>
    </div>
  );
};

export default SidebarAsesor;