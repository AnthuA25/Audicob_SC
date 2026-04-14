import axiosClient from "./axiosClient";

export const getClientesApi = async () => {
  const response = await axiosClient.get("/clientes");
  return response.data;
};

export const crearClienteApi = async (cliente) => {
  const response = await axiosClient.post("/clientes", cliente);
  return response.data;
};

export const actualizarClienteApi = async (id, cliente) => {
  const response = await axiosClient.put(`/clientes/${id}`, cliente);
  return response.data;
};

export const eliminarClienteApi = async (id) => {
  const response = await axiosClient.delete(`/clientes/${id}`);
  return response.data;
};
