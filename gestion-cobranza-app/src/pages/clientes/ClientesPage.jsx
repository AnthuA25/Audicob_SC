import { useState } from "react";
import { Search, UserPlus, Pencil, Trash2 } from "lucide-react";
import useClientes from "../../hooks/useClientes";
import {
  EstadoBadge,
  RiesgoBadge,
} from "../../components/clientes/ClienteEstadoBadge";
import "../../styles/clientes.css";

const ClientesPage = () => {
  const { clientes } = useClientes();

  const [busqueda, setBusqueda] = useState("");

  const clientesFiltrados = clientes.filter(
    (c) =>
      c.nombre.toLowerCase().includes(busqueda.toLowerCase()) ||
      c.email.toLowerCase().includes(busqueda.toLowerCase()),
  );

  const clientesActivos = clientes.filter((c) => c.estado !== "Moroso").length;
  const clientesMorosos = clientes.filter((c) => c.estado === "Moroso").length;

  const getDiasClass = (dias) => {
    if (dias >= 60) return "dias-atraso alto";
    if (dias >= 20) return "dias-atraso medio";
    return "dias-atraso bajo";
  };

  return (
    <div>
      <div className="clientes-header">
        <div className="clientes-header-info">
          <h1>Gestión de Clientes</h1>
          <p>Administra la cartera completa de clientes</p>
        </div>
        <button className="btn-nuevo">
          <UserPlus size={16} /> Nuevo Cliente
        </button>
      </div>

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
              <th>Deuda Pendiente</th>
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
                    <span>✉ {cliente.email}</span>
                    <span>📞 {cliente.telefono}</span>
                  </div>
                </td>
                <td>{cliente.asesorAsignado}</td>
                <td>{cliente.deudaPendiente}</td>
                <td>
                  <span className={getDiasClass(cliente.diasAtraso)}>
                    {cliente.diasAtraso} días
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
                    <button className="btn-editar">
                      <Pencil size={15} />
                    </button>
                    <button className="btn-eliminar">
                      <Trash2 size={15} />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default ClientesPage;
