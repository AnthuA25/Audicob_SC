import {
  getMisClientesApi,
  getMiClienteDetalleApi,
} from "../api/misClientesApi";

export const fetchMisClientes = async (busqueda = "") => {
  return await getMisClientesApi(busqueda);
};

export const fetchMiClienteDetalle = async (id) => {
  return await getMiClienteDetalleApi(id);
};