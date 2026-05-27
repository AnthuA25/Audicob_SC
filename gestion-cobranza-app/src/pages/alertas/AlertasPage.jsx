import { useState } from "react";
import {
  AlertCircle,
  AlertTriangle,
  XCircle,
  Calendar,
  FileText,
} from "lucide-react";
import "../../styles/alertas.css";

const alertasData = [
  {
    id: 1,
    tipo: "Recordatorio de pago",
    descripcion: "Carmen López tiene 90 dias de atraso y S/ 22,000 de deuda",
    prioridad: "media",
    fecha: "Hoy",
    leida: false,
    icono: "calendar",
  },
  {
    id: 2,
    tipo: "Riesgo de morosidad",
    descripcion: "Laura Martínez - 12 dias de atraso, requiere seguimiento",
    prioridad: "alta",
    fecha: "Hoy",
    leida: false,
    icono: "alerta",
  },
  {
    id: 3,
    tipo: "Acuerdo pendiente",
    descripcion: "Miguel Torres - Acuerdo de pago sin firma desde hace 5 dias",
    prioridad: "alta",
    fecha: "Ayer",
    leida: true,
    icono: "file",
  },
  {
    id: 4,
    tipo: "Recordatorio de pago",
    descripcion: "Patricia Ramírez - Pago próximo a vencer el 20/03/2026",
    prioridad: "baja",
    fecha: "09/03/2026",
    leida: true,
    icono: "calendar",
  },
  {
    id: 5,
    tipo: "Riesgo de morosidad",
    descripcion: "Sofia Vargas - Cliente con historial de atrasos recurrentes",
    prioridad: "media",
    fecha: "08/03/2026",
    leida: true,
    icono: "alerta",
  },
];

const getIcono = (tipo) => {
  if (tipo === "calendar") return <Calendar size={16} color="#3b82f6" />;
  if (tipo === "alerta") return <AlertTriangle size={16} color="#f59e0b" />;
  return <FileText size={16} color="#64748b" />;
};

const AlertasPage = () => {
  const [tabActiva, setTabActiva] = useState("todas");

  const noLeidas = alertasData.filter((a) => !a.leida);
  const filtradas = tabActiva === "noLeidas" ? noLeidas : alertasData;

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
            <div className="alerta-metric-valor">15</div>
            <div className="alerta-metric-sub">5-15 dias de atraso</div>
          </div>
        </div>
        <div className="alerta-metric-card">
          <div className="alerta-metric-icon alto">
            <AlertTriangle size={18} color="#f97316" />
          </div>
          <div className="alerta-metric-info">
            <p>Riesgo Alto</p>
            <div className="alerta-metric-valor">18</div>
            <div className="alerta-metric-sub">16-30 dias de atraso</div>
          </div>
        </div>
        <div className="alerta-metric-card">
          <div className="alerta-metric-icon critico">
            <XCircle size={18} color="#ef4444" />
          </div>
          <div className="alerta-metric-info">
            <p>Crítico</p>
            <div className="alerta-metric-valor">7</div>
            <div className="alerta-metric-sub">+30 dias de atraso</div>
          </div>
        </div>
      </div>

      <div className="alertas-tabs">
        <button
          className={`alerta-tab ${tabActiva === "noLeidas" ? "activo" : ""}`}
          onClick={() => setTabActiva("noLeidas")}
        >
          No Leídas ({noLeidas.length})
        </button>
        <button
          className={`alerta-tab ${tabActiva === "todas" ? "activo" : ""}`}
          onClick={() => setTabActiva("todas")}
        >
          Todas ({alertasData.length})
        </button>
      </div>

      <div className="alertas-lista">
        {filtradas.map((alerta) => (
          <div className="alerta-item" key={alerta.id}>
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
