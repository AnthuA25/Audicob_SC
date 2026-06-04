import axiosClient from "./axiosClient";

export const getNotificacionesAdminApi = async () => {
  const response = await axiosClient.get("/Alertas/admin", {
    params: { soloNoLeidas: true },
  });
  return response.data;
};

export const getNotificacionesAsesorApi = async () => {
  const response = await axiosClient.get("/Alertas/asesor", {
    params: { soloNoLeidas: true },
  });
  return response.data;
};

export const marcarNotificacionLeidaApi = async (idAlerta) => {
  const response = await axiosClient.patch(
    `/Alertas/${idAlerta}/marcar-leida`
  );
  return response.data;
};