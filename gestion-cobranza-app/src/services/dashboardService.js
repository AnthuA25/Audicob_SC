import {
  getDashboardMetricas,
  getCobranzaEvolucion,
  getDistribucionClientes,
  getRendimientoAsesores,
} from "../api/dashboardApi";

export const fetchMetricas = async () => {
  const data = await getDashboardMetricas();
  return data;
};

export const fetchCobranzaEvolucion = async () => {
  const data = await getCobranzaEvolucion();
  return data;
};

export const fetchDistribucionClientes = async () => {
  const data = await getDistribucionClientes();
  return data;
};

export const fetchRendimientoAsesores = async () => {
  const data = await getRendimientoAsesores();
  return data;
};