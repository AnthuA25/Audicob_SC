import { useMemo, useState } from "react";
import { Search, UserPlus, Pencil, Trash2 } from "lucide-react";
import useClientes from "../../hooks/useClientes";
import ClienteForm from "../../components/forms/ClienteForm";
import {
  EstadoBadge,
  RiesgoBadge,
} from "../../components/clientes/ClienteEstadoBadge";
import "../../styles/clientes.css";

const ClientesPage = () => {
  const {
    clientes,
    loading,
    error,
    agregarCliente,
    editarCliente,
    borrarCliente,
    cargarClientes,
  } = useClientes();
  const [busqueda, setBusqueda] = useState("");
  const [modalNuevo, setModalNuevo] = useState(false);
  const [clienteEditar, setClienteEditar] = useState(null);
  const [clienteEliminar, setClienteEliminar] = useState(null);
  const [toast, setToast] = useState("");

  const mostrarToast = (mensaje) => {
    setToast(mensaje);
    setTimeout(() => setToast(""), 3000);
  };

  const clientesFiltrados = useMemo(() => {
    return clientes.filter(
      (c) =>
        c.nombre.toLowerCase().includes(busqueda.toLowerCase()) ||
        c.email.toLowerCase().includes(busqueda.toLowerCase()) ||
        c.dni.toLowerCase().includes(busqueda.toLowerCase()),
    );
  }, [clientes, busqueda]);

  const clientesActivos = clientes.filter((c) => c.estado !== "Moroso").length;
  const clientesMorosos = clientes.filter((c) => c.estado === "Moroso").length;

  const handleGuardar = async (form) => {
    try {
      if (clienteEditar) {
        await editarCliente(clienteEditar.id, {
          ...form,
          estadoCliente: clienteEditar.estado?.toUpperCase() || "NUEVO",
          riesgo: clienteEditar.riesgo?.toUpperCase() || "BAJO",
        });

        await cargarClientes();

        mostrarToast("Cliente actualizado exitosamente");
        setClienteEditar(null);
      } else {
        await agregarCliente(form);
        await cargarClientes();
        mostrarToast("Cliente creado exitosamente");
        setModalNuevo(false);
      }
    } catch (err) {
      mostrarToast(err.response?.data?.message || "Ocurrió un error.");
    }
  };

  const handleEliminar = async () => {
    try {
      await borrarCliente(clienteEliminar.id);
      mostrarToast("Cliente eliminado exitosamente");
      setClienteEliminar(null);
    } catch (err) {
      mostrarToast(err.response?.data?.message || "No se pudo eliminar.");
    }
  };

  if (loading) return <div>Cargando clientes...</div>;

  return (
    <div>
      <div className="clientes-header">
        <div className="clientes-header-info">
          <h1>Gestión de Clientes</h1>
          <p>Administra la cartera completa de clientes</p>
        </div>
        <button className="btn-nuevo" onClick={() => setModalNuevo(true)}>
          <UserPlus size={16} /> Nuevo Cliente
        </button>
      </div>
      {error && <p style={{ color: "red" }}>{error}</p>}

      <div className="clientes-filtros">
        <div className="search-container">
          <Search size={16} className="search-icon" />
          <input
            className="search-input"
            placeholder="Buscar por nombre, email o ID..."
            value={busqueda}
            onChange={(e) => setBusqueda(e.target.value)}
          />
        </div>
      </div>

      <div className="clientes-stats">
        <div className="stat-card activos">
          <p>Clientes Activos</p>
          <span>{clientesActivos}</span>
        </div>
        <div className="stat-card morosos">
          <p>Clientes Morosos</p>
          <span>{clientesMorosos}</span>
        </div>
      </div>

      <div className="clientes-table-container">
        <table className="clientes-table">
          <thead>
            <tr>
              <th>Cliente</th>
              <th>Contacto</th>
              <th>Asesor Asignado</th>
              <th>Deuda Total</th>
              <th>Días Atraso</th>
              <th>Riesgo</th>
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {clientesFiltrados.map((cliente) => (
              <tr key={cliente.id}>
                <td>{cliente.nombre}</td>
                <td>
                  <div className="cliente-contacto">
                    <span>✉ {cliente.email || "-"}</span>
                    <span>📞 {cliente.telefono || "-"}</span>
                  </div>
                </td>
                <td>{cliente.asesorAsignado || "-"}</td>
                <td>
                  {cliente.deudaTotalTexto || "-"}
                </td>
                <td>
                  <span className="dias-atraso pendiente">
                    {cliente.diasAtraso > 0
                      ? `${cliente.diasAtraso} días`
                      : "-"}
                  </span>
                </td>
                <td>
                  <RiesgoBadge riesgo={cliente.riesgo} />
                </td>
                <td>
                  <EstadoBadge estado={cliente.estado} />
                </td>
                <td>
                  <div className="acciones">
                    <button
                      className="btn-editar"
                      onClick={() => setClienteEditar(cliente)}
                    >
                      <Pencil size={15} />
                    </button>
                    <button
                      className="btn-eliminar"
                      onClick={() => setClienteEliminar(cliente)}
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

      {(modalNuevo || clienteEditar) && (
        <ClienteForm
          clienteEditar={clienteEditar}
          clientesExistentes={clientes}
          onGuardar={handleGuardar}
          onCancelar={() => {
            setModalNuevo(false);
            setClienteEditar(null);
          }}
        />
      )}

      {clienteEliminar && (
        <div className="modal-overlay">
          <div className="modal-card">
            <h2 className="modal-title">¿Estás seguro?</h2>
            <p className="modal-delete-text">
              Esta acción no se puede deshacer. El cliente y todos sus datos
              asociados serán eliminados permanentemente del sistema.
            </p>
            <div className="modal-actions">
              <button
                className="btn-cancelar"
                onClick={() => setClienteEliminar(null)}
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

export default ClientesPage;
