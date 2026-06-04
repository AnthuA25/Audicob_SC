import axiosClient from "./axiosClient";

export const getResumenAlertasAdminApi = async () => {
  const response = await axiosClient.get("/Alertas/admin/resumen");
  return response.data;
};

export const getAlertasAdminApi = async (soloNoLeidas = false) => {
  const response = await axiosClient.get("/Alertas/admin", {
    params: soloNoLeidas ? { soloNoLeidas: true } : {},
  });
  return response.data;
};

export const getResumenAlertasAsesorApi = async () => {
  const response = await axiosClient.get("/Alertas/asesor/resumen");
  return response.data;
};

export const getAlertasAsesorApi = async (soloNoLeidas = false) => {
  const response = await axiosClient.get("/Alertas/asesor", {
    params: soloNoLeidas ? { soloNoLeidas: true } : {},
  });
  return response.data;
};

export const marcarAlertaLeidaApi = async (idAlerta) => {
  const response = await axiosClient.patch(
    `/Alertas/${idAlerta}/marcar-leida`
  );
  return response.data;
};