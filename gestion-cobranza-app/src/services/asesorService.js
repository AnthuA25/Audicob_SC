import {
  getAsesoresApi,
  crearAsesorApi,
  actualizarAsesorApi,
  eliminarAsesorApi,
} from "../api/asesorApi";

export const fetchAsesores = async (busqueda = "") => {
  return await getAsesoresApi(busqueda);
};

export const crearAsesor = async (asesor) => {
  return await crearAsesorApi(asesor);
};

export const actualizarAsesor = async (id, asesor) => {
  return await actualizarAsesorApi(id, asesor);
};

export const eliminarAsesor = async (id) => {
  return await eliminarAsesorApi(id);
};