import axiosClient from "./axiosClient";

export const getPagosPorAsesorApi = async (idAsesor) => {
  const response = await axiosClient.get(`/Pagos/asesor/${idAsesor}`);
  return response.data;
};

export const getResumenPagosApi = async (idAsesor) => {
  const response = await axiosClient.get(`/Pagos/asesor/${idAsesor}/resumen`);
  return response.data;
};

export const registrarPagoApi = async (pago) => {
  const response = await axiosClient.post("/Pagos/registrar", pago);
  return response.data;
};