import { useState, useEffect } from "react";

const ClienteForm = ({ clienteEditar, onGuardar, onCancelar }) => {
  const [form, setForm] = useState({
    nombre: "",
    email: "",
    dni: "",
    telefono: "",
    asesorAsignado: "",
    deudaPendiente: "",
    fechaPago: "",
  });

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
  };

  const handleSubmit = () => {
    if (!form.nombre || !form.email) return;
    onGuardar(form);
  };

  return (
    <div className="modal-overlay">
      <div className="modal-card">
        <h2 className="modal-title">
          {clienteEditar ? "Editar Cliente" : "Nuevo Cliente"}
        </h2>

        <div className="form-field">
          <label>Nombre Completo</label>
          <input name="nombre" value={form.nombre} onChange={handleChange} />
        </div>

        <div className="form-field">
          <label>Email</label>
          <input name="email" value={form.email} onChange={handleChange} />
        </div>

        <div className="form-row">
          <div className="form-field">
            <label>DNI</label>
            <input name="dni" value={form.dni} onChange={handleChange} />
          </div>
          <div className="form-field">
            <label>Teléfono</label>
            <input
              name="telefono"
              value={form.telefono}
              onChange={handleChange}
            />
          </div>
        </div>

        <div className="form-field">
          <label>Asesor Asignado</label>
          <select
            name="asesorAsignado"
            value={form.asesorAsignado}
            onChange={handleChange}
          >
            <option value="">Seleccionar asesor</option>
            <option value="Carlos Rodríguez">Carlos Rodríguez</option>
            <option value="María López">María López</option>
          </select>
        </div>

        <div className="form-field">
          <label>Deuda Pendiente</label>
          <input
            name="deudaPendiente"
            value={form.deudaPendiente}
            onChange={handleChange}
          />
        </div>

        <div className="form-field">
          <label>Fecha de Pago</label>
          <input
            type="date"
            name="fechaPago"
            value={form.fechaPago}
            onChange={handleChange}
          />
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
