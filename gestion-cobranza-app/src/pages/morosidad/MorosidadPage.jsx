import { useState } from "react";
import {
  Search,
  FileBarChart,
  AlertTriangle,
  DollarSign,
  TrendingUp,
  Calendar,
  Eye,
} from "lucide-react";
import useMorosidad from "../../hooks/useMorosidad";
import {
  EstadoBadge,
  RiesgoBadge,
} from "../../components/clientes/ClienteEstadoBadge";
import "../../styles/morosidad.css";

const MorosidadPage = () => {
  const { morosidad, metricas, loading, error } = useMorosidad();
  const [busqueda, setBusqueda] = useState("");

  const filtrados = morosidad.filter(
    (m) =>
      m.nombre.toLowerCase().includes(busqueda.toLowerCase()) ||
      m.email.toLowerCase().includes(busqueda.toLowerCase()) ||
      String(m.id).includes(busqueda),
  );

  const getDiasClass = (dias) => {
    if (dias >= 60) return "dias-atraso alto";
    if (dias >= 20) return "dias-atraso medio";
    return "dias-atraso bajo";
  };

  if (loading) return <div>Cargando...</div>;

  return (
    <div>
      <div className="morosidad-header">
        <div className="morosidad-header-info">
          <h1>Seguimiento de morosidad</h1>
          <p>Monitoreo y análisis de cuentas morosas</p>
        </div>
        <button className="btn-reporte">
          <FileBarChart size={16} /> Generar Reporte
        </button>
      </div>
      
      {error && <p style={{ color: "red" }}>{error}</p>}

      <div className="morosidad-metricas">
        <div className="morosidad-metric-card">
          <div className="morosidad-metric-info">
            <p>Clientes Morosos</p>
            <span>{metricas?.clientesMorosos}</span>
            <small>87.5% del total</small>
          </div>
          <div className="morosidad-metric-icon">
            <AlertTriangle size={18} color="#f59e0b" />
          </div>
        </div>

        <div className="morosidad-metric-card">
          <div className="morosidad-metric-info">
            <p>Deuda Morosa Total</p>
            <span>{metricas?.deudaMorosaTotal}</span>
            <small>En 7 cuentas</small>
          </div>
          <div className="morosidad-metric-icon">
            <DollarSign size={18} color="#a855f7" />
          </div>
        </div>

        <div className="morosidad-metric-card">
          <div className="morosidad-metric-info">
            <p>Morosidad Crítica</p>
            <span>{metricas?.morosidadCritica}</span>
            <small>Más de 60 días de atraso</small>
          </div>
          <div className="morosidad-metric-icon">
            <TrendingUp size={18} color="#f97316" />
          </div>
        </div>

        <div className="morosidad-metric-card">
          <div className="morosidad-metric-info">
            <p>Promedio de Atraso</p>
            <span>{metricas?.promedioAtraso}</span>
            <small>En cuentas morosas</small>
          </div>
          <div className="morosidad-metric-icon">
            <Calendar size={18} color="#22c55e" />
          </div>
        </div>
      </div>

      <div className="morosidad-table-section">
        <div className="morosidad-table-header">
          <h2>Clientes morosos ({morosidad.length})</h2>
          <p>Lista detallada de todas las cuentas con atraso en pagos</p>
        </div>

        <div className="morosidad-search">
          <Search size={16} className="morosidad-search-icon" />
          <input
            placeholder="Buscar por nombre, email o ID..."
            value={busqueda}
            onChange={(e) => setBusqueda(e.target.value)}
          />
        </div>

        <table className="morosidad-table">
          <thead>
            <tr>
              <th>Cliente</th>
              <th>Contacto</th>
              <th>Asesor Asignado</th>
              <th>Días Atraso</th>
              <th>Deuda Pendiente</th>
              <th>Riesgo</th>
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {filtrados.map((m) => (
              <tr key={m.id}>
                <td>{m.nombre}</td>
                <td>
                  <div className="morosidad-contacto">
                    <span>✉ {m.email}</span>
                    <span>📞 {m.telefono}</span>
                  </div>
                </td>
                <td>{m.asesorAsignado}</td>
                <td>
                  <span className={getDiasClass(m.diasAtraso)}>
                    {m.diasAtraso} días
                  </span>
                </td>
                <td>{m.deudaPendiente}</td>
                <td>
                  <RiesgoBadge riesgo={m.riesgo} />
                </td>
                <td>
                  <EstadoBadge estado={m.estado} />
                </td>
                <td>
                  <button className="btn-ver">
                    <Eye size={15} />
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {filtrados.length === 0 && (
          <p style={{ padding: "16px", color: "#64748b" }}>
            No se encontraron clientes morosos.
          </p>
        )}
      </div>
    </div>
  );
};

export default MorosidadPage;
