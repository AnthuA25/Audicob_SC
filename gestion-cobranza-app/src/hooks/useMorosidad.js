import { useState, useEffect } from "react";
import {
  fetchMorosidad,
  fetchMorosidadMetricas,
} from "../services/morosidadService";

const morosidadEjemplo = [
  {
    id: 1,
    nombre: "Laura Martinez",
    email: "maria.lopez@audicob.com",
    telefono: "+51 555 123 4567",
    asesorAsignado: "Carlos Rodríguez",
    diasAtraso: 90,
    deudaPendiente: "S./ 2,000",
    riesgo: "Alto",
    estado: "Moroso",
  },
  {
    id: 2,
    nombre: "Laura Martinez",
    email: "maria.lopez@audicob.com",
    telefono: "+51 555 123 4567",
    asesorAsignado: "María López",
    diasAtraso: 60,
    deudaPendiente: "S./ 5,500",
    riesgo: "Alto",
    estado: "Contactado",
  },
  {
    id: 3,
    nombre: "Josmer Jauregui",
    email: "maria.lopez@audicob.com",
    telefono: "+51 555 123 4567",
    asesorAsignado: "María López",
    diasAtraso: 45,
    deudaPendiente: "S./ 3,000",
    riesgo: "Alto",
    estado: "Contactado",
  },
  {
    id: 4,
    nombre: "Janett Mendez",
    email: "maria.lopez@audicob.com",
    telefono: "+51 555 123 4567",
    asesorAsignado: "Carlos Rodríguez",
    diasAtraso: 35,
    deudaPendiente: "S./ 1,500",
    riesgo: "Medio",
    estado: "Contactado",
  },
  {
    id: 5,
    nombre: "Karla Santos",
    email: "maria.lopez@audicob.com",
    telefono: "+51 555 123 4567",
    asesorAsignado: "Carlos Rodríguez",
    diasAtraso: 25,
    deudaPendiente: "S./ 3,500",
    riesgo: "Medio",
    estado: "Negociación",
  },
];

const metricasEjemplo = {
  clientesMorosos: 7,
  deudaMorosaTotal: "S./ 7,000",
  morosidadCritica: 1,
  promedioAtraso: "41 días",
};

const useMorosidad = () => {
  const [morosidad, setMorosidad] = useState([]);
  const [metricas, setMetricas] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const cargar = async () => {
      try {
        const [m, met] = await Promise.all([
          fetchMorosidad(),
          fetchMorosidadMetricas(),
        ]);
        setMorosidad(m);
        setMetricas(met);
      } catch {
        setMorosidad(morosidadEjemplo);
        setMetricas(metricasEjemplo);
      } finally {
        setLoading(false);
      }
    };
    cargar();
  }, []);

  return { morosidad, metricas, loading };
};

export default useMorosidad;
