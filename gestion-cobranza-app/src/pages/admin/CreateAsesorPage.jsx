import { useNavigate } from "react-router-dom";
import { useState } from "react";
import AsesorForm from "../../components/forms/AsesorForm";
import "../../styles/create-asesor-page.css";

export default function CreateAsesorPage() {
  const navigate = useNavigate();
  const [successMessage, setSuccessMessage] = useState("");

  const handleSuccess = () => {
    setSuccessMessage("Asesor creado correctamente");

    // Mostrar el mensaje de éxito durante 2 segundos
    setTimeout(() => {
      // Redirigir a la lista de asesores
      navigate("/asesores");
    }, 2000);
  };

  const handleCancel = () => {
    // Redirigir a la lista de asesores sin guardar
    navigate("/asesores");
  };

  return (
    <div className="create-asesor-page">
      {successMessage && (
        <div className="toast toast-success">
          <span className="toast-icon">✓</span>
          {successMessage}
        </div>
      )}

      <AsesorForm onSuccess={handleSuccess} onCancel={handleCancel} />
    </div>
  );
}
