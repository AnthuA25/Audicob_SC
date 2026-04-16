import {
  getClientesApi,
  crearClienteApi,
  actualizarClienteApi,
  eliminarClienteApi,
  getAsesoresApi,
} from "../api/clienteApi";

export const fetchClientes = async (busqueda = "") => {
  return await getClientesApi(busqueda);
};

export const crearCliente = async (cliente) => {
  return await crearClienteApi(cliente);
};

export const actualizarCliente = async (id, cliente) => {
  return await actualizarClienteApi(id, cliente);
};

export const eliminarCliente = async (id) => {
  return await eliminarClienteApi(id);
};

export const fetchAsesores = async () => {
  return await getAsesoresApi();
};