const coloresEstado = {
  CONTACTADO: { bg: "#ede9fe", color: "#7c3aed" },
  NEGOCIACION: { bg: "#fef9c3", color: "#ca8a04" },
  "PROMESA DE PAGO": { bg: "#dbeafe", color: "#2563eb" },
  PAGADO: { bg: "#dcfce7", color: "#16a34a" },
  MOROSO: { bg: "#fee2e2", color: "#dc2626" },
};

const coloresRiesgo = {
  ALTO: { bg: "#fee2e2", color: "#dc2626" },
  MEDIO: { bg: "#fef9c3", color: "#ca8a04" },
  BAJO: { bg: "#dcfce7", color: "#16a34a" },
};

export const EstadoBadge = ({ estado }) => {

   const estadoNormalizado = estado?.toUpperCase();
  
  const estilo = coloresEstado[estadoNormalizado] || { bg: "#f1f5f9", color: "#64748b" };
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
  const riesgoNormalizado = riesgo?.toUpperCase();
  const estilo = coloresRiesgo[riesgoNormalizado] || { bg: "#f1f5f9", color: "#64748b" };
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