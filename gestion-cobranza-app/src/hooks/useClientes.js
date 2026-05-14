import { useState, useEffect } from "react";
import {
  fetchClientes,
  crearCliente,
  actualizarCliente,
  eliminarCliente,
} from "../services/clienteService";

const mapCliente = (c) => ({
  id: c.idCliente,
  nombre: `${c.nombres ?? ""} ${c.apellidos ?? ""}`.trim(),
  nombres: c.nombres ?? "",
  apellidos: c.apellidos ?? "",
  email: c.correo ?? "",
  dni: c.dni ?? "",
  telefono: c.telefono ?? "",
  direccion: c.direccion ?? "",
  idAsesor: c.idAsesor ?? null,
  asesorAsignado: c.asesor ?? "",
  riesgo: capitalizar(c.riesgo ?? "BAJO"),
  estado: capitalizar(c.estadoCliente ?? "NUEVO"),
  deudaTotal: formatearMonto(c.deudaTotal),
  diasAtraso: c.diasAtraso ?? 0,
});

const capitalizar = (texto) =>
  texto ? texto.charAt(0).toUpperCase() + texto.slice(1).toLowerCase() : "";

const formatearMonto = (monto) => {
  const numero = Number(monto ?? 0);
  return `S/. ${numero.toLocaleString("es-PE", {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  })}`;
};

const useClientes = () => {
  const [clientes, setClientes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const cargarClientes = async (busqueda = "") => {
    try {
      setLoading(true);
      const data = await fetchClientes(busqueda);
      setClientes((data || []).map(mapCliente));
      setError("");
    } catch (err) {
      setError(
        err.response?.data?.message || "No se pudieron cargar los clientes.",
      );
      setClientes([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    cargarClientes();
  }, []);

  const agregarCliente = async (cliente) => {
    const response = await crearCliente(cliente);

    const nuevoCliente = mapCliente({
      ...(response.cliente ?? response),
      deudaTotal: response.deuda?.montoTotal ?? cliente.montoDeuda ?? 0,
      diasAtraso: response.deuda?.diasAtraso ?? 0,
    });

    setClientes((prev) => [...prev, nuevoCliente]);

    return nuevoCliente;
  };

  const editarCliente = async (id, cliente) => {
    const response = await actualizarCliente(id, cliente);
    const actualizado = response.cliente
      ? mapCliente(response.cliente)
      : mapCliente(response);

    setClientes((prev) => prev.map((c) => (c.id === id ? actualizado : c)));
    return actualizado;
  };

  const borrarCliente = async (id) => {
    await eliminarCliente(id);
    setClientes((prev) => prev.filter((c) => c.id !== id));
  };

  return {
    clientes,
    loading,
    error,
    cargarClientes,
    agregarCliente,
    editarCliente,
    borrarCliente,
  };
};

export default useClientes;
