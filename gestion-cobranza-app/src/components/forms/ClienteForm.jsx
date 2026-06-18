import { useEffect, useState } from "react";
import { fetchAsesores } from "../../services/clienteService";

const ClienteForm = ({
  clienteEditar,
  onGuardar,
  onCancelar,
  clientesExistentes = [],
}) => {
  const [asesores, setAsesores] = useState([]);
  const [form, setForm] = useState({
    nombres: "",
    apellidos: "",
    dni: "",
    correo: "",
    telefono: "",
    direccion: "",
    idAsesor: "",
    observacion: "",
    montoDeuda: "",
    fechaVencimiento: "",
  });

  const [errores, setErrores] = useState({});

  useEffect(() => {
    const cargarAsesores = async () => {
      try {
        const data = await fetchAsesores();
        setAsesores(data || []);
      } catch {
        setAsesores([]);
      }
    };

    cargarAsesores();
  }, []);

  useEffect(() => {
    if (clienteEditar) {
      setForm({
        nombres: clienteEditar.nombres || "",
        apellidos: clienteEditar.apellidos || "",
        dni: clienteEditar.dni || "",
        correo: clienteEditar.email || "",
        telefono: clienteEditar.telefono || "",
        direccion: clienteEditar.direccion || "",
        idAsesor: clienteEditar.idAsesor || "",
        observacion: clienteEditar.observacion || "",
        montoDeuda: clienteEditar.deudaTotal || "",
        fechaVencimiento: clienteEditar.fechaVencimiento || "",
      });
    }
  }, [clienteEditar]);

  const handleChange = (e) => {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
    setErrores((prev) => ({ ...prev, [e.target.name]: "" }));
  };

  const validar = () => {
    const nuevosErrores = {};

    if (!form.nombres.trim())
      nuevosErrores.nombres = "Los nombres son obligatorios.";

    if (!form.apellidos.trim())
      nuevosErrores.apellidos = "Los apellidos son obligatorios.";

    if (!form.dni.trim()) nuevosErrores.dni = "El DNI es obligatorio.";
    else if (!/^\d{8}$/.test(form.dni))
      nuevosErrores.dni = "El DNI debe tener exactamente 8 dígitos.";
    else if (
      !clienteEditar &&
      clientesExistentes.some((c) => c.dni === form.dni)
    )
      nuevosErrores.dni = "Ya existe un cliente con ese DNI.";

    if (form.correo && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.correo))
      nuevosErrores.correo = "El formato del correo es inválido.";

    if (!form.telefono.trim())
      nuevosErrores.telefono = "El teléfono es obligatorio.";

    // if (!form.idAsesor) nuevosErrores.idAsesor = "Selecciona un asesor.";

    if (!clienteEditar && !form.montoDeuda)
      nuevosErrores.montoDeuda = "La deuda total es obligatoria.";

    if (!clienteEditar && Number(form.montoDeuda) <= 0)
      nuevosErrores.montoDeuda = "La deuda total debe ser mayor a 0.";

    if (!clienteEditar && !form.fechaVencimiento)
      nuevosErrores.fechaVencimiento = "La fecha de pago es obligatoria.";

    return nuevosErrores;
  };

  const handleSubmit = () => {
    const nuevosErrores = validar();

    if (Object.keys(nuevosErrores).length > 0) {
      setErrores(nuevosErrores);
      return;
    }

    onGuardar({
      idAsesor: form.idAsesor ? Number(form.idAsesor) : null,
      nombres: form.nombres.trim(),
      apellidos: form.apellidos.trim(),
      dni: form.dni.trim(),
      correo: form.correo.trim(),
      telefono: form.telefono.trim(),
      direccion: form.direccion.trim(),
      observacion: form.observacion.trim(),
      montoDeuda: form.montoDeuda ? Number(form.montoDeuda) : undefined,
      fechaEmision: !clienteEditar
        ? new Date().toISOString().split("T")[0]
        : undefined,
      fechaVencimiento: form.fechaVencimiento || undefined,
      descripcionDeuda: !clienteEditar
        ? "Deuda inicial del cliente"
        : undefined,
    });
  };

  return (
    <div className="modal-overlay">
      <div className="modal-card">
        <h2 className="modal-title">
          {clienteEditar ? "Editar Cliente" : "Nuevo Cliente"}
        </h2>

        <div className="form-row">
          <div className="form-field">
            <label>Nombres *</label>
            <input
              name="nombres"
              value={form.nombres}
              onChange={handleChange}
            />
            {errores.nombres && (
              <span className="form-error">{errores.nombres}</span>
            )}
          </div>

          <div className="form-field">
            <label>Apellidos *</label>
            <input
              name="apellidos"
              value={form.apellidos}
              onChange={handleChange}
            />
            {errores.apellidos && (
              <span className="form-error">{errores.apellidos}</span>
            )}
          </div>
        </div>

        <div className="form-row">
          <div className="form-field">
            <label>DNI *</label>
            <input
              name="dni"
              value={form.dni}
              onChange={handleChange}
              maxLength={8}
            />
            {errores.dni && <span className="form-error">{errores.dni}</span>}
          </div>

          <div className="form-field">
            <label>Teléfono *</label>
            <input
              name="telefono"
              value={form.telefono}
              onChange={handleChange}
            />
            {errores.telefono && (
              <span className="form-error">{errores.telefono}</span>
            )}
          </div>
        </div>

        <div className="form-field">
          <label>Correo</label>
          <input name="correo" value={form.correo} onChange={handleChange} />
          {errores.correo && (
            <span className="form-error">{errores.correo}</span>
          )}
        </div>

        <div className="form-field">
          <label>Dirección</label>
          <input
            name="direccion"
            value={form.direccion}
            onChange={handleChange}
          />
        </div>

        <div className="form-field">
          <label>Asesor Asignado</label>
          <select name="idAsesor" value={form.idAsesor} onChange={handleChange}>
            <option value="">Seleccionar asesor</option>
            {asesores.map((asesor) => (
              <option key={asesor.idUsuario} value={asesor.idUsuario}>
                {asesor.nombres} {asesor.apellidos}
              </option>
            ))}
          </select>
          {errores.idAsesor && (
            <span className="form-error">{errores.idAsesor}</span>
          )}
        </div>

        <div className="form-row">
          <div className="form-field">
            <label>Deuda Total *</label>
            <input
              type="number"
              name="montoDeuda"
              value={form.montoDeuda}
              onChange={handleChange}
              placeholder="Ej. 15000"
            />
            {errores.montoDeuda && (
              <span className="form-error">{errores.montoDeuda}</span>
            )}
          </div>

          <div className="form-field">
            <label>Fecha de Pago *</label>
            <input
              type="date"
              name="fechaVencimiento"
              value={form.fechaVencimiento}
              onChange={handleChange}
            />
            {errores.fechaVencimiento && (
              <span className="form-error">{errores.fechaVencimiento}</span>
            )}
          </div>
        </div>

        <div className="form-field">
          <label>Observación</label>
          <textarea
            name="observacion"
            value={form.observacion}
            onChange={handleChange}
          />
        </div>

        <div className="modal-actions">
          <button className="btn-cancelar" onClick={onCancelar}>
            Cancelar
          </button>
          <button className="btn-confirmar" onClick={handleSubmit}>
            {clienteEditar ? "Actualizar cliente" : "Crear cliente"}
          </button>
        </div>
      </div>
    </div>
  );
};

export default ClienteForm;
