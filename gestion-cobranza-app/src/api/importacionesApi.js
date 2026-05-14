import axiosClient from "./axiosClient";

export const descargarPlantillaApi = async () => {
  const response = await axiosClient.get("/Importaciones/plantilla", {
    responseType: "blob",
  });

  descargarBlob(response.data, "plantilla_clientes.xlsx");
};

export const subirArchivoImportacionApi = async (archivo) => {
  const formData = new FormData();
  formData.append("archivo", archivo);

  const response = await axiosClient.post("/Importaciones/subir", formData, {
    headers: { "Content-Type": "multipart/form-data" },
  });

  return response.data;
};

export const getImportacionesRecientesApi = async () => {
  const response = await axiosClient.get("/Importaciones/recientes");
  return response.data;
};

export const descargarImportacionApi = async (idImportacion, nombreArchivo) => {
  try {
    const response = await axiosClient.get(
      `/Importaciones/${idImportacion}/descargar`,
      { responseType: "blob" },
    );

    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement("a");

    link.href = url;
    link.setAttribute("download", nombreArchivo || "importacion.xlsx");
    document.body.appendChild(link);
    link.click();
    link.remove();

    window.URL.revokeObjectURL(url);
  } catch (error) {
    if (error.response?.data instanceof Blob) {
      const text = await error.response.data.text();
      const json = JSON.parse(text);
      throw new Error(json.message);
    }

    throw error;
  }
};

const descargarBlob = (data, nombreArchivo) => {
  const url = window.URL.createObjectURL(
    new Blob([data], {
      type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    }),
  );

  const link = document.createElement("a");
  link.href = url;
  link.setAttribute("download", nombreArchivo);
  document.body.appendChild(link);
  link.click();
  link.remove();
  window.URL.revokeObjectURL(url);
};
