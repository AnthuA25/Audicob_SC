import { useEffect, useState } from "react";
import {
  fetchDashboardAsesorMetricas,
  fetchDashboardAsesorDistribucion,
  fetchDashboardAsesorClasificacion,
  fetchDashboardAsesorMorosidad,
} from "../services/dashboardAsesorService";

const useDashboardAsesor = () => {
  const [metricas, setMetricas] = useState(null);
  const [distribucionClientes, setDistribucionClientes] = useState([]);
  const [clasificacionDeudores, setClasificacionDeudores] = useState([]);
  const [tendenciaMorosidad, setTendenciaMorosidad] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const cargarDatos = async () => {
      try {
        const [m, d, c, t] = await Promise.all([
          fetchDashboardAsesorMetricas(),
          fetchDashboardAsesorDistribucion(),
          fetchDashboardAsesorClasificacion(),
          fetchDashboardAsesorMorosidad(),
        ]);

        setMetricas(m);
        setDistribucionClientes(d);
        setClasificacionDeudores(c);
        setTendenciaMorosidad(t);
      } catch (err) {
        setError(err.response?.data?.message || "Error al cargar dashboard del asesor.");
      } finally {
        setLoading(false);
      }
    };

    cargarDatos();
  }, []);

  return {
    metricas,
    distribucionClientes,
    clasificacionDeudores,
    tendenciaMorosidad,
    loading,
    error,
  };
};

export default useDashboardAsesor;