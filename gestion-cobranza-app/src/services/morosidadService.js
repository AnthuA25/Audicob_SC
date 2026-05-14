import { getDashboardMorosidadApi } from "../api/morosidadApi";

export const fetchDashboardMorosidad = async () => {
  const data = await getDashboardMorosidadApi();

  return {
    metricas: {
      clientesMorosos: data.clientesMorosos ?? 0,
      deudaMorosaTotal: `S/. ${Number(
        data.deudaMorosaTotal ?? 0,
      ).toLocaleString()}`,
      morosidadCritica: data.morosidadCritica ?? 0,
      promedioAtraso: `${data.promedioAtrasoDias ?? 0} días`,
    },

    morosidad: (data.detalleClientes ?? []).map((c) => ({
      id: c.idCliente,
      nombre: c.nombreCompleto,
      email: c.correo || "-",
      telefono: c.telefono || "-",
      asesorAsignado: c.asesorAsignado || "-",
      diasAtraso: c.diasAtraso ?? 0,
      deudaPendiente: `S/. ${Number(c.deudaPendiente ?? 0).toLocaleString()}`,
      riesgo: c.riesgo,
      estado: c.estado,
    })),
  };
};