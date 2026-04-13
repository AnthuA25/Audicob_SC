import { NavLink, useNavigate } from "react-router-dom";
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
          Dashboard
        </NavLink>
        <NavLink
          to={ROUTES.ASESORES}
          className={({ isActive }) =>
            isActive ? "sidebar-link active" : "sidebar-link"
          }
        >
          Asesores
        </NavLink>
        <NavLink
          to={ROUTES.CLIENTES}
          className={({ isActive }) =>
            isActive ? "sidebar-link active" : "sidebar-link"
          }
        >
          Clientes
        </NavLink>
        <NavLink
          to={ROUTES.MOROSIDAD}
          className={({ isActive }) =>
            isActive ? "sidebar-link active" : "sidebar-link"
          }
        >
          Morosidad
        </NavLink>
        <NavLink
          to={ROUTES.ALERTAS}
          className={({ isActive }) =>
            isActive ? "sidebar-link active" : "sidebar-link"
          }
        >
          Alertas
        </NavLink>
        <NavLink
          to={ROUTES.REPORTES}
          className={({ isActive }) =>
            isActive ? "sidebar-link active" : "sidebar-link"
          }
        >
          Reportes
        </NavLink>
      </nav>

      <button className="sidebar-logout" onClick={handleLogout}>
        Cerrar Sesión
      </button>
    </div>
  );
};

export default Sidebar;
