import {
  getPagosPorAsesorApi,
  getResumenPagosApi,
  registrarPagoApi,
} from "../api/pagosApi";

export const fetchPagosPorAsesor = async (idAsesor) => {
  return await getPagosPorAsesorApi(idAsesor);
};

export const fetchResumenPagos = async (idAsesor) => {
  return await getResumenPagosApi(idAsesor);
};

export const registrarPago = async (pago) => {
  return await registrarPagoApi(pago);
};