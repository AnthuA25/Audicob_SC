const MetricCard = ({ icono, valor, label, variacion }) => {
  return (
    <div className="metric-card">
      <div className="metric-icon-container">{icono}</div>
      <div className="metric-valor">{valor}</div>
      <div className="metric-label">{label}</div>
      {variacion && <div className="metric-variacion">{variacion}</div>}
    </div>
  );
};

export default MetricCard;