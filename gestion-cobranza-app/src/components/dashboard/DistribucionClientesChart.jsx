import {
  PieChart,
  Pie,
  Cell,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from "recharts";

const COLORS = ["#22c55e", "#f59e0b", "#ef4444"];

const LABELS = ["Al Día", "Alerta", "Morosos"];

const DistribucionClientesChart = ({ data }) => {
  const dataConNombre = data.map((item, index) => ({
    ...item,
    name: LABELS[index],
  }));

  return (
    <div className="chart-card">
      <p className="chart-title">Distribución de Clientes</p>
      <ResponsiveContainer width="100%" height={200}>
        <PieChart>
          <Pie
            data={dataConNombre}
            cx="50%"
            cy="50%"
            innerRadius={55}
            outerRadius={85}
            dataKey="valor"
          >
            {dataConNombre.map((entry, index) => (
              <Cell key={`cell-${index}`} fill={COLORS[index]} />
            ))}
          </Pie>
          <Tooltip />
          <Legend />
        </PieChart>
      </ResponsiveContainer>
    </div>
  );
};

export default DistribucionClientesChart;