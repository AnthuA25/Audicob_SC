import axiosClient from "./axiosClient";

export const loginApi = async ({ dni, password }) => {
  const response = await axiosClient.post("/Auth/login", {
    dni,
    password,
  });

  return response.data;
};