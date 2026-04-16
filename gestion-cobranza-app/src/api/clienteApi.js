import axiosClient from "./axiosClient";

export const getClientesApi = async (busqueda = "") => {
  const response = await axiosClient.get("/Clientes", {
    params: busqueda ? { busqueda } : {},
  });
  return response.data;
};

export const getClienteByIdApi = async (id) => {
  const response = await axiosClient.get(`/Clientes/${id}`);
  return response.data;
};

export const crearClienteApi = async (cliente) => {
  const response = await axiosClient.post("/Clientes", cliente);
  return response.data;
};

export const actualizarClienteApi = async (id, cliente) => {
  const response = await axiosClient.put(`/Clientes/${id}`, cliente);
  return response.data;
};

export const eliminarClienteApi = async (id) => {
  const response = await axiosClient.delete(`/Clientes/${id}`);
  return response.data;
};

export const getAsesoresApi = async () => {
  const response = await axiosClient.get("/Usuarios/asesores");
  return response.data;
};