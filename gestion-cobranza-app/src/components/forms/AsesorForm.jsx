import { useEffect, useState } from "react";

const AsesorForm = ({
  asesorEditar,
  onGuardar,
  onCancelar,
  asesoresExistentes = [],
}) => {
  const [form, setForm] = useState({
    nombres: "",
    apellidos: "",
    dni: "",
    correo: "",
    telefono: "",
    password: "",
    estado: "ACTIVO",
  });

  const [errores, setErrores] = useState({});

  useEffect(() => {
    if (asesorEditar) {
      setForm({
        nombres: asesorEditar.nombres || "",
        apellidos: asesorEditar.apellidos || "",
        dni: asesorEditar.dni || "",
        correo: asesorEditar.correo || "",
        telefono: asesorEditar.telefono || "",
        password: "",
        estado: (asesorEditar.estado || "ACTIVO").toUpperCase(),
      });
    } else {
      setForm({
        nombres: "",
        apellidos: "",
        dni: "",
        correo: "",
        telefono: "",
        password: "",
        estado: "ACTIVO",
      });
    }
  }, [asesorEditar]);

  const handleChange = (e) => {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
    setErrores((prev) => ({ ...prev, [e.target.name]: "" }));
  };

  const validar = () => {
    const nuevosErrores = {};

    if (!form.nombres.trim()) nuevosErrores.nombres = "Los nombres son obligatorios.";
    if (!form.apellidos.trim()) nuevosErrores.apellidos = "Los apellidos son obligatorios.";

    if (!form.dni.trim()) {
      nuevosErrores.dni = "El DNI es obligatorio.";
    } else if (!/^\d{8}$/.test(form.dni)) {
      nuevosErrores.dni = "El DNI debe tener 8 dígitos.";
    } else if (
      !asesorEditar &&
      asesoresExistentes.some((a) => a.dni === form.dni)
    ) {
      nuevosErrores.dni = "Ya existe un asesor con ese DNI.";
    }

    if (!form.correo.trim()) {
      nuevosErrores.correo = "El correo es obligatorio.";
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.correo)) {
      nuevosErrores.correo = "El correo no es válido.";
    }

    if (!form.telefono.trim()) {
      nuevosErrores.telefono = "El teléfono es obligatorio.";
    }

    if (!asesorEditar && !form.password.trim()) {
      nuevosErrores.password = "La contraseña es obligatoria.";
    } else if (form.password && form.password.length < 6) {
      nuevosErrores.password = "La contraseña debe tener al menos 6 caracteres.";
    }

    if (!form.estado.trim()) {
      nuevosErrores.estado = "El estado es obligatorio.";
    }

    return nuevosErrores;
  };

  const handleSubmit = () => {
    const nuevosErrores = validar();

    if (Object.keys(nuevosErrores).length > 0) {
      setErrores(nuevosErrores);
      return;
    }

    onGuardar({
      nombres: form.nombres.trim(),
      apellidos: form.apellidos.trim(),
      dni: form.dni.trim(),
      correo: form.correo.trim(),
      telefono: form.telefono.trim(),
      password: form.password.trim(),
      estado: form.estado.trim().toUpperCase(),
    });
  };

  return (
    <div className="modal-overlay">
      <div className="modal-card">
        <h2 className="modal-title">
          {asesorEditar ? "Editar Asesor" : "Nuevo Asesor de Cobranza"}
        </h2>

        <div className="form-row">
          <div className="form-field">
            <label>Nombres *</label>
            <input
              name="nombres"
              value={form.nombres}
              onChange={handleChange}
              style={errores.nombres ? { borderColor: "#ef4444" } : {}}
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
              style={errores.apellidos ? { borderColor: "#ef4444" } : {}}
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
          <label>Correo *</label>
          <input
            name="correo"
            value={form.correo}
            onChange={handleChange}
            style={errores.correo ? { borderColor: "#ef4444" } : {}}
          />
          {errores.correo && (
            <span className="form-error">{errores.correo}</span>
          )}
        </div>

        <div className="form-row">
          <div className="form-field">
            <label>{asesorEditar ? "Nueva contraseña" : "Contraseña"} {!asesorEditar ? "*" : ""}</label>
            <input
              type="password"
              name="password"
              value={form.password}
              onChange={handleChange}
              style={errores.password ? { borderColor: "#ef4444" } : {}}
            />
            {errores.password && (
              <span className="form-error">{errores.password}</span>
            )}
          </div>

          <div className="form-field">
            <label>Estado *</label>
            <select
              name="estado"
              value={form.estado}
              onChange={handleChange}
              style={errores.estado ? { borderColor: "#ef4444" } : {}}
            >
              <option value="ACTIVO">ACTIVO</option>
              <option value="INACTIVO">INACTIVO</option>
            </select>
            {errores.estado && (
              <span className="form-error">{errores.estado}</span>
            )}
          </div>
        </div>

        <div className="modal-actions">
          <button className="btn-cancelar" onClick={onCancelar}>
            Cancelar
          </button>
          <button className="btn-confirmar" onClick={handleSubmit}>
            {asesorEditar ? "Actualizar asesor" : "Crear asesor"}
          </button>
        </div>
      </div>
    </div>
  );
};

export default AsesorForm;