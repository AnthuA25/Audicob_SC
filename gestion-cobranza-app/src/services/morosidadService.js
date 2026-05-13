import { getMorosidadApi, getMorosidadMetricasApi } from "../api/morosidadApi";

export const fetchMorosidad = async () => {
  const data = await getMorosidadApi();
  return data;
};

export const fetchMorosidadMetricas = async () => {
  const data = await getMorosidadMetricasApi();
  return data;
};
