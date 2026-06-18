import axiosClient from "./axiosClient";

export const getDashboardMorosidadApi = async () => {
  const response = await axiosClient.get("/Morosidad/dashboard");
  return response.data;
};

export const descargarReporteMorosidadApi = async () => {
  const response = await axiosClient.get("/Morosidad/reporte", {
    responseType: "blob",
  });

  const url = window.URL.createObjectURL(new Blob([response.data]));
  const link = document.createElement("a");

  link.href = url;
  link.setAttribute("download", "reporte_seguimiento_morosidad.xlsx");
  document.body.appendChild(link);
  link.click();

  link.remove();
  window.URL.revokeObjectURL(url);
};