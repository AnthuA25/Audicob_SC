import axiosClient from "./axiosClient";

// Crear un nuevo asesor de cobranza
export const crearAsesor = async (asesorData) => {
  const response = await axiosClient.post("/Usuarios", asesorData);
  return response.data;
};

// Listar todos los asesores
export const listarAsesores = async (busqueda = "") => {
  const params = busqueda ? { busqueda } : {};
  const response = await axiosClient.get("/Usuarios/asesores", { params });
  return response.data;
};

// Obtener un asesor específico por ID
export const obtenerAsesor = async (id) => {
  const response = await axiosClient.get(`/Usuarios/${id}`);
  return response.data;
};

// Actualizar un asesor
export const actualizarAsesor = async (id, asesorData) => {
  const response = await axiosClient.put(`/Usuarios/${id}`, asesorData);
  return response.data;
};

// Eliminar un asesor
export const eliminarAsesor = async (id) => {
  const response = await axiosClient.delete(`/Usuarios/${id}`);
  return response.data;
};
