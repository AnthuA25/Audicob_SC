import { useState, useEffect } from "react";
import {
  fetchMetricas,
  fetchCobranzaEvolucion,
  fetchDistribucionClientes,
  fetchRendimientoAsesores,
} from "../services/dashboardService";

const useDashboard = () => {
  const [metricas, setMetricas] = useState(null);
  const [cobranzaEvolucion, setCobranzaEvolucion] = useState([]);
  const [distribucionClientes, setDistribucionClientes] = useState([]);
  const [rendimientoAsesores, setRendimientoAsesores] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const cargarDatos = async () => {
      try {
        const [m, c, d, r] = await Promise.all([
          fetchMetricas(),
          fetchCobranzaEvolucion(),
          fetchDistribucionClientes(),
          fetchRendimientoAsesores(),
        ]);
        setMetricas(m);
        setCobranzaEvolucion(c);
        setDistribucionClientes(d);
        setRendimientoAsesores(r);
      } catch (err) {
        setError("Error al cargar los datos del dashboard.");
      } finally {
        setLoading(false);
      }
    };

    cargarDatos();
  }, []);

  return {
    metricas,
    cobranzaEvolucion,
    distribucionClientes,
    rendimientoAsesores,
    loading,
    error,
  };
};

export default useDashboard;