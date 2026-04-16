import LoginForm from "../../components/forms/LoginForm";
import "../../styles/login.css";

const LoginPage = () => {
  return (
    <div className="login-page">
      <div className="login-header">
        <h1>Sistema Audicob</h1>
        <p>Gestión de Cobranza de Productos de Belleza</p>
      </div>

      <div className="login-card">
        <p className="card-title">Iniciar Sesión</p>
        <p className="card-subtitle">Ingresa tus credenciales para acceder</p>
        <LoginForm />
      </div>

      <p className="login-footer">
        © 2026 Audicob · Todos los derechos reservados
      </p>
    </div>
  );
};

export default LoginPage;
