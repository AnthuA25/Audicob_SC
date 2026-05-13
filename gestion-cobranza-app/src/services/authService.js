import { loginApi } from "../api/authApi";
import { saveToken, saveUser } from "../utils/storage";
import { ROLES } from "../constants/roles";
import { ROUTES } from "../constants/routes";

export const loginService = async (dni, password) => {
  const data = await loginApi({ dni, password });

  if (!data.token || !data.user) {
    throw new Error("Respuesta de login inválida");
  }

  const user = {
    ...data.user,
    rol: data.user.rol || data.user.Rol,
  };

  saveToken(data.token);
  saveUser(user);

  if (data.user.rol === ROLES.ADMIN) {
    return ROUTES.DASHBOARD_ADMIN;
  } 

  if (data.user.rol === ROLES.ASESOR) {
    return ROUTES.DASHBOARD_ASESOR;
  }

  return ROUTES.LOGIN;
};
