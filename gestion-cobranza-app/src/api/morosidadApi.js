import axiosClient from "./axiosClient";

export const getDashboardMorosidadApi = async () => {
  const response = await axiosClient.get("/Morosidad/dashboard");
  return response.data;
};