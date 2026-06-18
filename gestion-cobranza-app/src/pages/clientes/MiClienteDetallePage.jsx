import { useState } from "react";
import useAuth from "../../hooks/useAuth";
import {
  ArrowLeft,
  Mail,
  Phone,
  Calendar,
  MessageSquare,
  DollarSign,
  TrendingUp,
  Clock,
  AlertCircle,
  CheckCircle,
  PhoneCall,
} from "lucide-react";
import { useNavigate, useParams } from "react-router-dom";
import useMiClienteDetalle from "../../hooks/useMiClienteDetalle";
import { registrarGestion } from "../../services/gestionesService";
import "../../styles/miClienteDetalle.css";

const formatoMoneda = (valor) => `S/. ${Number(valor ?? 0).toLocaleString()}`;

const MiClienteDetallePage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();
  const { cliente, loading, error, cargarDetalle } = useMiClienteDetalle(id);

  const [tabActiva, setTabActiva] = useState("bitacora");
  const [modalGestion, setModalGestion] = useState(false);
  const [toast, setToast] = useState("");

  const [formGestion, setFormGestion] = useState({
    tipo: "Llamada",
    descripcion: "",
    resultado: "",
    proximaGestion: "",
  });

  const mostrarToast = (msg) => {
    setToast(msg);
    setTimeout(() => setToast(""), 3000);
  };

  const handleGestion = async () => {
    if (!formGestion.descripcion.trim()) {
      mostrarToast("La descripción es obligatoria.");
      return;
    }

    try {
      await registrarGestion({
        idCliente: Number(id),
        tipoGestion: formGestion.tipo,
        descripcion: formGestion.descripcion.trim(),
        resultado: formGestion.resultado.trim(),
        proximaAccion: formGestion.proximaGestion || null,
      });
      mostrarToast("Gestión registrada correctamente");
      setModalGestion(false);

      setFormGestion({
        tipo: "Llamada",
        descripcion: "",
        resultado: "",
        proximaGestion: "",
      });
      await cargarDetalle();
    } catch (err) {
      mostrarToast(
        err.response?.data?.message || "No se pudo registrar la gestión.",
      );
    }
  };

  const getTipoIcon = (tipo) => {
    if (tipo === "Llamada" || tipo === "LLAMADA")
      return <PhoneCall size={14} color="#3b82f6" />;

    if (tipo === "Acuerdo" || tipo === "ACUERDO")
      return <CheckCircle size={14} color="#22c55e" />;

    if (tipo === "PAGO" || tipo === "Pago registrado")
      return <DollarSign size={14} color="#a855f7" />;

    return <MessageSquare size={14} color="#64748b" />;
  };

  if (loading) return <div>Cargando detalle del cliente...</div>;

  if (error) return <p style={{ color: "red" }}>{error}</p>;

  if (!cliente) return <p>No se encontró información del cliente.</p>;

  return (
    <div>
      <div className="detalle-header">
        <div className="detalle-header-left">
          <button
            className="detalle-back"
            onClick={() =>
              navigate(
                user?.rol === "Administrador" ? "/morosidad" : "/mis-clientes",
              )
            }
          >
            <ArrowLeft size={16} /> Volver
          </button>
          <h1>{cliente.nombreCompleto}</h1>
          <p>Detalle completo del cliente</p>
        </div>

        <div style={{ display: "flex", gap: "10px" }}>
          <button className="btn-nuevo" onClick={() => setModalGestion(true)}>
            <MessageSquare size={16} /> Nueva Gestión
          </button>
        </div>
      </div>

      <div className="detalle-metricas">
        <div className="detalle-metric-card">
          <div className="detalle-metric-info">
            <p>Deuda Total</p>
            <div className="detalle-metric-valor">
              {formatoMoneda(cliente.deudaTotal)}
            </div>
          </div>
          <div className="detalle-metric-icon">
            <DollarSign size={16} color="#a855f7" />
          </div>
        </div>

        <div className="detalle-metric-card">
          <div className="detalle-metric-info">
            <p>Deuda pendiente</p>
            <div className="detalle-metric-valor rojo">
              {formatoMoneda(cliente.deudaPendiente)}
            </div>
          </div>
          <div className="detalle-metric-icon">
            <TrendingUp size={16} color="#ef4444" />
          </div>
        </div>

        <div className="detalle-metric-card">
          <div className="detalle-metric-info">
            <p>Días de Atraso</p>
            <div className="detalle-metric-valor rojo">
              {cliente.diasAtraso ?? 0}
            </div>
          </div>
          <div className="detalle-metric-icon">
            <Clock size={16} color="#3b82f6" />
          </div>
        </div>

        <div className="detalle-metric-card">
          <div className="detalle-metric-info">
            <p>Score de Riesgo</p>
            <div className="detalle-metric-valor">{cliente.riesgo}</div>
          </div>
          <div className="detalle-metric-icon">
            <AlertCircle size={16} color="#f59e0b" />
          </div>
        </div>
      </div>

      <div className="detalle-info-grid">
        <div className="detalle-info-card">
          <h3>Información de Contacto</h3>
          <div className="detalle-info-row">
            <Mail size={14} color="#94a3b8" />
            {cliente.correo || "-"}
          </div>
          <div className="detalle-info-row">
            <Phone size={14} color="#94a3b8" />
            {cliente.telefono || "-"}
          </div>
          <div className="detalle-info-row">
            <Calendar size={14} color="#94a3b8" />
            Registro: {cliente.fechaRegistro || "-"}
          </div>
        </div>

        <div className="detalle-info-card">
          <h3>Estado y Seguimiento</h3>
          <div className="estado-label">Estado Actual</div>
          <div className="estado-valor">{cliente.estadoActual || "-"}</div>

          <div className="estado-label">Último Contacto</div>
          <div className="estado-fecha">{cliente.ultimoContacto || "-"}</div>

          <div className="estado-label">Próximo Seguimiento</div>
          <div className="estado-fecha">
            {cliente.proximoSeguimiento || "-"}
          </div>
        </div>

        <div className="detalle-info-card">
          <h3>Asesor Actual</h3>
          <div className="detalle-info-row">{cliente.asesorActual || "-"}</div>
          <div className="detalle-info-row">
            <Mail size={14} color="#94a3b8" />
            {cliente.correo || "-"}
          </div>
          <div className="detalle-info-row">
            <Phone size={14} color="#94a3b8" />
            {cliente.telefono || "-"}
          </div>
          <div className="detalle-info-row">
            <Calendar size={14} color="#94a3b8" />
            Registro: {cliente.fechaRegistro || "-"}
          </div>
        </div>
      </div>

      <div className="detalle-tabs">
        <button
          className={`tab-btn ${tabActiva === "bitacora" ? "activo" : ""}`}
          onClick={() => setTabActiva("bitacora")}
        >
          Bitácora de Gestión
        </button>
        <button
          className={`tab-btn ${tabActiva === "pagos" ? "activo" : ""}`}
          onClick={() => setTabActiva("pagos")}
        >
          Historial de Pagos
        </button>
        <button
          className={`tab-btn ${tabActiva === "timeline" ? "activo" : ""}`}
          onClick={() => setTabActiva("timeline")}
        >
          Timeline
        </button>
      </div>

      <div className="detalle-tab-content">
        {tabActiva === "bitacora" && (
          <div>
            <p
              style={{
                fontSize: "14px",
                fontWeight: "500",
                color: "#1e3a5f",
                marginBottom: "1rem",
              }}
            >
              Bitácora de Gestión
            </p>

            {(cliente.bitacora || []).length === 0 && (
              <p>No hay gestiones registradas.</p>
            )}

            {(cliente.bitacora || []).map((item, i) => (
              <div className="bitacora-item" key={i}>
                <div className="bitacora-icon">
                  {getTipoIcon(item.tipoGestion || item.titulo)}
                </div>
                <div className="bitacora-info">
                  <strong>{item.titulo || item.tipoGestion}</strong>
                  <span>{item.descripcion}</span>
                </div>
                <span className="bitacora-fecha">
                  {item.fechaTexto || item.fecha}
                </span>
              </div>
            ))}
          </div>
        )}

        {tabActiva === "pagos" && (
          <div>
            <p
              style={{
                fontSize: "14px",
                fontWeight: "500",
                color: "#1e3a5f",
                marginBottom: "1rem",
              }}
            >
              Historial de Pagos
            </p>

            {(cliente.historialPagos || []).length === 0 && (
              <p>No hay pagos registrados.</p>
            )}

            {(cliente.historialPagos || []).map((pago, i) => (
              <div className="pago-item" key={i}>
                <div className="pago-info">
                  <strong>{formatoMoneda(pago.monto)}</strong>
                  <span>
                    {pago.fechaTexto || pago.fechaPago} • {pago.metodoPago}
                  </span>
                  <span>{pago.nota || "-"}</span>
                </div>
                <CheckCircle size={18} color="#22c55e" />
              </div>
            ))}
          </div>
        )}

        {tabActiva === "timeline" && (
          <div>
            <p
              style={{
                fontSize: "14px",
                fontWeight: "500",
                color: "#1e3a5f",
                marginBottom: "1rem",
              }}
            >
              Timeline Completo
            </p>

            {(cliente.timeline || []).length === 0 && (
              <p>No hay eventos en el timeline.</p>
            )}

            {(cliente.timeline || []).map((item, i) => (
              <div className="timeline-item" key={i}>
                <div className="timeline-icon">
                  {getTipoIcon(item.tipo || item.titulo)}
                </div>
                <div className="timeline-info">
                  <strong>{item.titulo || item.tipo}</strong>
                  <span>{item.descripcion}</span>
                </div>
                <span className="timeline-fecha">{item.fecha}</span>
              </div>
            ))}
          </div>
        )}
      </div>

      {modalGestion && (
        <div className="modal-overlay">
          <div className="modal-card">
            <p className="modal-title">Registrar Gestión</p>
            <p className="modal-subtitle">
              Registra una nueva interacción con el cliente
            </p>

            <div className="form-field">
              <label>Tipo de Gestión</label>
              <select
                value={formGestion.tipo}
                onChange={(e) =>
                  setFormGestion({ ...formGestion, tipo: e.target.value })
                }
              >
                <option>Llamada</option>
                <option>Mensaje</option>
                <option>Acuerdo</option>
                <option>Visita</option>
                <option>Otro</option>
              </select>
            </div>

            <div className="form-field">
              <label>Descripción</label>
              <textarea
                placeholder="Describe la gestión realizada..."
                value={formGestion.descripcion}
                onChange={(e) =>
                  setFormGestion({
                    ...formGestion,
                    descripcion: e.target.value,
                  })
                }
              />
            </div>

            <div className="form-field">
              <label>Resultado</label>
              <select
                value={formGestion.resultado}
                onChange={(e) =>
                  setFormGestion({ ...formGestion, resultado: e.target.value })
                }
              >
                <option value="">Seleccionar resultado</option>
                <option value="CONTACTADO">Contactado</option>
                <option value="NEGOCIACION">Negociación</option>
                <option value="PROMESA DE PAGO">Promesa de Pago</option>
                <option value="PAGADO">Pagado</option>
                <option value="MOROSO">Moroso</option>
              </select>
            </div>

            <div className="form-field">
              <label>Programar próxima gestión</label>
              <input
                type="date"
                value={formGestion.proximaGestion}
                onChange={(e) =>
                  setFormGestion({
                    ...formGestion,
                    proximaGestion: e.target.value,
                  })
                }
              />
            </div>

            <div className="modal-actions">
              <button
                className="btn-cancelar"
                onClick={() => setModalGestion(false)}
              >
                Cancelar
              </button>
              <button className="btn-confirmar" onClick={handleGestion}>
                Crear Gestión
              </button>
            </div>
          </div>
        </div>
      )}

      {toast && (
        <div className="toast">
          <span className="toast-icon">✓</span> {toast}
        </div>
      )}
    </div>
  );
};

export default MiClienteDetallePage;
