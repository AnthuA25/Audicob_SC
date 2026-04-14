import {
  getClientesApi,
  crearClienteApi,
  actualizarClienteApi,
  eliminarClienteApi,
} from "../api/clienteApi";

export const fetchClientes = async () => {
  const data = await getClientesApi();
  return data;
};

export const crearCliente = async (cliente) => {
  const data = await crearClienteApi(cliente);
  return data;
};

export const actualizarCliente = async (id, cliente) => {
  const data = await actualizarClienteApi(id, cliente);
  return data;
};

export const eliminarCliente = async (id) => {
  const data = await eliminarClienteApi(id);
  return data;
};
