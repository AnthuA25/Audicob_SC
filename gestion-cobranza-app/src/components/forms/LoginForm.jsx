import { useState } from "react";
import useAuth from "../../hooks/useAuth";
import { loginService } from "../../services/authService";
import { saveToken, saveUser } from "../../utils/storage";
import { useNavigate } from "react-router-dom";

const LoginForm = () => {
  const [dni, setDni] = useState("");
  const [password, setPassword] = useState("");
  const [dniError, setDniError] = useState("");
  const [passError, setPassError] = useState("");
  const [alertError, setAlertError] = useState("");
  const [loading, setLoading] = useState(false);

  const { setUser, setToken } = useAuth();
  const navigate = useNavigate();

  const validate = () => {
    let valid = true;
    setDniError("");
    setPassError("");
    setAlertError("");

    if (!/^\d{8}$/.test(dni)) {
      setDniError("El DNI debe tener 8 dígitos.");
      valid = false;
    }
    if (!password) {
      setPassError("La contraseña es obligatoria.");
      valid = false;
    }
    return valid;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) return;

    setLoading(true);
    try {
      const redirectTo = await loginService(dni, password);
      const user = JSON.parse(localStorage.getItem("user"));
      const token = localStorage.getItem("token");
      setUser(user);
      setToken(token);
      navigate(redirectTo);
    } catch (error) {
      setAlertError("Credenciales incorrectas. Verifica tu DNI y contraseña.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      {alertError && <div className="alert-error visible">{alertError}</div>}

      <div className="field">
        <label htmlFor="dni">Número de DNI</label>
        <input
          type="text"
          id="dni"
          placeholder="Ej: 75452589"
          maxLength={8}
          value={dni}
          onChange={(e) => setDni(e.target.value)}
        />
        {dniError && <span className="error-msg visible">{dniError}</span>}
      </div>

      <div className="field">
        <label htmlFor="password">Contraseña</label>
        <input
          type="password"
          id="password"
          placeholder="••••••••••"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
        {passError && <span className="error-msg visible">{passError}</span>}
      </div>

      <button className="btn-login" type="submit" disabled={loading}>
        {loading ? "Verificando..." : "Iniciar Sesión"}
      </button>
    </form>
  );
};

export default LoginForm;
