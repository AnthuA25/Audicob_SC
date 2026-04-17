import { ArrowLeft, Mail, Phone, Calendar, MessageSquare } from "lucide-react";
import { useNavigate, useParams } from "react-router-dom";
import useMiClienteDetalle from "../../hooks/useMiClienteDetalle";
import "../../styles/dashboard.css";

const MiClienteDetallePage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { cliente, loading, error } = useMiClienteDetalle(id);

  if (loading) return <div>Cargando detalle del cliente...</div>;
  if (error) return <div>{error}</div>;
  if (!cliente) return <div>No se encontró el cliente.</div>;

  return (
    <div>
      <div className="dashboard-header" style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <div>
          <button
            onClick={() => navigate("/mis-clientes")}
            style={{ background: "none", border: "none", cursor: "pointer", marginBottom: "10px" }}
          >
            <ArrowLeft size={20} />
          </button>
          <h1>{cliente.nombreCompleto}</h1>
          <p>Detalle completo del cliente</p>
        </div>

        <button className="btn-nuevo">
          <MessageSquare size={16} /> Nueva Gestión
        </button>
      </div>

      <div className="metricas-grid">
        <div className="metric-card">
          <div className="metric-label">Deuda Total</div>
          <div className="metric-valor">{cliente.deudaTotal}</div>
        </div>
        <div className="metric-card">
          <div className="metric-label">Deuda pendiente</div>
          <div className="metric-valor" style={{ color: "red" }}>{cliente.deudaPendiente}</div>
        </div>
        <div className="metric-card">
          <div className="metric-label">Días de Atraso</div>
          <div className="metric-valor" style={{ color: "red" }}>{cliente.diasAtraso}</div>
        </div>
        <div className="metric-card">
          <div className="metric-label">Score de Riesgo</div>
          <div className="metric-valor">{cliente.scoreRiesgo}</div>
        </div>
      </div>

      <div className="charts-grid" style={{ gridTemplateColumns: "repeat(3, 1fr)" }}>
        <div className="chart-card">
          <p className="chart-title">Información de Contacto</p>
          <p><Mail size={14} /> {cliente.correo}</p>
          <p><Phone size={14} /> {cliente.telefono}</p>
          <p><Calendar size={14} /> Registro: {cliente.fechaRegistro}</p>
        </div>

        <div className="chart-card">
          <p className="chart-title">Estado y Seguimiento</p>
          <p>Estado Actual</p>
          <p style={{ color: "#7c3aed" }}>{cliente.estadoActual}</p>
          <p>Último Contacto</p>
          <p>{cliente.ultimoContacto}</p>
          <p>Próximo Seguimiento</p>
          <p>{cliente.proximoSeguimiento}</p>
        </div>

        <div className="chart-card">
          <p className="chart-title">Asesor Actual</p>
          <p>{cliente.asesorActual}</p>
          <p><Mail size={14} /> {cliente.correo}</p>
          <p><Phone size={14} /> {cliente.telefono}</p>
        </div>
      </div>

      <div className="rendimiento-card" style={{ marginTop: "20px" }}>
        <p className="rendimiento-title">Bitácora de Gestión</p>
        {cliente.bitacora?.length > 0 ? (
          cliente.bitacora.map((item, index) => (
            <div key={index} style={{ padding: "12px 0", borderBottom: "1px solid #eee" }}>
              <strong>{item.titulo}</strong>
              <p>{item.descripcion}</p>
              <small>{item.fecha}</small>
            </div>
          ))
        ) : (
          <p>No hay gestiones registradas.</p>
        )}
      </div>
    </div>
  );
};

export default MiClienteDetallePage;