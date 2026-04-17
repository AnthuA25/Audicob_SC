import axiosClient from "./axiosClient";

export const getAsesoresApi = async (busqueda = "") => {
  const response = await axiosClient.get("/Usuarios/asesores", {
    params: busqueda ? { busqueda } : {},
  });
  return response.data;
};

export const getAsesorByIdApi = async (id) => {
  const response = await axiosClient.get(`/Usuarios/asesores/${id}`);
  return response.data;
};

export const crearAsesorApi = async (asesor) => {
  const response = await axiosClient.post("/Usuarios/asesores", asesor);
  return response.data;
};

export const actualizarAsesorApi = async (id, asesor) => {
  const response = await axiosClient.put(`/Usuarios/asesores/${id}`, asesor);
  return response.data;
};

export const eliminarAsesorApi = async (id) => {
  const response = await axiosClient.delete(`/Usuarios/asesores/${id}`);
  return response.data;
};