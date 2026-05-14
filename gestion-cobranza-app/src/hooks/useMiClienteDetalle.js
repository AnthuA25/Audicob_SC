import { useEffect, useState } from "react";
import { fetchMiClienteDetalle } from "../services/misClientesService";

const useMiClienteDetalle = (id) => {
  const [cliente, setCliente] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const cargarDetalle = async () => {
    try {
      setLoading(true);
      const data = await fetchMiClienteDetalle(id);
      setCliente(data);
      setError("");
    } catch (err) {
      setError(
        err.response?.data?.message || "Error al cargar el detalle del cliente."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (id) cargarDetalle();
  }, [id]);

  return { cliente, loading, error, cargarDetalle };
};

export default useMiClienteDetalle;