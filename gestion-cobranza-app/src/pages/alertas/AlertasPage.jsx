import {
  AlertCircle,
  AlertTriangle,
  XCircle,
  Calendar,
  FileText,
} from "lucide-react";
import { useAlertas } from "../../hooks/useAlertas";
import "../../styles/alertas.css";


const getIcono = (tipo) => {
  if (tipo === "calendar") return <Calendar size={16} color="#3b82f6" />;
  if (tipo === "alerta") return <AlertTriangle size={16} color="#f59e0b" />;
  return <FileText size={16} color="#64748b" />;
};

const AlertasPage = () => {
  const {
    resumen,
    alertas,
    tabActiva,
    setTabActiva,
    marcarComoLeida,
    loading,
  } = useAlertas("admin");

  const noLeidas = alertas.filter((a) => !a.leida);


  return (
    <div>
      <div className="alertas-header">
        <h1>Alertas y notificaciones</h1>
        <p>Gestiona recordatorios y avisos importantes</p>
      </div>

      <div className="alertas-metricas">
        <div className="alerta-metric-card">
          <div className="alerta-metric-icon medio">
            <AlertCircle size={18} color="#f59e0b" />
          </div>
          <div className="alerta-metric-info">
            <p>Riesgo Medio</p>
            <div className="alerta-metric-valor">{loading ? "..." : resumen.riesgoMedio}</div>
            <div className="alerta-metric-sub">5-15 dias de atraso</div>
          </div>
        </div>
        <div className="alerta-metric-card">
          <div className="alerta-metric-icon alto">
            <AlertTriangle size={18} color="#f97316" />
          </div>
          <div className="alerta-metric-info">
            <p>Riesgo Alto</p>
            <div className="alerta-metric-valor">{loading ? "..." : resumen.riesgoAlto}</div>
            <div className="alerta-metric-sub">16-30 dias de atraso</div>
          </div>
        </div>
        <div className="alerta-metric-card">
          <div className="alerta-metric-icon critico">
            <XCircle size={18} color="#ef4444" />
          </div>
          <div className="alerta-metric-info">
            <p>Crítico</p>
            <div className="alerta-metric-valor">{loading ? "..." : resumen.critico}</div>
            <div className="alerta-metric-sub">+30 dias de atraso</div>
          </div>
        </div>
      </div>

      <div className="alertas-tabs">
        <button
          className={`alerta-tab ${tabActiva === "noLeidas" ? "activo" : ""}`}
          onClick={() => setTabActiva("noLeidas")}
        >
          No Leídas ({loading ? "..." : noLeidas.length})
        </button>
        <button
          className={`alerta-tab ${tabActiva === "todas" ? "activo" : ""}`}
          onClick={() => setTabActiva("todas")}
        >
          Todas ({loading ? "..." : alertas.length})
        </button>
      </div>

      <div className="alertas-lista">
        {alertas.map((alerta) => (
          <div className="alerta-item" key={alerta.id} onClick={() => !alerta.leida && marcarComoLeida(alerta.id)}>
            <div className={`alerta-borde ${alerta.prioridad}`} />
            <div className="alerta-icono">{getIcono(alerta.icono)}</div>
            <div className="alerta-contenido">
              <strong>{alerta.tipo}</strong>
              <span>{alerta.descripcion}</span>
              <span className={`alerta-badge ${alerta.prioridad}`}>
                Prioridad{" "}
                {alerta.prioridad.charAt(0).toUpperCase() +
                  alerta.prioridad.slice(1)}
              </span>
            </div>
            <span className="alerta-fecha">{alerta.fecha}</span>
            {!alerta.leida && <div className="alerta-punto" />}
          </div>
        ))}
      </div>
    </div>
  );
};

export default AlertasPage;
