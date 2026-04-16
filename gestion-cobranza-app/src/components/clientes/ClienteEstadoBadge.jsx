const coloresEstado = {
  Contactado: { bg: "#ede9fe", color: "#7c3aed" },
  Negociación: { bg: "#fef9c3", color: "#ca8a04" },
  "Promesa de Pago": { bg: "#dbeafe", color: "#2563eb" },
  Pagado: { bg: "#dcfce7", color: "#16a34a" },
  Moroso: { bg: "#fee2e2", color: "#dc2626" },
};

const coloresRiesgo = {
  Alto: { bg: "#fee2e2", color: "#dc2626" },
  Medio: { bg: "#fef9c3", color: "#ca8a04" },
  Bajo: { bg: "#dcfce7", color: "#16a34a" },
};

export const EstadoBadge = ({ estado }) => {
  const estilo = coloresEstado[estado] || { bg: "#f1f5f9", color: "#64748b" };
  return (
    <span
      style={{
        background: estilo.bg,
        color: estilo.color,
        padding: "4px 10px",
        borderRadius: "20px",
        fontSize: "12px",
        fontWeight: "500",
      }}
    >
      {estado}
    </span>
  );
};

export const RiesgoBadge = ({ riesgo }) => {
  const estilo = coloresRiesgo[riesgo] || { bg: "#f1f5f9", color: "#64748b" };
  return (
    <span
      style={{
        background: estilo.bg,
        color: estilo.color,
        padding: "4px 10px",
        borderRadius: "20px",
        fontSize: "12px",
        fontWeight: "500",
      }}
    >
      {riesgo}
    </span>
  );
};