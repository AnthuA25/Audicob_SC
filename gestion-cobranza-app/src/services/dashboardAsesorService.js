import {
  getDashboardAsesorMetricas,
  getDashboardAsesorDistribucion,
  getDashboardAsesorClasificacion,
  getDashboardAsesorMorosidad,
} from "../api/dashboardAsesorApi";

export const fetchDashboardAsesorMetricas = async () => {
  return await getDashboardAsesorMetricas();
};

export const fetchDashboardAsesorDistribucion = async () => {
  return await getDashboardAsesorDistribucion();
};

export const fetchDashboardAsesorClasificacion = async () => {
  return await getDashboardAsesorClasificacion();
};

export const fetchDashboardAsesorMorosidad = async () => {
  return await getDashboardAsesorMorosidad();
};