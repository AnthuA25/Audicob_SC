import { useState, useRef } from "react";
import { Download, FileText, Calendar } from "lucide-react";
import "../../styles/reportes.css";

const resumenClientesData = [
  { estado: "Nuevo", cantidad: 0, deudaTotal: "S./ 0", porcentaje: "0.0%" },
  {
    estado: "Contactado",
    cantidad: 1,
    deudaTotal: "S./ 12,000",
    porcentaje: "33.3%",
  },
  {
    estado: "Negociacion",
    cantidad: 1,
    deudaTotal: "S./ 5,500",
    porcentaje: "33.3%",
  },
  {
    estado: "Promesa pago",
    cantidad: 0,
    deudaTotal: "S./ 0",
    porcentaje: "0.0%",
  },
  { estado: "Pagado", cantidad: 1, deudaTotal: "S./ 0", porcentaje: "33.3%" },
  { estado: "Moroso", cantidad: 0, deudaTotal: "S./ 0", porcentaje: "0.0%" },
];

const reportesRecientes = [
  {
    id: 1,
    nombre: "Estado de cuenta - Marzo 2026",
    fecha: "11/03/2026 10:30",
    tamanio: "2.4 MB",
  },
  {
    id: 2,
    nombre: "Métricas de desempeño - Febrero 2026",
    fecha: "01/03/2026 14:15",
    tamanio: "1.8 MB",
  },
  {
    id: 3,
    nombre: "Reporte crediticio completo",
    fecha: "05/03/2026 16:45",
    tamanio: "3.2 MB",
  },
  {
    id: 4,
    nombre: "Estado de cuenta - Marzo 2026",
    fecha: "11/03/2026 10:30",
    tamanio: "2.4 MB",
  },
];

const ReportesAsesorPage = () => {
  const [tipoReporte, setTipoReporte] = useState("Reporte General");
  const [fechaDesde, setFechaDesde] = useState("");
  const [fechaHasta, setFechaHasta] = useState("");
  const fechaDesdeRef = useRef();
  const fechaHastaRef = useRef();

  return (
    <div>
      <div className="reportes-header">
        <h1>Reportes</h1>
        <p>Genera y descarga reportes del sistema</p>
      </div>

      <div className="reportes-card">
        <h2>Generar Reporte</h2>
        <div className="reporte-filtros">
          <div className="reporte-filtro-field">
            <label>Tipo de Reporte</label>
            <select
              value={tipoReporte}
              onChange={(e) => setTipoReporte(e.target.value)}
            >
              <option>Reporte General</option>
              <option>Cobranza</option>
              <option>Morosidad</option>
              <option>Clientes</option>
            </select>
          </div>
          <div className="reporte-filtro-field">
            <label>Fecha Desde</label>
            <div
              style={{
                position: "relative",
                display: "flex",
                alignItems: "center",
              }}
            >
              <input
                ref={fechaDesdeRef}
                type="date"
                value={fechaDesde}
                onChange={(e) => setFechaDesde(e.target.value)}
                style={{ paddingRight: "36px" }}
              />
              <Calendar
                size={16}
                color="#94a3b8"
                style={{
                  position: "absolute",
                  right: "10px",
                  cursor: "pointer",
                }}
                onClick={() => fechaDesdeRef.current.showPicker()}
              />
            </div>
          </div>
          <div className="reporte-filtro-field">
            <label>Fecha Hasta</label>
            <div
              style={{
                position: "relative",
                display: "flex",
                alignItems: "center",
              }}
            >
              <input
                ref={fechaHastaRef}
                type="date"
                value={fechaHasta}
                onChange={(e) => setFechaHasta(e.target.value)}
                style={{ paddingRight: "36px" }}
              />
              <Calendar
                size={16}
                color="#94a3b8"
                style={{
                  position: "absolute",
                  right: "10px",
                  cursor: "pointer",
                }}
                onClick={() => fechaHastaRef.current.showPicker()}
              />
            </div>
          </div>
        </div>
        <button className="btn-descargar-excel">
          <Download size={16} /> Descargar Excel
        </button>
      </div>

      <div className="reportes-card">
        <h2>Resumen de clientes</h2>
        <table className="reporte-table">
          <thead>
            <tr>
              <th>Estado</th>
              <th>Cantidad</th>
              <th>Deuda Total</th>
              <th>Porcentaje</th>
            </tr>
          </thead>
          <tbody>
            {resumenClientesData.map((r, i) => (
              <tr key={i}>
                <td>{r.estado}</td>
                <td>{r.cantidad}</td>
                <td>{r.deudaTotal}</td>
                <td>{r.porcentaje}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <div className="reportes-card">
        <h2>Reportes recientes</h2>
        <div className="reportes-recientes-list">
          {reportesRecientes.map((r) => (
            <div className="reporte-reciente-item" key={r.id}>
              <div className="reporte-reciente-info">
                <div className="reporte-reciente-icon">
                  <FileText size={18} color="#3b82f6" />
                </div>
                <div className="reporte-reciente-texto">
                  <strong>{r.nombre}</strong>
                  <span>
                    {r.fecha} • {r.tamanio}
                  </span>
                </div>
              </div>
              <button className="btn-descargar-reporte">
                <Download size={16} />
              </button>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default ReportesAsesorPage;
