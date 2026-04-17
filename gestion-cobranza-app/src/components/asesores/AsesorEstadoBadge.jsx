const coloresEstado = {
  Activo: { bg: "#dcfce7", color: "#16a34a" },
  Inactivo: { bg: "#fee2e2", color: "#dc2626" },
};

export const AsesorEstadoBadge = ({ estado }) => {
  const estilo = coloresEstado[estado] || {
    bg: "#f1f5f9",
    color: "#64748b",
  };

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