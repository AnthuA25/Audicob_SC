import { useState } from "react";
import { crearAsesor } from "../../api/asesorApi";
import "../../styles/asesor-form.css";

export default function AsesorForm({ onSuccess, onCancel }) {
  // Estado del formulario
  const [formData, setFormData] = useState({
    nombreCompleto: "",
    email: "",
    dni: "",
    telefono: "",
  });

  // Estado de errores de validación
  const [errors, setErrors] = useState({
    nombreCompleto: "",
    email: "",
    dni: "",
    telefono: "",
  });

  // Errores del servidor
  const [serverError, setServerError] = useState("");
  const [loading, setLoading] = useState(false);

  // Validación de formato de email
  const isValidEmail = (email) => {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
  };

  // Validación de DNI (exactamente 8 dígitos)
  const isValidDNI = (dni) => {
    const regex = /^\d{8}$/;
    return regex.test(dni);
  };

  // Validar formulario
  const validateForm = () => {
    const newErrors = {
      nombreCompleto: "",
      email: "",
      dni: "",
      telefono: "",
    };

    let isValid = true;

    // Validar nombre completo
    if (!formData.nombreCompleto.trim()) {
      newErrors.nombreCompleto = "El nombre completo es requerido";
      isValid = false;
    }

    // Validar email
    if (!formData.email.trim()) {
      newErrors.email = "El email es requerido";
      isValid = false;
    } else if (!isValidEmail(formData.email)) {
      newErrors.email = "Formato inválido";
      isValid = false;
    }

    // Validar DNI
    if (!formData.dni.trim()) {
      newErrors.dni = "El DNI es requerido";
      isValid = false;
    } else if (!isValidDNI(formData.dni)) {
      newErrors.dni = "Formato inválido";
      isValid = false;
    }

    // Validar teléfono (opcional, pero si se ingresa debe ser válido)
    if (formData.telefono.trim() && !/^\d+$/.test(formData.telefono)) {
      newErrors.telefono = "Formato inválido";
      isValid = false;
    }

    setErrors(newErrors);
    return isValid;
  };

  // Manejar cambios en los campos
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));

    // Limpiar error del campo cuando el usuario empieza a escribir
    if (errors[name]) {
      setErrors((prev) => ({
        ...prev,
        [name]: "",
      }));
    }

    // Limpiar error del servidor cuando el usuario empieza a editar
    if (serverError) {
      setServerError("");
    }
  };

  // Manejar envío del formulario
  const handleSubmit = async (e) => {
    e.preventDefault();
    setServerError("");

    // Validar antes de enviar
    if (!validateForm()) {
      return;
    }

    setLoading(true);

    try {
      // Separar nombre y apellido (asumimos formato "Nombre Apellido")
      const nombreParts = formData.nombreCompleto.trim().split(" ");
      const nombres = nombreParts[0];
      const apellidos = nombreParts.slice(1).join(" ") || "S/A";

      // El rol ID 2 es para "Asesor" (ajustar si es diferente)
      const asesorData = {
        idRol: 2, // ID del rol Asesor
        nombres,
        apellidos,
        dni: formData.dni.trim(),
        correo: formData.email.trim().toLowerCase(),
        telefono: formData.telefono.trim() || null,
        password: formData.dni.trim(), // Contraseña temporal: el DNI
      };

      await crearAsesor(asesorData);

      // Limpiar formulario y llamar a callback de éxito
      setFormData({
        nombreCompleto: "",
        email: "",
        dni: "",
        telefono: "",
      });

      if (onSuccess) {
        onSuccess();
      }
    } catch (error) {
      const message =
        error.response?.data?.message ||
        "Error al crear el asesor. Intenta nuevamente.";
      setServerError(message);
    } finally {
      setLoading(false);
    }
  };

  // Manejar cancelación
  const handleCancel = () => {
    setFormData({
      nombreCompleto: "",
      email: "",
      dni: "",
      telefono: "",
    });
    setErrors({
      nombreCompleto: "",
      email: "",
      dni: "",
      telefono: "",
    });
    setServerError("");

    if (onCancel) {
      onCancel();
    }
  };

  return (
    <div className="asesor-form-container">
      <div className="asesor-form">
        <h1>Nuevo Asesor de Cobranza</h1>

        {serverError && <div className="alert alert-error">{serverError}</div>}

        <form onSubmit={handleSubmit}>
          {/* Nombre Completo */}
          <div className="form-group">
            <label htmlFor="nombreCompleto">Nombre Completo</label>
            <input
              type="text"
              id="nombreCompleto"
              name="nombreCompleto"
              value={formData.nombreCompleto}
              onChange={handleChange}
              disabled={loading}
              className={errors.nombreCompleto ? "input-error" : ""}
              placeholder="Ingresa nombre y apellido"
            />
            {errors.nombreCompleto && (
              <span className="error-message">{errors.nombreCompleto}</span>
            )}
          </div>

          {/* Email */}
          <div className="form-group">
            <label htmlFor="email">Email</label>
            <input
              type="email"
              id="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              disabled={loading}
              className={errors.email ? "input-error" : ""}
              placeholder="correo@ejemplo.com"
            />
            {errors.email && (
              <span className="error-message">{errors.email}</span>
            )}
          </div>

          {/* DNI y Teléfono */}
          <div className="form-row">
            <div className="form-group">
              <label htmlFor="dni">DNI</label>
              <input
                type="text"
                id="dni"
                name="dni"
                value={formData.dni}
                onChange={handleChange}
                disabled={loading}
                className={errors.dni ? "input-error" : ""}
                placeholder="12345678"
                maxLength="8"
              />
              {errors.dni && (
                <span className="error-message">{errors.dni}</span>
              )}
            </div>

            <div className="form-group">
              <label htmlFor="telefono">Teléfono</label>
              <input
                type="tel"
                id="telefono"
                name="telefono"
                value={formData.telefono}
                onChange={handleChange}
                disabled={loading}
                className={errors.telefono ? "input-error" : ""}
                placeholder="987654321"
              />
              {errors.telefono && (
                <span className="error-message">{errors.telefono}</span>
              )}
            </div>
          </div>

          {/* Botones */}
          <div className="form-actions">
            <button
              type="button"
              className="btn btn-secondary"
              onClick={handleCancel}
              disabled={loading}
            >
              Cancelar
            </button>
            <button
              type="submit"
              className="btn btn-primary"
              disabled={loading}
            >
              {loading ? "Creando..." : "Crear Asesor"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
