import { useEffect, useState } from "react";
import {
  fetchPagosPorAsesor,
  fetchResumenPagos,
} from "../services/pagosService";

const usePagos = (idAsesor) => {
  const [pagos, setPagos] = useState([]);
  const [resumen, setResumen] = useState({
    totalPagosHoy: 0,
    transaccionesHoy: 0,
    totalPagosSemana: 0,
    transaccionesSemana: 0,
    totalPagosMes: 0,
    transaccionesMes: 0,
  });
  const [loading, setLoading] = useState(true);

  const cargarPagos = async () => {
    if (!idAsesor) return;

    try {
      setLoading(true);

      const [pagosData, resumenData] = await Promise.all([
        fetchPagosPorAsesor(idAsesor),
        fetchResumenPagos(idAsesor),
      ]);

      setPagos(pagosData || []);
      setResumen(resumenData || {});
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    cargarPagos();
  }, [idAsesor]);

  return { pagos, resumen, loading, cargarPagos };
};

export default usePagos;