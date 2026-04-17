import { UserRound, Users, BadgeDollarSign, TrendingUp } from "lucide-react";
import useDashboard from "../../hooks/useDashboard";
import MetricCard from "../../components/dashboard/MetricCard";
import CobranzaLineChart from "../../components/dashboard/CobranzaLineChart";
import DistribucionClientesChart from "../../components/dashboard/DistribucionClientesChart";
import RendimientoAsesoresTable from "../../components/dashboard/RendimientoAsesoresTable";

const datosEjemplo = {
  metricas: {
    totalAsesores: 12,
    totalClientes: 1247,
    cobranzaTotal: "S./ 854,320",
    eficienciaGlobal: "87.5%",
    variacionAsesores: "+2 este mes",
    variacionClientes: "+156 este mes",
    variacionCobranza: "+18.2%",
    variacionEficiencia: "+3.2%",
  },
  cobranzaEvolucion: [
    { mes: "Enero", cobranza: 50 },
    { mes: "Febrero", cobranza: 75 },
    { mes: "Marzo", cobranza: 95 },
  ],
  distribucionClientes: [{ valor: 70 }, { valor: 20 }, { valor: 10 }],
  rendimientoAsesores: [
    {
      nombre: "María López",
      deudaGestionada: "S./ 125,000",
      clientes: 145,
      eficiencia: 92,
    },
    {
      nombre: "Juan Pérez",
      deudaGestionada: "S./ 98,500",
      clientes: 112,
      eficiencia: 82,
    },
    {
      nombre: "Sofía Torres",
      deudaGestionada: "S./ 71,200",
      clientes: 78,
      eficiencia: 79,
    },
  ],
};

const DashboardAdminPage = () => {
  const {
    metricas,
    cobranzaEvolucion,
    distribucionClientes,
    rendimientoAsesores,
    loading,
    error,
  } = useDashboard();

  const m = metricas && !error ? metricas : datosEjemplo.metricas;
  const ce =
    cobranzaEvolucion.length > 0 && !error
      ? cobranzaEvolucion
      : datosEjemplo.cobranzaEvolucion;
  const dc =
    distribucionClientes.length > 0 && !error
      ? distribucionClientes
      : datosEjemplo.distribucionClientes;
  const ra =
    rendimientoAsesores.length > 0 && !error
      ? rendimientoAsesores
      : datosEjemplo.rendimientoAsesores;
  if (loading) return <div>Cargando...</div>;

  return (
    <div>
      <div className="dashboard-header">
        <h1>Panel de Administración</h1>
        <p>Gestión y supervisión del sistema de cobranza</p>
      </div>

      <div className="metricas-grid">
        <MetricCard
          icono={<UserRound size={20} color="#6366f1" />}
          valor={m.totalAsesores}
          label="Total de Asesores"
          variacion={m.variacionAsesores}
        />
        <MetricCard
          icono={<Users size={20} color="#22c55e" />}
          valor={m.totalClientes}
          label="Total de Clientes"
          variacion={m.variacionClientes}
        />
        <MetricCard
          icono={<BadgeDollarSign size={20} color="#a855f7" />}
          valor={m.cobranzaTotal}
          label="Cobranza Total"
          variacion={m.variacionCobranza}
        />
        <MetricCard
          icono={<TrendingUp size={20} color="#f97316" />}
          valor={m.eficienciaGlobal}
          label="Eficiencia Global"
          variacion={m.variacionEficiencia}
        />
      </div>

      <div className="charts-grid">
        <CobranzaLineChart
          data={ce}
          title="Evolución de Cobranza"
          dataKey="cobranza"
          color="#3b82f6"
        />
        <DistribucionClientesChart data={dc} />
      </div>

      <RendimientoAsesoresTable data={ra} />
    </div>
  );
};

export default DashboardAdminPage;
