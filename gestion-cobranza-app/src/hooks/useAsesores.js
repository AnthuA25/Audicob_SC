import { useEffect, useState } from "react";
import {
  fetchAsesores,
  crearAsesor,
  actualizarAsesor,
  eliminarAsesor,
} from "../services/asesorService";

const capitalizar = (texto) =>
  texto ? texto.charAt(0).toUpperCase() + texto.slice(1).toLowerCase() : "";

const mapAsesor = (a) => ({
  id: a.idUsuario,
  idUsuario: a.idUsuario,
  nombre: `${a.nombres ?? ""} ${a.apellidos ?? ""}`.trim(),
  nombres: a.nombres ?? "",
  apellidos: a.apellidos ?? "",
  dni: a.dni ?? "",
  correo: a.correo ?? "",
  telefono: a.telefono ?? "",
  estado: capitalizar(a.estado ?? "ACTIVO"),
});

const useAsesores = () => {
  const [asesores, setAsesores] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const cargarAsesores = async (busqueda = "") => {
    try {
      setLoading(true);
      const data = await fetchAsesores(busqueda);
      setAsesores((data || []).map(mapAsesor));
      setError("");
    } catch (err) {
      setError(err.response?.data?.message || "Error al cargar asesores.");
      setAsesores([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    cargarAsesores();
  }, []);

  const agregarAsesor = async (asesor) => {
    const response = await crearAsesor(asesor);
    const nuevo = response.asesor ? mapAsesor(response.asesor) : mapAsesor(response);
    setAsesores((prev) => [...prev, nuevo]);
    return nuevo;
  };

  const editarAsesor = async (id, asesor) => {
    const response = await actualizarAsesor(id, asesor);
    const actualizado = response.asesor
      ? mapAsesor(response.asesor)
      : mapAsesor(response);

    setAsesores((prev) => prev.map((a) => (a.id === id ? actualizado : a)));
    return actualizado;
  };

  const borrarAsesor = async (id) => {
    await eliminarAsesor(id);
    setAsesores((prev) => prev.filter((a) => a.id !== id));
  };

  return {
    asesores,
    loading,
    error,
    cargarAsesores,
    agregarAsesor,
    editarAsesor,
    borrarAsesor,
  };
};

export default useAsesores;