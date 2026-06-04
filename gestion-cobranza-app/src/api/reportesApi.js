import axiosClient from "./axiosClient";

export const getReporteAdminApi = async () => {
  const response = await axiosClient.get("/reportes/admin");
  return response.data;
};

export const getReporteAsesorApi = async () => {
  const response = await axiosClient.get("/reportes/asesor");
  return response.data;
};

export const generarReporteAdminApi = async (data) => {
  const response = await axiosClient.post("/reportes/admin/generar", data);
  return response.data;
};

export const generarReporteAsesorApi = async (data) => {
  const response = await axiosClient.post("/reportes/asesor/generar", data);
  return response.data;
};