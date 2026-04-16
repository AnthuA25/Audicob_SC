import { NavLink, useNavigate } from "react-router-dom";
import {
  LayoutDashboard,
  UserRound,
  Users,
  AlertTriangle,
  Bell,
  BarChart2,
  Upload,
  LogOut,
} from "lucide-react";

import useAuth from "../../hooks/useAuth";
import { ROUTES } from "../../constants/routes";

const Sidebar = () => {
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
          to={ROUTES.DASHBOARD_ADMIN}
          className={({ isActive }) =>
            isActive ? "sidebar-link active" : "sidebar-link"
          }
        >
          <LayoutDashboard size={18} /> Dashboard
        </NavLink>
        <NavLink
          to={ROUTES.ASESORES}
          className={({ isActive }) =>
            isActive ? "sidebar-link active" : "sidebar-link"
          }
        >
          <UserRound size={18} /> Asesores
        </NavLink>
        <NavLink
          to={ROUTES.CLIENTES}
          className={({ isActive }) =>
            isActive ? "sidebar-link active" : "sidebar-link"
          }
        >
          <Users size={18} /> Clientes
        </NavLink>
        <NavLink
          to={ROUTES.MOROSIDAD}
          className={({ isActive }) =>
            isActive ? "sidebar-link active" : "sidebar-link"
          }
        >
          <AlertTriangle size={18} /> Morosidad
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
          <BarChart2 size={18} /> Reportes
        </NavLink>
        <NavLink
          to={ROUTES.IMPORTAR}
          className={({ isActive }) =>
            isActive ? "sidebar-link active" : "sidebar-link"
          }
        >
          <Upload size={18} /> Importar
        </NavLink>
      </nav>

      <button className="sidebar-logout" onClick={handleLogout}>
        <LogOut size={18} /> Cerrar Sesión
      </button>
    </div>
  );
};

export default Sidebar;
