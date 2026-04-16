import axiosClient from "./axiosClient";

export const getDashboardMetricas = async () => {
  const response = await axiosClient.get("/dashboard/metricas");
  return response.data;
};

export const getCobranzaEvolucion = async () => {
  const response = await axiosClient.get("/dashboard/cobranza-evolucion");
  return response.data;
};

export const getDistribucionClientes = async () => {
  const response = await axiosClient.get("/dashboard/distribucion-clientes");
  return response.data;
};

export const getRendimientoAsesores = async () => {
  const response = await axiosClient.get("/dashboard/rendimiento-asesores");
  return response.data;
};