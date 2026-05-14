import { registrarGestionApi } from "../api/gestionesApi";

export const registrarGestion = async (gestion) => {
  return await registrarGestionApi(gestion);
};