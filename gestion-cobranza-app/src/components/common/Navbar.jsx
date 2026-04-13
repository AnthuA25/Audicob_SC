import useAuth from "../../hooks/useAuth";

const Navbar = () => {
  const { user } = useAuth();

  return (
    <div className="navbar">
      <div className="navbar-brand">
        <span className="navbar-title">Sistema de Gestión de Cobranza</span>
        <span className="navbar-subtitle">Audicob</span>
      </div>

      <div className="navbar-search">
        <input type="text" placeholder="Buscar clientes..." />
      </div>

      <div className="navbar-actions">
        <div className="navbar-notif">🔔</div>
        <div className="navbar-user">
          <span className="navbar-username">{user?.nombre || "Usuario"}</span>
          <span className="navbar-rol">{user?.rol || "Rol"}</span>
        </div>
      </div>
    </div>
  );
};

export default Navbar;
