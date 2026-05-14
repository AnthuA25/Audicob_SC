import { useState, useEffect } from "react";
import { fetchDashboardMorosidad } from "../services/morosidadService";

const useMorosidad = () => {
  const [morosidad, setMorosidad] = useState([]);
  const [metricas, setMetricas] = useState({
    clientesMorosos: 0,
    deudaMorosaTotal: "S/. 0",
    morosidadCritica: 0,
    promedioAtraso: "0 días",
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let activo = true;

    const cargar = async () => {
      try {
        const data = await fetchDashboardMorosidad();

        if (!activo) return;

        setMorosidad(data.morosidad);
        setMetricas(data.metricas);
      } catch(err) {
        if (!activo) return;

        console.error("Error al cargar morosidad:", err);
        setError("No se pudo cargar la información de morosidad.");
      } finally {
        if (activo) setLoading(false);
      }
    };
    cargar();
    
    return () =>{
      activo = false;
    }
  }, []);

  return { morosidad, metricas, loading, error };
};

export default useMorosidad;
