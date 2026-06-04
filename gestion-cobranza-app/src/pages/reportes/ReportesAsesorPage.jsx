import { useState, useRef, useEffect } from "react";
import { Download, FileText, Calendar } from "lucide-react";
import {
  getReporteAsesorApi,
  generarReporteAsesorApi,
} from "../../api/reportesApi";
import "../../styles/reportes.css";

const tiposReporteAsesor = [
  "Mi Cartera",
  "Mis Clientes",
  "Mis Deudas",
  "Mis Pagos Recuperados",
  "Mis Gestiones",
  "Mis Alertas",
];

const formatoSoles = (valor) => {
  return `S./ ${Number(valor || 0).toLocaleString("es-PE", {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  })}`;
};

const formatoFecha = (fecha) => {
  if (!fecha) return "";
  return new Date(fecha).toLocaleString("es-PE");
};


const ReportesAsesorPage = () => {
  const [tipoReporte, setTipoReporte] = useState("Reporte General");
  const [fechaDesde, setFechaDesde] = useState("");
  const [fechaHasta, setFechaHasta] = useState("");
  const [resumenClientesData, setResumenClientesData] = useState([]);
  const [reportesRecientes, setReportesRecientes] = useState([]);

  const fechaDesdeRef = useRef();
  const fechaHastaRef = useRef();

  const cargarDatos = async () => {
    try {
      const data = await getReporteAsesorApi();

      setResumenClientesData(data.distribucionClientes || []);
      setReportesRecientes(data.reportesRecientes || []);
    } catch (error) {
      console.error("Error al cargar reportes del asesor:", error);
    }
  };

  const descargarExcel = async () => {
    try {
      const data = await generarReporteAsesorApi({
        tipoReporte,
        fechaDesde: fechaDesde || null,
        fechaHasta: fechaHasta || null,
      });

      if (data.urlDescarga) {
        window.open(data.urlDescarga, "_blank");
      }

      await cargarDatos();
    } catch (error) {
      console.error("Error al descargar reporte:", error);
      alert("No se pudo descargar el reporte.");
    }
  };

  useEffect(() => {
    cargarDatos();
  }, []);


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
              {tiposReporteAsesor.map((tipo) => (
                <option key={tipo} value={tipo}>
                  {tipo}
                </option>
              ))}
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
                onClick={() => fechaDesdeRef.current?.showPicker()}
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
                onClick={() => fechaHastaRef.current?.showPicker()}
              />
            </div>
          </div>
        </div>
        <button className="btn-descargar-excel" onClick={descargarExcel}>
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
                <td>{formatoSoles(r.deudaTotal)}</td>
                <td>{r.porcentaje}%</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <div className="reportes-card">
        <h2>Reportes recientes</h2>
        <div className="reportes-recientes-list">
          {reportesRecientes.map((r) => (
            <div className="reporte-reciente-item" key={r.idReporte}>
              <div className="reporte-reciente-info">
                <div className="reporte-reciente-icon">
                  <FileText size={18} color="#3b82f6" />
                </div>
                <div className="reporte-reciente-texto">
                  <strong>{r.nombreReporte}</strong>
                  <span>
                    {formatoFecha(r.fechaGeneracion)}
                  </span>
                </div>
              </div>
              <button className="btn-descargar-reporte"
              onClick={() => window.open(r.archivoUrl, "_blank")}>
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
