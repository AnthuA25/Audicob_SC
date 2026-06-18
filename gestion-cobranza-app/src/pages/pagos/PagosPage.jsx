import { useState, useRef, useEffect } from "react";
import { Plus, CheckCircle, Upload } from "lucide-react";
import usePagos from "../../hooks/usePagos";
import { registrarPago } from "../../services/pagosService";
import {
  fetchMisClientes,
  fetchMiClienteDetalle,
} from "../../services/misClientesService";
import "../../styles/pagos.css";

const formatoMoneda = (valor) => `S/. ${Number(valor ?? 0).toLocaleString()}`;

const obtenerIdUsuarioToken = () => {
  const token = localStorage.getItem("token");
  if (!token) return null;

  const payload = JSON.parse(atob(token.split(".")[1]));

  return (
    payload.nameid ||
    payload.sub ||
    payload[
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
    ]
  );
};

const PagosPage = () => {
  const idAsesor = obtenerIdUsuarioToken();
  const { pagos, resumen, loading, cargarPagos } = usePagos(idAsesor);

  const [clientes, setClientes] = useState([]);
  const [deudasCliente, setDeudasCliente] = useState([]);
  const [modalAbierto, setModalAbierto] = useState(false);
  const [toast, setToast] = useState("");
  const [comprobante, setComprobante] = useState(null);
  const [errores, setErrores] = useState({});
  const inputRef = useRef();

  const [form, setForm] = useState({
    idCliente: "",
    idDeuda: "",
    monto: "",
    fechaPago: "",
    metodoPago: "",
    nota: "",
  });

  useEffect(() => {
    const cargarClientes = async () => {
      try {
        const data = await fetchMisClientes();
        setClientes(data || []);
      } catch {
        setClientes([]);
      }
    };

    cargarClientes();
  }, []);

  const mostrarToast = (msg) => {
    setToast(msg);
    setTimeout(() => setToast(""), 3000);
  };

  const handleChange = async (e) => {
    const { name, value } = e.target;

    setForm((prev) => ({ ...prev, [name]: value }));
    setErrores((prev) => ({ ...prev, [name]: "" }));

    if (name === "idCliente") {
      try {
        const detalle = await fetchMiClienteDetalle(value);
        const deudas = detalle.deudas || [];

        setDeudasCliente(deudas);

        const primeraDeudaPendiente = deudas.find(
          (d) => Number(d.saldoPendiente) > 0,
        );

        setForm((prev) => ({
          ...prev,
          idCliente: value,
          idDeuda: primeraDeudaPendiente?.idDeuda || "",
        }));
      } catch {
        setDeudasCliente([]);
        setForm((prev) => ({ ...prev, idDeuda: "" }));
      }
    }
  };

  const validar = () => {
    const nuevos = {};

    if (!form.idCliente) nuevos.idCliente = "Selecciona un cliente.";
    if (!form.idDeuda) nuevos.idDeuda = "El cliente no tiene deuda pendiente.";
    if (!form.monto) nuevos.monto = "El monto es obligatorio.";
    else if (!/^\d+(\.\d{1,2})?$/.test(form.monto))
      nuevos.monto = "Ingresa un monto válido.";

    if (!form.fechaPago) nuevos.fechaPago = "La fecha es obligatoria.";
    if (!form.metodoPago) nuevos.metodoPago = "Selecciona un método de pago.";

    return nuevos;
  };

  const handleSubmit = async () => {
    const nuevosErrores = validar();

    if (Object.keys(nuevosErrores).length > 0) {
      setErrores(nuevosErrores);
      return;
    }

    try {
      await registrarPago({
        idDeuda: Number(form.idDeuda),
        montoPagado: Number(form.monto),
        metodoPago: form.metodoPago,
        nroOperacion: comprobante?.name || "",
        observacion: form.nota,
      });

      await cargarPagos();

      setModalAbierto(false);
      setForm({
        idCliente: "",
        idDeuda: "",
        monto: "",
        fechaPago: "",
        metodoPago: "",
        nota: "",
      });
      setDeudasCliente([]);
      setComprobante(null);

      mostrarToast("Pago registrado correctamente");
    } catch (error) {
      mostrarToast(
        error.response?.data?.mensaje || "No se pudo registrar el pago.",
      );
    }
  };

  if (loading) return <div>Cargando pagos...</div>;

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
            <div className="pago-metric-valor">
              {formatoMoneda(resumen.totalPagosHoy)}
            </div>
            <div className="pago-metric-sub">
              {resumen.transaccionesHoy} transacciones
            </div>
          </div>
          <div className="pago-metric-icon">
            <CheckCircle size={18} color="#22c55e" />
          </div>
        </div>
        <div className="pago-metric-card">
          <div className="pago-metric-info">
            <p>Esta Semana</p>
            <div className="pago-metric-valor">
              {formatoMoneda(resumen.totalPagosSemana)}
            </div>
            <div className="pago-metric-sub">
              {resumen.transaccionesSemana} transacciones
            </div>
          </div>
          <div className="pago-metric-icon">
            <CheckCircle size={18} color="#22c55e" />
          </div>
        </div>
        <div className="pago-metric-card">
          <div className="pago-metric-info">
            <p>Este Mes</p>
            <div className="pago-metric-valor">
              {formatoMoneda(resumen.totalPagosMes)}
            </div>
            <div className="pago-metric-sub">
              {resumen.transaccionesMes} transacciones
            </div>
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
            {pagos.map((pago) => (
              <tr key={pago.idPago}>
                <td style={{ padding: "14px 12px", color: "black" }}>
                  {pago.fechaPago}
                </td>
                <td style={{ padding: "14px 12px", color: "black" }}>
                  {pago.nombreCliente}
                </td>
                <td
                  style={{
                    padding: "14px 12px",
                    color: "#22c55e",
                    fontWeight: "500",
                  }}
                >
                  {formatoMoneda(pago.monto)}
                </td>
                <td style={{ padding: "14px 12px", color: "black" }}>
                  {pago.metodoPago}
                </td>
                <td style={{ padding: "14px 12px", color: "black" }}>
                  {pago.nota || "-"}
                </td>
                <td style={{ padding: "14px 12px", color: "black" }}>
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
                name="idCliente"
                value={form.idCliente}
                onChange={handleChange}
                style={errores.cliente ? { borderColor: "#ef4444" } : {}}
              >
                <option value="">Seleccionar cliente ...</option>
                {clientes.map((c) => (
                  <option key={c.idCliente} value={c.idCliente}>
                    {c.nombres} {c.apellidos}
                  </option>
                ))}
              </select>
              {errores.idCliente && (
                <span className="form-error">{errores.idCliente}</span>
              )}
            </div>

            {deudasCliente.length > 1 && (
              <div className="form-field">
                <label>Deuda</label>
                <select
                  name="idDeuda"
                  value={form.idDeuda}
                  onChange={handleChange}
                >
                  <option value="">Seleccionar deuda ...</option>
                  {deudasCliente
                    .filter((d) => Number(d.saldoPendiente) > 0)
                    .map((d) => (
                      <option key={d.idDeuda} value={d.idDeuda}>
                        Deuda #{d.idDeuda} - Saldo{" "}
                        {formatoMoneda(d.saldoPendiente)}
                      </option>
                    ))}
                </select>
              </div>
            )}

            {errores.idDeuda && (
              <span className="form-error">{errores.idDeuda}</span>
            )}

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
                <option value="Transferencia">Transferencia</option>
                <option value="Efectivo">Efectivo</option>
                <option value="Tarjeta">Tarjeta</option>
                <option value="Yape">Yape</option>
                <option value="Plin">Plin</option>
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
