import { useMemo, useState } from "react";
import { Search, Eye } from "lucide-react";
import { useNavigate } from "react-router-dom";
import useMisClientes from "../../hooks/useMisClientes";
import {
  EstadoBadge,
  RiesgoBadge,
} from "../../components/clientes/ClienteEstadoBadge";
import "../../styles/clientes.css";

const MisClientesPage = () => {
  const { clientes, loading, error } = useMisClientes();
  const [busqueda, setBusqueda] = useState("");
  const navigate = useNavigate();

  const clientesFiltrados = useMemo(() => {
    return clientes.filter(
      (c) =>
        c.nombre.toLowerCase().includes(busqueda.toLowerCase()) ||
        c.email.toLowerCase().includes(busqueda.toLowerCase()) ||
        c.dni.toLowerCase().includes(busqueda.toLowerCase())
    );
  }, [clientes, busqueda]);

  const getDiasClass = (dias) => {
    if (dias >= 60) return "dias-atraso alto";
    if (dias >= 20) return "dias-atraso medio";
    if (dias > 0) return "dias-atraso bajo";
    return "dias-atraso pendiente";
  };

  if (loading) return <div>Cargando mis clientes...</div>;

  return (
    <div>
      <div className="clientes-header">
        <div className="clientes-header-info">
          <h1>Mis Clientes</h1>
          <p>Visualiza los clientes asignados a tu cartera</p>
        </div>
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

      <div className="clientes-table-container">
        <table className="clientes-table">
          <thead>
            <tr>
              <th>Cliente</th>
              <th>Contacto</th>
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
                    <span>✉ {cliente.email || "-"}</span>
                    <span>📞 {cliente.telefono || "-"}</span>
                    <span>DNI: {cliente.dni}</span>
                  </div>
                </td>
                <td>{cliente.deudaPendiente || "-"}</td>
                <td>
                  <span className={getDiasClass(cliente.diasAtraso)}>
                    {cliente.diasAtraso !== "" ? `${cliente.diasAtraso} días` : "-"}
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
                      onClick={() => navigate(`/mis-clientes/${cliente.id}`)}
                      title="Ver detalle"
                    >
                      <Eye size={16} />
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

export default MisClientesPage;