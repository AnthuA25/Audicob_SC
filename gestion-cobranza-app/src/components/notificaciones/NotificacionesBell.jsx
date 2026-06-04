import { useEffect, useRef, useState } from "react";
import { Bell, AlertTriangle } from "lucide-react";
import {
  getNotificacionesAdminApi,
  getNotificacionesAsesorApi,
  marcarNotificacionLeidaApi,
} from "../../api/notificacionesApi";
import "../../styles/notificaciones.css";

const NotificacionesBell = ({ rol }) => {
  const [abierto, setAbierto] = useState(false);
  const [notificaciones, setNotificaciones] = useState([]);
  const ref = useRef(null);

  const esAdmin = rol?.toLowerCase() === "administrador";

  const cargarNotificaciones = async () => {
    const data = esAdmin
      ? await getNotificacionesAdminApi()
      : await getNotificacionesAsesorApi();

    setNotificaciones(data || []);
  };

  const marcarLeida = async (idAlerta) => {
    await marcarNotificacionLeidaApi(idAlerta);
    await cargarNotificaciones();
  };

  useEffect(() => {
    cargarNotificaciones();
  }, [rol]);

  useEffect(() => {
    const cerrar = (e) => {
      if (ref.current && !ref.current.contains(e.target)) {
        setAbierto(false);
      }
    };

    document.addEventListener("mousedown", cerrar);
    return () => document.removeEventListener("mousedown", cerrar);
  }, []);

  return (
    <div className="noti-wrapper" ref={ref}>
      <button
        type="button"
        className="noti-bell-btn"
        onClick={() => setAbierto(!abierto)}
      >
        <Bell size={20} color="#374151" />
        {notificaciones.length > 0 && <span className="notif-dot" />}
      </button>

      {abierto && (
        <div className="noti-dropdown">
          <div className="noti-header">
            <strong>Notificaciones</strong>
            <span>{notificaciones.length} pendientes</span>
          </div>

          {notificaciones.length === 0 ? (
            <div className="noti-empty">No tienes alertas pendientes</div>
          ) : (
            <div className="noti-list">
              {notificaciones.map((n) => (
                <div className="noti-item" key={n.idAlerta}>
                  <div className="noti-icon">
                    <AlertTriangle size={16} />
                  </div>

                  <div className="noti-content">
                    <strong>{n.tipoAlerta}</strong>
                    <p>
                      {n.nombreCliente} - {n.mensaje}
                    </p>
                    <span>Prioridad {n.prioridad}</span>
                  </div>

                  <button
                    type="button"
                    className="noti-action"
                    onClick={() => marcarLeida(n.idAlerta)}
                  >
                    Leer
                  </button>
                </div>
              ))}
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default NotificacionesBell;