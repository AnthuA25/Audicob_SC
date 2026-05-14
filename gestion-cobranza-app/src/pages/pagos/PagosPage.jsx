import { useState, useRef } from "react";
import { Plus, CheckCircle, Upload } from "lucide-react";
import "../../styles/pagos.css";

const clientesEjemplo = [
  { id: 1, nombre: "Miguel Torres" },
  { id: 2, nombre: "Juan Pérez" },
  { id: 3, nombre: "Laura Martínez" },
];

const PagosPage = () => {
  const [modalAbierto, setModalAbierto] = useState(false);
  const [toast, setToast] = useState("");
  const [comprobante, setComprobante] = useState(null);
  const [errores, setErrores] = useState({});
  const inputRef = useRef();

  const [form, setForm] = useState({
    cliente: "",
    monto: "",
    fechaPago: "",
    metodoPago: "",
    nota: "",
  });

  const mostrarToast = (msg) => {
    setToast(msg);
    setTimeout(() => setToast(""), 3000);
  };

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
    setErrores({ ...errores, [e.target.name]: "" });
  };

  const validar = () => {
    const nuevos = {};
    if (!form.cliente) nuevos.cliente = "Selecciona un cliente.";
    if (!form.monto) nuevos.monto = "El monto es obligatorio.";
    else if (!/^\d+(\.\d{1,2})?$/.test(form.monto))
      nuevos.monto = "Ingresa un monto válido.";
    if (!form.fechaPago) nuevos.fechaPago = "La fecha es obligatoria.";
    if (!form.metodoPago) nuevos.metodoPago = "Selecciona un método de pago.";
    return nuevos;
  };

  const handleSubmit = () => {
    const nuevosErrores = validar();
    if (Object.keys(nuevosErrores).length > 0) {
      setErrores(nuevosErrores);
      return;
    }
    setModalAbierto(false);
    setForm({
      cliente: "",
      monto: "",
      fechaPago: "",
      metodoPago: "",
      nota: "",
    });
    setComprobante(null);
    mostrarToast("Pago registrado correctamente");
  };

  return (
    <div>
      <div className="pagos-header">
        <div className="pagos-header-info">
          <h1>Registro de Pagos</h1>
          <p>Registra y consulta los pagos de tus clientes</p>
        </div>
        <button className="btn-nuevo" onClick={() => setModalAbierto(true)}>
          <Plus size={16} /> Registrar Pago
        </button>
      </div>

      <div className="pagos-metricas">
        <div className="pago-metric-card">
          <div className="pago-metric-info">
            <p>Total Pagos Hoy</p>
            <div className="pago-metric-valor">S/. 3,500</div>
            <div className="pago-metric-sub">2 transacciones</div>
          </div>
          <div className="pago-metric-icon">
            <CheckCircle size={18} color="#22c55e" />
          </div>
        </div>
        <div className="pago-metric-card">
          <div className="pago-metric-info">
            <p>Esta Semana</p>
            <div className="pago-metric-valor">S/. 15,100</div>
            <div className="pago-metric-sub">8 transacciones</div>
          </div>
          <div className="pago-metric-icon">
            <CheckCircle size={18} color="#22c55e" />
          </div>
        </div>
        <div className="pago-metric-card">
          <div className="pago-metric-info">
            <p>Este Mes</p>
            <div className="pago-metric-valor">S/. 45,000</div>
            <div className="pago-metric-sub">10 transacciones</div>
          </div>
          <div className="pago-metric-icon">
            <CheckCircle size={18} color="#22c55e" />
          </div>
        </div>
      </div>
      <div
        style={{
          background: "#fff",
          border: "1px solid #e2e8f0",
          borderRadius: "12px",
          padding: "1.25rem",
          marginTop: "1.5rem",
        }}
      >
        <p
          style={{
            fontSize: "14px",
            fontWeight: "600",
            color: "black",
            marginBottom: "1rem",
          }}
        >
          Historial de Pagos
        </p>
        <table
          style={{
            width: "100%",
            borderCollapse: "collapse",
            fontSize: "13px",
          }}
        >
          <thead>
            <tr>
              {["Fecha", "Cliente", "Monto", "Método", "Notas", "Estado"].map(
                (h) => (
                  <th
                    key={h}
                    style={{
                      textAlign: "left",
                      padding: "10px 12px",
                      color: "black",
                      fontWeight: "500",
                      borderBottom: "1px solid #e2e8f0",
                    }}
                  >
                    {h}
                  </th>
                ),
              )}
            </tr>
          </thead>
          <tbody>
            {[
              {
                fecha: "09 mar 2026",
                cliente: "Miguel Torres",
                monto: "S/. 3500",
                metodo: "Transferencia",
                notas: "Pago Total",
              },
              {
                fecha: "04 mar 2026",
                cliente: "Miguel Torres",
                monto: "S/. 3500",
                metodo: "Efectivo",
                notas: "Primera cuota",
              },
              {
                fecha: "09 feb 2026",
                cliente: "Juan Pérez",
                monto: "S/. 3500",
                metodo: "Transferencia",
                notas: "Pago parcial acordado",
              },
            ].map((p, i) => (
              <tr key={i}>
                <td
                  style={{
                    padding: "14px 12px",
                    borderBottom: "1px solid #f1f5f9",
                    color: "black",
                  }}
                >
                  {p.fecha}
                </td>
                <td
                  style={{
                    padding: "14px 12px",
                    borderBottom: "1px solid #f1f5f9",
                    color: "black",
                  }}
                >
                  {p.cliente}
                </td>
                <td
                  style={{
                    padding: "14px 12px",
                    borderBottom: "1px solid #f1f5f9",
                    color: "#22c55e",
                    fontWeight: "500",
                  }}
                >
                  {p.monto}
                </td>
                <td
                  style={{
                    padding: "14px 12px",
                    borderBottom: "1px solid #f1f5f9",
                    color: "black",
                  }}
                >
                  {p.metodo}
                </td>
                <td
                  style={{
                    padding: "14px 12px",
                    borderBottom: "1px solid #f1f5f9",
                    color: "black",
                  }}
                >
                  {p.notas}
                </td>
                <td
                  style={{
                    padding: "14px 12px",
                    borderBottom: "1px solid #f1f5f9",
                    color: "black",
                  }}
                >
                  <span
                    style={{
                      display: "flex",
                      alignItems: "center",
                      gap: "6px",
                      color: "#22c55e",
                      fontSize: "12px",
                    }}
                  >
                    <CheckCircle size={14} /> Confirmado
                  </span>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      {modalAbierto && (
        <div
          className="modal-pago-overlay"
          onClick={() => setModalAbierto(false)}
        >
          <div
            className="modal-pago-panel"
            onClick={(e) => e.stopPropagation()}
          >
            <h2>Registrar nuevo pago</h2>

            <div className="form-field">
              <label>Cliente</label>
              <select
                name="cliente"
                value={form.cliente}
                onChange={handleChange}
                style={errores.cliente ? { borderColor: "#ef4444" } : {}}
              >
                <option value="">Seleccionar cliente ...</option>
                {clientesEjemplo.map((c) => (
                  <option key={c.id} value={c.nombre}>
                    {c.nombre}
                  </option>
                ))}
              </select>
              {errores.cliente && (
                <span className="form-error">{errores.cliente}</span>
              )}
            </div>

            <div className="form-field">
              <label>Monto</label>
              <input
                name="monto"
                placeholder="S/. 0.00"
                value={form.monto}
                onChange={handleChange}
                style={errores.monto ? { borderColor: "#ef4444" } : {}}
              />
              {errores.monto && (
                <span className="form-error">{errores.monto}</span>
              )}
            </div>

            <div className="form-field">
              <label>Fecha de Pago</label>
              <input
                type="date"
                name="fechaPago"
                value={form.fechaPago}
                onChange={handleChange}
                style={errores.fechaPago ? { borderColor: "#ef4444" } : {}}
              />
              {errores.fechaPago && (
                <span className="form-error">{errores.fechaPago}</span>
              )}
            </div>

            <div className="form-field">
              <label>Método de pago</label>
              <select
                name="metodoPago"
                value={form.metodoPago}
                onChange={handleChange}
                style={errores.metodoPago ? { borderColor: "#ef4444" } : {}}
              >
                <option value="">Seleccionar método ...</option>
                <option>Transferencia</option>
                <option>Efectivo</option>
                <option>Tarjeta</option>
                <option>Yape</option>
                <option>Plin</option>
              </select>
              {errores.metodoPago && (
                <span className="form-error">{errores.metodoPago}</span>
              )}
            </div>

            <div className="form-field">
              <label>Comprobante (Opcional)</label>
              <div
                className="comprobante-dropzone"
                onClick={() => inputRef.current.click()}
              >
                <Upload size={20} color="#94a3b8" />
                <p>
                  {comprobante
                    ? comprobante.name
                    : "Arrastra o haz click para subir comprobante"}
                </p>
                <input
                  ref={inputRef}
                  type="file"
                  style={{ display: "none" }}
                  onChange={(e) => setComprobante(e.target.files[0])}
                />
              </div>
            </div>

            <div className="form-field">
              <label>Nota (Opcional)</label>
              <textarea
                name="nota"
                placeholder="Información adicional sobre el pago ..."
                value={form.nota}
                onChange={handleChange}
              />
            </div>

            <div className="pago-modal-actions">
              <button
                className="btn-cancelar-pago"
                onClick={() => setModalAbierto(false)}
              >
                Cancelar
              </button>
              <button className="btn-registrar-pago" onClick={handleSubmit}>
                Registrar Pago
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

export default PagosPage;
