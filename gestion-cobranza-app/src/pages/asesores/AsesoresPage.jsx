import { useMemo, useState } from "react";
import { Search, UserPlus, Pencil, Trash2 } from "lucide-react";
import useAsesores from "../../hooks/useAsesores";
import AsesorForm from "../../components/forms/AsesorForm";
import { AsesorEstadoBadge } from "../../components/asesores/AsesorEstadoBadge";
import "../../styles/clientes.css";

const AsesoresPage = () => {
  const {
    asesores,
    loading,
    error,
    agregarAsesor,
    editarAsesor,
    borrarAsesor,
  } = useAsesores();

  const [busqueda, setBusqueda] = useState("");
  const [modalNuevo, setModalNuevo] = useState(false);
  const [asesorEditar, setAsesorEditar] = useState(null);
  const [asesorEliminar, setAsesorEliminar] = useState(null);
  const [toast, setToast] = useState("");

  const mostrarToast = (mensaje) => {
    setToast(mensaje);
    setTimeout(() => setToast(""), 3000);
  };

  const asesoresFiltrados = useMemo(() => {
    return asesores.filter(
      (a) =>
        a.nombre.toLowerCase().includes(busqueda.toLowerCase()) ||
        a.correo.toLowerCase().includes(busqueda.toLowerCase()) ||
        a.dni.toLowerCase().includes(busqueda.toLowerCase()),
    );
  }, [asesores, busqueda]);

  const asesoresActivos = asesores.filter((a) => a.estado === "Activo").length;
  const asesoresInactivos = asesores.filter(
    (a) => a.estado === "Inactivo",
  ).length;

  const handleGuardar = async (form) => {
    try {
      if (asesorEditar) {
        await editarAsesor(asesorEditar.id, form);
        mostrarToast("Asesor actualizado exitosamente");
        setAsesorEditar(null);
      } else {
        await agregarAsesor(form);
        mostrarToast("Asesor creado exitosamente");
        setModalNuevo(false);
      }
    } catch (err) {
      mostrarToast(err.response?.data?.message || "Ocurrió un error.");
    }
  };

  const handleEliminar = async () => {
    try {
      await borrarAsesor(asesorEliminar.id);
      mostrarToast("Asesor eliminado exitosamente");
      setAsesorEliminar(null);
    } catch (err) {
      mostrarToast(err.response?.data?.message || "No se pudo eliminar.");
    }
  };

  if (loading) return <div>Cargando asesores...</div>;

  return (
    <div>
      <div className="clientes-header">
        <div className="clientes-header-info">
          <h1>Gestión de Asesores</h1>
          <p>Administra los asesores de cobranza del sistema</p>
        </div>

        <button className="btn-nuevo" onClick={() => setModalNuevo(true)}>
          <UserPlus size={16} /> Nuevo Asesor
        </button>
      </div>

      {error && <p style={{ color: "red" }}>{error}</p>}

      <div className="clientes-filtros">
        <div className="search-container">
          <Search size={16} className="search-icon" />
          <input
            className="search-input"
            placeholder="Buscar por nombre, correo o DNI..."
            value={busqueda}
            onChange={(e) => setBusqueda(e.target.value)}
          />
        </div>
      </div>

      <div className="clientes-stats">
        <div className="stat-card activos">
          <p>Asesores Activos</p>
          <span>{asesoresActivos}</span>
        </div>
        <div className="stat-card morosos">
          <p>Asesores Inactivos</p>
          <span>{asesoresInactivos}</span>
        </div>
      </div>

      <div className="clientes-table-container">
        <table className="clientes-table">
          <thead>
            <tr>
              <th>Asesor</th>
              <th>Contacto</th>
              <th>DNI</th>
              <th>Clientes</th>
              <th>Deuda Gestionada</th>
              <th>Pagos Recuperados</th>
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {asesoresFiltrados.map((asesor) => (
              <tr key={asesor.id}>
                <td>{asesor.nombre}</td>
                <td>
                  <div className="cliente-contacto">
                    <span>✉ {asesor.correo || "-"}</span>
                    <span>📞 {asesor.telefono || "-"}</span>
                  </div>
                </td>
                <td>{asesor.dni}</td>
                <td>{asesor.clientesAsignados}</td>
                <td>S/. {Number(asesor.deudaGestionada).toLocaleString()}</td>
                <td>S/. {Number(asesor.pagosRecuperados).toLocaleString()}</td>
                <td>
                  <AsesorEstadoBadge estado={asesor.estado} />
                </td>
                <td>
                  <div className="acciones">
                    <button
                      className="btn-editar"
                      onClick={() => setAsesorEditar(asesor)}
                    >
                      <Pencil size={15} />
                    </button>
                    <button
                      className="btn-eliminar"
                      onClick={() => setAsesorEliminar(asesor)}
                    >
                      <Trash2 size={15} />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {(modalNuevo || asesorEditar) && (
        <AsesorForm
          asesorEditar={asesorEditar}
          asesoresExistentes={asesores}
          onGuardar={handleGuardar}
          onCancelar={() => {
            setModalNuevo(false);
            setAsesorEditar(null);
          }}
        />
      )}

      {asesorEliminar && (
        <div className="modal-overlay">
          <div className="modal-card">
            <h2 className="modal-title">¿Estás seguro?</h2>
            <p className="modal-delete-text">
              Esta acción realizará una eliminación lógica del asesor.
            </p>
            <div className="modal-actions">
              <button
                className="btn-cancelar"
                onClick={() => setAsesorEliminar(null)}
              >
                Cancelar
              </button>
              <button
                className="btn-eliminar-confirmar"
                onClick={handleEliminar}
              >
                Eliminar
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

export default AsesoresPage;
