const getEstadoColor = (eficiencia) => {
  if (eficiencia >= 90) return "estado-excelente";
  if (eficiencia >= 75) return "estado-bueno";
  return "estado-regular";
};

const getBarraColor = (eficiencia) => {
  if (eficiencia >= 90) return "#22c55e";
  if (eficiencia >= 75) return "#3b82f6";
  return "#f59e0b";
};

const getEstadoLabel = (eficiencia) => {
  if (eficiencia >= 90) return "Excelente";
  if (eficiencia >= 75) return "Bueno";
  return "Regular";
};

const RendimientoAsesoresTable = ({ data }) => {
  return (
    <div className="rendimiento-card">
      <p className="rendimiento-title">Rendimiento de Asesores</p>
      <table className="rendimiento-table">
        <thead>
          <tr>
            <th>Asesor</th>
            <th>Deuda Gestionada</th>
            <th>Clientes</th>
            <th>Eficiencia</th>
            <th>Estado</th>
          </tr>
        </thead>
        <tbody>
          {data.map((asesor, index) => (
            <tr key={index}>
              <td>{asesor.nombre}</td>
              <td>{asesor.deudaGestionada}</td>
              <td>{asesor.clientes}</td>
              <td>
                <div className="eficiencia-container">
                  <div className="eficiencia-barra-bg">
                    <div
                      className="eficiencia-barra"
                      style={{
                        width: `${asesor.eficiencia}%`,
                        backgroundColor: getBarraColor(asesor.eficiencia),
                      }}
                    />
                  </div>
                  <span>{asesor.eficiencia}%</span>
                </div>
              </td>
              <td>
                <span
                  className={`estado-badge ${getEstadoColor(asesor.eficiencia)}`}
                >
                  {getEstadoLabel(asesor.eficiencia)}
                </span>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default RendimientoAsesoresTable;
