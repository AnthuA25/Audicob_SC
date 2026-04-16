import { useState, useEffect } from "react";
import {
  fetchClientes,
  crearCliente,
  actualizarCliente,
  eliminarCliente,
} from "../services/clienteService";

const useClientes = () => {
  const [clientes, setClientes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const cargarClientes = async () => {
    try {
      const data = await fetchClientes();
      setClientes(data);
    } catch (err) {
      setClientes(clientesEjemplo);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    cargarClientes();
  }, []);

  const agregarCliente = async (cliente) => {
    try {
      const nuevo = await crearCliente(cliente);
      setClientes((prev) => [...prev, nuevo]);
    } catch {
      const nuevo = { ...cliente, id: Date.now() };
      setClientes((prev) => [...prev, nuevo]);
    }
  };

  const editarCliente = async (id, cliente) => {
    try {
      const actualizado = await actualizarCliente(id, cliente);
      setClientes((prev) => prev.map((c) => (c.id === id ? actualizado : c)));
    } catch {
      setClientes((prev) =>
        prev.map((c) => (c.id === id ? { ...c, ...cliente } : c)),
      );
    }
  };

  const borrarCliente = async (id) => {
    try {
      await eliminarCliente(id);
    } catch {}
    setClientes((prev) => prev.filter((c) => c.id !== id));
  };

  return {
    clientes,
    loading,
    error,
    agregarCliente,
    editarCliente,
    borrarCliente,
  };
};

const clientesEjemplo = [
  {
    id: 1,
    nombre: "Laura Martínez",
    email: "maria.lopez@audicob.com",
    telefono: "+51 555 123 4567",
    asesorAsignado: "Carlos Rodríguez",
    deudaPendiente: "S./ 2,000",
    diasAtraso: 45,
    riesgo: "Alto",
    estado: "Contactado",
  },
  {
    id: 2,
    nombre: "Josmer Jauregui",
    email: "maria.lopez@audicob.com",
    telefono: "+51 555 123 4567",
    asesorAsignado: "María López",
    deudaPendiente: "S./ 5,500",
    diasAtraso: 20,
    riesgo: "Medio",
    estado: "Negociación",
  },
  {
    id: 3,
    nombre: "Janett Mendez",
    email: "maria.lopez@audicob.com",
    telefono: "+51 555 123 4567",
    asesorAsignado: "María López",
    deudaPendiente: "S./ 3,000",
    diasAtraso: 10,
    riesgo: "Bajo",
    estado: "Promesa de Pago",
  },
  {
    id: 4,
    nombre: "Karla Santos",
    email: "maria.lopez@audicob.com",
    telefono: "+51 555 123 4567",
    asesorAsignado: "Carlos Rodríguez",
    deudaPendiente: "S./ 0",
    diasAtraso: 0,
    riesgo: "Bajo",
    estado: "Pagado",
  },
  {
    id: 5,
    nombre: "Karen Silva",
    email: "maria.lopez@audicob.com",
    telefono: "+51 555 123 4567",
    asesorAsignado: "Carlos Rodríguez",
    deudaPendiente: "S./ 3,500",
    diasAtraso: 90,
    riesgo: "Alto",
    estado: "Moroso",
  },
  {
    id: 6,
    nombre: "Karen Silva",
    email: "maria.lopez@audicob.com",
    telefono: "+51 555 123 4567",
    asesorAsignado: "Carlos Rodríguez",
    deudaPendiente: "S./ 800",
    diasAtraso: 60,
    riesgo: "Alto",
    estado: "Contactado",
  },
];

export default useClientes;