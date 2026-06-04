import { useEffect, useState } from "react";
import {
  getResumenAlertasAdminApi,
  getAlertasAdminApi,
  getResumenAlertasAsesorApi,
  getAlertasAsesorApi,
  marcarAlertaLeidaApi,
} from "../api/alertasApi";

const normalizarPrioridad = (prioridad) => {
  const valor = prioridad?.toLowerCase() || "";

  if (valor.includes("alta")) return "alta";
  if (valor.includes("media")) return "media";
  if (valor.includes("baja")) return "baja";

  return "media";
};

const obtenerIcono = (tipoAlerta) => {
  const tipo = tipoAlerta?.toLowerCase() || "";

  if (tipo.includes("pago") || tipo.includes("recordatorio")) return "calendar";
  if (tipo.includes("riesgo") || tipo.includes("morosidad")) return "alerta";

  return "file";
};

const formatearFecha = (fecha) => {
  if (!fecha) return "";

  const fechaAlerta = new Date(fecha);
  const hoy = new Date();

  const esHoy =
    fechaAlerta.toDateString() === hoy.toDateString();

  const ayer = new Date();
  ayer.setDate(hoy.getDate() - 1);

  const esAyer =
    fechaAlerta.toDateString() === ayer.toDateString();

  if (esHoy) return "Hoy";
  if (esAyer) return "Ayer";

  return fechaAlerta.toLocaleDateString("es-PE");
};

const mapearAlerta = (a) => ({
  id: a.idAlerta,
  tipo: a.tipoAlerta,
  descripcion: `${a.nombreCliente} - ${a.mensaje}`,
  prioridad: normalizarPrioridad(a.prioridad),
  fecha: formatearFecha(a.fechaAlerta),
  leida: a.leido,
  icono: obtenerIcono(a.tipoAlerta),
});

export const useAlertas = (rol = "admin") => {
  const [resumen, setResumen] = useState({
    riesgoMedio: 0,
    riesgoAlto: 0,
    critico: 0,
  });

  const [alertas, setAlertas] = useState([]);
  const [tabActiva, setTabActiva] = useState("todas");
  const [loading, setLoading] = useState(true);

  const esAdmin = rol === "admin";

  const cargarAlertas = async () => {
    try {
      setLoading(true);

      const soloNoLeidas = tabActiva === "noLeidas";

      const resumenData = esAdmin
        ? await getResumenAlertasAdminApi()
        : await getResumenAlertasAsesorApi();

      const alertasData = esAdmin
        ? await getAlertasAdminApi(soloNoLeidas)
        : await getAlertasAsesorApi(soloNoLeidas);

      setResumen(resumenData);
      setAlertas(alertasData.map(mapearAlerta));
    } catch (error) {
      console.error("Error al cargar alertas:", error);
    } finally {
      setLoading(false);
    }
  };

  const marcarComoLeida = async (idAlerta) => {
    try {
      await marcarAlertaLeidaApi(idAlerta);
      await cargarAlertas();
    } catch (error) {
      console.error("Error al marcar alerta como leída:", error);
    }
  };

  useEffect(() => {
    cargarAlertas();
  }, [tabActiva]);

  return {
    resumen,
    alertas,
    tabActiva,
    setTabActiva,
    marcarComoLeida,
    loading,
  };
};