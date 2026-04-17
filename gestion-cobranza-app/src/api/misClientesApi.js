import axiosClient from "./axiosClient";

export const getMisClientesApi = async (busqueda = "") => {
  const response = await axiosClient.get("/Clientes/mis-clientes", {
    params: busqueda ? { busqueda } : {},
  });
  return response.data;
};

export const getMiClienteDetalleApi = async (id) => {
  const response = await axiosClient.get(`/Clientes/mis-clientes/${id}`);
  return response.data;
};