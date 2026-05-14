import { useEffect, useState } from "react";
import { fetchMisClientes } from "../services/misClientesService";

const capitalizar = (texto) =>
  texto ? texto.charAt(0).toUpperCase() + texto.slice(1).toLowerCase() : "";

const mapCliente = (c) => ({
  id: c.idCliente,
  nombre: `${c.nombres ?? ""} ${c.apellidos ?? ""}`.trim(),
  nombres: c.nombres ?? "",
  apellidos: c.apellidos ?? "",
  email: c.correo ?? "",
  dni: c.dni ?? "",
  telefono: c.telefono ?? "",
  direccion: c.direccion ?? "",
  asesorAsignado: c.asesor ?? "",
  riesgo: capitalizar(c.riesgo ?? "BAJO"),
  estado: capitalizar(c.estadoCliente ?? "NUEVO"),
  diasAtraso: c.diasAtraso ?? "",
  deudaPendiente:
    c.deudaPendiente !== null && c.deudaPendiente !== undefined
      ? `S/. ${Number(c.deudaPendiente).toLocaleString()}`
      : "-",
});

const useMisClientes = () => {
  const [clientes, setClientes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const cargarClientes = async (busqueda = "") => {
    try {
      setLoading(true);
      const data = await fetchMisClientes(busqueda);
      setClientes((data || []).map(mapCliente));
      setError("");
    } catch (err) {
      setError(err.response?.data?.message || "Error al cargar mis clientes.");
      setClientes([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    cargarClientes();
  }, []);

  return {
    clientes,
    loading,
    error,
    cargarClientes,
  };
};

export default useMisClientes;
