import {
  Users,
  BadgeDollarSign,
  TrendingUp,
  AlertTriangle,
} from "lucide-react";
import useDashboardAsesor from "../../hooks/useDashboardAsesor";
import MetricCard from "../../components/dashboard/MetricCard";
import CobranzaLineChart from "../../components/dashboard/CobranzaLineChart";
import ClasificacionDeudoresChart from "../../components/dashboard/ClasificacionDeudoresChart";
import MorosidadBarChart from "../../components/dashboard/MorosidadBarChart";

const datosEjemplo = {
  metricas: {
    totalClientes: 238,
    deudasPendientes: "S/. 45,280",
    pagosRealizados: "S/. 73,000",
    clientesMorosidad: 40,
    variacionClientes: "+12 este mes",
    variacionDeudas: "-8% vs mes anterior",
    variacionPagos: "+15% este mes",
    variacionMorosidad: "-5 vs semana pasada",
  },
  distribucionClientes: [
    { mes: "Ene", cobranza: 40 },
    { mes: "Feb", cobranza: 28 },
    { mes: "Mar", cobranza: 32 },
    { mes: "Abr", cobranza: 25 },
    { mes: "May", cobranza: 78 },
    { mes: "Jun", cobranza: 80 },
  ],
  clasificacionDeudores: [
    { name: "Al día 65%", valor: 65 },
    { name: "Atraso leve 18%", valor: 18 },
    { name: "Morosidad 12%", valor: 12 },
    { name: "Crítico 5%", valor: 5 },
  ],
  tendenciaMorosidad: [
    { mes: "Ene", morosidad: 950 },
    { mes: "Feb", morosidad: 860 },
    { mes: "Mar", morosidad: 340 },
    { mes: "Abr", morosidad: 200 },
    { mes: "May", morosidad: 400 },
    { mes: "Jun", morosidad: 590 },
  ],
};

const DashboardAsesorPage = () => {
  const {
    metricas,
    distribucionClientes,
    clasificacionDeudores,
    tendenciaMorosidad,
    loading,
    error,
  } = useDashboardAsesor();

  const m = metricas && !error ? metricas : datosEjemplo.metricas;
  const dc =
    distribucionClientes.length > 0 && !error
      ? distribucionClientes
      : datosEjemplo.distribucionClientes;
  const cd =
    clasificacionDeudores.length > 0 && !error
      ? clasificacionDeudores
      : datosEjemplo.clasificacionDeudores;
  const tm =
    tendenciaMorosidad.length > 0 && !error
      ? tendenciaMorosidad
      : datosEjemplo.tendenciaMorosidad;

  if (loading) return <div>Cargando dashboard...</div>;

  return (
    <div>
      <div className="dashboard-header">
        <h1>Panel de Asesor</h1>
        <p>Tu rendimiento y clientes asignados</p>
      </div>

      {error && <p style={{ color: "red" }}>{error}</p>}

      <div className="metricas-grid">
        <MetricCard
          icono={<Users size={20} color="#6366f1" />}
          valor={m.totalClientes}
          label="Total de clientes"
          variacion={m.variacionClientes}
        />
        <MetricCard
          icono={<BadgeDollarSign size={20} color="#ef4444" />}
          valor={m.deudasPendientes}
          label="Deudas pendientes"
          variacion={m.variacionDeudas}
        />
        <MetricCard
          icono={<TrendingUp size={20} color="#22c55e" />}
          valor={m.pagosRealizados}
          label="Pagos realizados"
          variacion={m.variacionPagos}
        />
        <MetricCard
          icono={<AlertTriangle size={20} color="#f59e0b" />}
          valor={m.clientesMorosidad}
          label="Clientes en morosidad"
          variacion={m.variacionMorosidad}
        />
      </div>

      <div className="charts-grid">
        <CobranzaLineChart
          data={dc}
          title="Distribución de Clientes"
          dataKey="clientes"
          color="#10b981"
        />
        <ClasificacionDeudoresChart data={cd} />
      </div>

      <MorosidadBarChart data={tm} />
    </div>
  );
};

export default DashboardAsesorPage;
