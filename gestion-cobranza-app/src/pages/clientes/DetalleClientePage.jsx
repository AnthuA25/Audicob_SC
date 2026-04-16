import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import {
  ArrowLeft,
  Plus,
  Mail,
  Phone,
  CalendarCheck,
  UserCheck,
} from "lucide-react";
import useClientes from "../../hooks/useClientes";
import "../../styles/clientes.css";

const DetalleClientePage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { clientes, loading } = useClientes();
  const [cliente, setCliente] = useState(null);
  const [activeTab, setActiveTab] = useState("bitacora");

  useEffect(() => {
    if (!loading) {
      const item = clientes.find((c) => c.id.toString() === id);
      setCliente(item);
    }
  }, [clientes, id, loading]);

  if (loading) {
    return <div className="clientes-page">Cargando cliente...</div>;
  }

  if (!cliente) {
    return (
      <div className="clientes-page">
        <button className="detalle-back" onClick={() => navigate(-1)}>
          <ArrowLeft size={18} /> Volver
        </button>
        <div className="detalle-missing">Cliente no encontrado</div>
      </div>
    );
  }

  const deudaTotal = "S./ 15,000";
  const deudaPendiente = cliente.deudaPendiente;
  const diasAtraso = cliente.diasAtraso;
  const scoreRiesgo = 40;

  return (
    <div className="clientes-detail-page">
      <div className="detalle-header">
        <div>
          <button className="detalle-back" onClick={() => navigate(-1)}>
            <ArrowLeft size={18} />
            <span>Volver</span>
          </button>
          <h1>{cliente.nombre}</h1>
          <p>Detalle completo del cliente</p>
        </div>
        <button className="btn-gestion">
          <Plus size={16} /> Nueva Gestión
        </button>
      </div>

      <div className="detalle-stats-grid">
        <article className="detalle-stat-card">
          <span className="detalle-stat-label">Deuda Total</span>
          <strong>{deudaTotal}</strong>
          <span className="detalle-stat-badge">S./</span>
        </article>
        <article className="detalle-stat-card">
          <span className="detalle-stat-label">Deuda pendiente</span>
          <strong className="text-danger">{deudaPendiente}</strong>
          <span className="detalle-stat-meta">↗</span>
        </article>
        <article className="detalle-stat-card">
          <span className="detalle-stat-label">Días de Atraso</span>
          <strong>{diasAtraso}</strong>
          <span className="detalle-stat-meta">⏱</span>
        </article>
        <article className="detalle-stat-card">
          <span className="detalle-stat-label">Score de Riesgo</span>
          <strong>{scoreRiesgo}</strong>
          <span className="detalle-stat-meta">⚠</span>
        </article>
      </div>

      <div className="detalle-info-grid">
        <section className="detalle-card">
          <h2>Información de Contacto</h2>
          <div className="detalle-contact-item">
            <Mail size={16} />
            <span>{cliente.email}</span>
          </div>
          <div className="detalle-contact-item">
            <Phone size={16} />
            <span>{cliente.telefono}</span>
          </div>
          <div className="detalle-contact-item">
            <CalendarCheck size={16} />
            <span>Registro: 14 ago 2025</span>
          </div>
        </section>

        <section className="detalle-card">
          <h2>Estado y Seguimiento</h2>
          <div className="detalle-status-row">
            <span>Estado Actual</span>
            <strong>{cliente.estado}</strong>
          </div>
          <div className="detalle-status-row">
            <span>Último Contacto</span>
            <strong>14 mar 2026</strong>
          </div>
          <div className="detalle-status-row">
            <span>Próximo Seguimiento</span>
            <strong>24 mar 2026</strong>
          </div>
        </section>

        <section className="detalle-card">
          <h2>Asesor Actual</h2>
          <div className="detalle-contact-item">
            <UserCheck size={16} />
            <span>{cliente.asesorAsignado}</span>
          </div>
          <div className="detalle-contact-item">
            <Mail size={16} />
            <span>{cliente.email}</span>
          </div>
          <div className="detalle-contact-item">
            <Phone size={16} />
            <span>{cliente.telefono}</span>
          </div>
          <div className="detalle-contact-item">
            <CalendarCheck size={16} />
            <span>Registro: 14 ago 2025</span>
          </div>
        </section>
      </div>

      <div className="detalle-tabs">
        <button
          className={activeTab === "bitacora" ? "tab active" : "tab"}
          onClick={() => setActiveTab("bitacora")}
        >
          Bitácora de Gestión
        </button>
        <button
          className={activeTab === "pagos" ? "tab active" : "tab"}
          onClick={() => setActiveTab("pagos")}
        >
          Historial de Pagos
        </button>
        <button
          className={activeTab === "timeline" ? "tab active" : "tab"}
          onClick={() => setActiveTab("timeline")}
        >
          Timeline
        </button>
      </div>

      <div className="detalle-tab-content">
        {activeTab === "bitacora" && (
          <div className="detalle-log-card">
            <div className="detalle-log-entry">
              <div className="detalle-log-icon">
                <CheckIcon />
              </div>
              <div>
                <h3>Acuerdo</h3>
                <p>Acuerdo de pago: 4 cuotas de S/. 3,750</p>
              </div>
              <span>15 mar 2026 10:45</span>
            </div>
            <div className="detalle-log-entry">
              <div className="detalle-log-icon">
                <Phone size={16} />
              </div>
              <div>
                <h3>Llamada</h3>
                <p>Contacto telefónico. Cliente solicita plan de pagos.</p>
              </div>
              <span>15 mar 2026 10:30</span>
            </div>
          </div>
        )}
        {activeTab === "pagos" && (
          <div className="detalle-log-card detalle-placeholder">
            <p>Historial de pagos aún no disponible.</p>
          </div>
        )}
        {activeTab === "timeline" && (
          <div className="detalle-log-card detalle-placeholder">
            <p>Timeline del cliente aún no disponible.</p>
          </div>
        )}
      </div>
    </div>
  );
};

const CheckIcon = () => (
  <div className="check-icon">
    <svg viewBox="0 0 24 24" width="16" height="16" fill="none" stroke="#10b981" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <path d="M20 6L9 17l-5-5" />
    </svg>
  </div>
);

export default DetalleClientePage;
