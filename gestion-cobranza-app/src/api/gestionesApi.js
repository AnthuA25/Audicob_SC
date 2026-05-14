import axiosClient from "./axiosClient";

export const registrarGestionApi = async (gestion) => {
  const response = await axiosClient.post("/Gestiones", gestion);
  return response.data;
};