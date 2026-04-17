import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from "recharts";

const MorosidadBarChart = ({ data }) => {
  return (
    <div className="chart-card">
      <p className="chart-title">Tendencia de morosidad</p>
      <ResponsiveContainer width="100%" height={240}>
        <BarChart data={data}>
          <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
          <XAxis dataKey="mes" tick={{ fontSize: 12 }} />
          <YAxis tick={{ fontSize: 12 }} />
          <Tooltip />
          <Bar dataKey="morosidad" fill="#b45309" />
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
};

export default MorosidadBarChart;