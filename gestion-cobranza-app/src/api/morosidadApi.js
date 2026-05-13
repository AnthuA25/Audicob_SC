import axiosClient from "./axiosClient";

export const getMorosidadApi = async () => {
  const response = await axiosClient.get("/morosidad");
  return response.data;
};

export const getMorosidadMetricasApi = async () => {
  const response = await axiosClient.get("/morosidad/metricas");
  return response.data;
};
