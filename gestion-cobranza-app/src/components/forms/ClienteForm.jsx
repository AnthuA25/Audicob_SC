import { useState, useEffect } from "react";

const ClienteForm = ({
  clienteEditar,
  onGuardar,
  onCancelar,
  clientesExistentes = [],
}) => {
  const [form, setForm] = useState({
    nombre: "",
    email: "",
    dni: "",
    telefono: "",
    asesorAsignado: "",
    deudaPendiente: "",
    fechaPago: "",
  });

  const [errores, setErrores] = useState({});

  useEffect(() => {
    if (clienteEditar) {
      setForm({
        nombre: clienteEditar.nombre || "",
        email: clienteEditar.email || "",
        dni: clienteEditar.dni || "",
        telefono: clienteEditar.telefono || "",
        asesorAsignado: clienteEditar.asesorAsignado || "",
        deudaPendiente: clienteEditar.deudaPendiente || "",
        fechaPago: clienteEditar.fechaPago || "",
      });
    }
  }, [clienteEditar]);

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
    setErrores({ ...errores, [e.target.name]: "" });
  };

  const validar = () => {
    const nuevosErrores = {};

    if (!form.nombre.trim()) nuevosErrores.nombre = "El nombre es obligatorio.";

    if (!form.email.trim()) nuevosErrores.email = "El email es obligatorio.";
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email))
      nuevosErrores.email = "El formato del email es inválido.";

    if (!form.dni.trim()) nuevosErrores.dni = "El DNI es obligatorio.";
    else if (!/^\d{8}$/.test(form.dni))
      nuevosErrores.dni = "El DNI debe tener exactamente 8 dígitos numéricos.";
    else if (
      !clienteEditar &&
      clientesExistentes.some((c) => c.dni === form.dni)
    )
      nuevosErrores.dni = "Ya existe un cliente registrado con este DNI.";

    if (!form.telefono.trim())
      nuevosErrores.telefono = "El teléfono es obligatorio.";

    if (!form.asesorAsignado)
      nuevosErrores.asesorAsignado = "Selecciona un asesor.";

    if (!form.deudaPendiente.trim())
      nuevosErrores.deudaPendiente = "La deuda pendiente es obligatoria.";
    else if (!/^\d+(\.\d{1,2})?$/.test(form.deudaPendiente))
      nuevosErrores.deudaPendiente =
        "Ingresa un valor numérico válido (ej: 1500.00).";

    if (!form.fechaPago)
      nuevosErrores.fechaPago = "La fecha de pago es obligatoria.";

    return nuevosErrores;
  };

  const handleSubmit = () => {
    const nuevosErrores = validar();
    if (Object.keys(nuevosErrores).length > 0) {
      setErrores(nuevosErrores);
      return;
    }
    onGuardar(form);
  };

  return (
    <div className="modal-overlay">
      <div className="modal-card">
        <h2 className="modal-title">
          {clienteEditar ? "Editar Cliente" : "Nuevo Cliente"}
        </h2>

        <div className="form-field">
          <label>Nombre Completo *</label>
          <input
            name="nombre"
            value={form.nombre}
            onChange={handleChange}
            style={errores.nombre ? { borderColor: "#ef4444" } : {}}
          />
          {errores.nombre && (
            <span className="form-error">{errores.nombre}</span>
          )}
        </div>

        <div className="form-field">
          <label>Email *</label>
          <input
            name="email"
            value={form.email}
            onChange={handleChange}
            style={errores.email ? { borderColor: "#ef4444" } : {}}
          />
          {errores.email && <span className="form-error">{errores.email}</span>}
        </div>

        <div className="form-row">
          <div className="form-field">
            <label>DNI *</label>
            <input
              name="dni"
              value={form.dni}
              onChange={handleChange}
              maxLength={8}
              style={errores.dni ? { borderColor: "#ef4444" } : {}}
            />
            {errores.dni && <span className="form-error">{errores.dni}</span>}
          </div>
          <div className="form-field">
            <label>Teléfono *</label>
            <input
              name="telefono"
              value={form.telefono}
              onChange={handleChange}
              style={errores.telefono ? { borderColor: "#ef4444" } : {}}
            />
            {errores.telefono && (
              <span className="form-error">{errores.telefono}</span>
            )}
          </div>
        </div>

        <div className="form-field">
          <label>Asesor Asignado *</label>
          <select
            name="asesorAsignado"
            value={form.asesorAsignado}
            onChange={handleChange}
            style={errores.asesorAsignado ? { borderColor: "#ef4444" } : {}}
          >
            <option value="">Seleccionar asesor</option>
            <option value="Carlos Rodríguez">Carlos Rodríguez</option>
            <option value="María López">María López</option>
          </select>
          {errores.asesorAsignado && (
            <span className="form-error">{errores.asesorAsignado}</span>
          )}
        </div>

        <div className="form-field">
          <label>Deuda Pendiente *</label>
          <input
            name="deudaPendiente"
            value={form.deudaPendiente}
            onChange={handleChange}
            placeholder="Ej: 1500.00"
            style={errores.deudaPendiente ? { borderColor: "#ef4444" } : {}}
          />
          {errores.deudaPendiente && (
            <span className="form-error">{errores.deudaPendiente}</span>
          )}
        </div>

        <div className="form-field">
          <label>Fecha de Pago *</label>
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

        <div className="modal-actions">
          <button className="btn-cancelar" onClick={onCancelar}>
            Cancelar
          </button>
          <button className="btn-confirmar" onClick={handleSubmit}>
            {clienteEditar ? "Actualizar cliente" : "Crear Cliente"}
          </button>
        </div>
      </div>
    </div>
  );
};

export default ClienteForm;