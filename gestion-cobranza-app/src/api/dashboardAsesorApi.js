import axiosClient from "./axiosClient";

export const getDashboardAsesorMetricas = async () => {
  const response = await axiosClient.get("/dashboard-asesor/metricas");
  return response.data;
};

export const getDashboardAsesorDistribucion = async () => {
  const response = await axiosClient.get("/dashboard-asesor/distribucion-clientes");
  return response.data;
};

export const getDashboardAsesorClasificacion = async () => {
  const response = await axiosClient.get("/dashboard-asesor/clasificacion-deudores");
  return response.data;
};

export const getDashboardAsesorMorosidad = async () => {
  const response = await axiosClient.get("/dashboard-asesor/tendencia-morosidad");
  return response.data;
};