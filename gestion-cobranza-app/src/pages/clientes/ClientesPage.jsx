import { useState } from "react";
import { Search, Eye, Mail, Phone } from "lucide-react";
import useClientes from "../../hooks/useClientes";
import { EstadoBadge, RiesgoBadge } from "../../components/clientes/ClienteEstadoBadge";
import "../../styles/clientes.css";

const ClientesPage = () => {
  const { clientes } = useClientes();
  const [busqueda, setBusqueda] = useState("");

  const clientesFiltrados = clientes.filter(
    (c) =>
      c.nombre.toLowerCase().includes(busqueda.toLowerCase()) ||
      c.email.toLowerCase().includes(busqueda.toLowerCase()) ||
      c.id.toString().includes(busqueda.toLowerCase()),
  );

  const getDiasClass = (dias) => {
    if (dias >= 60) return "dias-atraso alto";
    if (dias >= 20) return "dias-atraso medio";
    return "dias-atraso bajo";
  };

  return (
    <div className="clientes-page">
      <div className="clientes-header">
        <div className="clientes-header-info">
          <h1>Gestión de clientes</h1>
        </div>
      </div>

      <div className="clientes-filtros">
        <div className="search-container">
          <Search size={18} className="search-icon" />
          <input
            className="search-input"
            placeholder="Buscar por nombre, email o ID..."
            value={busqueda}
            onChange={(e) => setBusqueda(e.target.value)}
          />
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
                    <span>
                      <Mail size={14} /> {cliente.email}
                    </span>
                    <span>
                      <Phone size={14} /> {cliente.telefono}
                    </span>
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
                  <button className="btn-ver" aria-label={`Ver ${cliente.nombre}`}>
                    <Eye size={16} />
                  </button>
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
