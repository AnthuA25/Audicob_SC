import {
  getDashboardMetricas,
  getCobranzaEvolucion,
  getDistribucionClientes,
  getRendimientoAsesores,
} from "../api/dashboardApi";

export const fetchMetricas = async () => {
  return await getDashboardMetricas();
};

export const fetchCobranzaEvolucion = async () => {
  return await getCobranzaEvolucion();
};

export const fetchDistribucionClientes = async () => {
  return await getDistribucionClientes();
};

export const fetchRendimientoAsesores = async () => {
  return await getRendimientoAsesores();
};