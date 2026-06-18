import { HashRouter, Routes, Route, Navigate } from "react-router-dom";
import AuthLayout from "../layouts/AuthLayout";
import DashboardLayout from "../layouts/DashboardLayout";
import LoginPage from "../pages/auth/LoginPage";
import DashboardAdminPage from "../pages/dashboard/DashboardAdminPage";
import ClientesPage from "../pages/clientes/ClientesPage";
import AsesoresPage from "../pages/asesores/AsesoresPage";
import DashboardAsesorPage from "../pages/dashboard/DashboardAsesorPage";
import MisClientesPage from "../pages/clientes/MisClientesPage";
import MiClienteDetallePage from "../pages/clientes/MiClienteDetallePage";
import MorosidadPage from "../pages/morosidad/MorosidadPage";
import ImportarPage from "../pages/importar/ImportarPage";
import PagosPage from "../pages/pagos/PagosPage";
import AlertasPage from "../pages/alertas/AlertasPage";
import ReportesPage from "../pages/reportes/ReportesPage";
import AlertasAsesorPage from "../pages/alertas/AlertasAsesorPage";
import ReportesAsesorPage from "../pages/reportes/ReportesAsesorPage";
import useAuth from "../hooks/useAuth";
import { ROUTES } from "../constants/routes";

const ProtectedRoute = ({ children, allowedRoles = [] }) => {
  const { token, user, loading } = useAuth();

  if (loading) return <div>Cargando...</div>;
  if (!token) return <Navigate to={ROUTES.LOGIN} />;

  if (allowedRoles.length > 0 && !allowedRoles.includes(user?.rol)) {
    return <Navigate to={ROUTES.LOGIN} />;
  }

  return children;
};
const AlertasRouter = () => {
  const { user } = useAuth();
  return user?.rol === "Administrador" ? (
    <AlertasPage />
  ) : (
    <AlertasAsesorPage />
  );
};

const ReportesRouter = () => {
  const { user } = useAuth();
  return user?.rol === "Administrador" ? (
    <ReportesPage />
  ) : (
    <ReportesAsesorPage />
  );
};
const AppRouter = () => {
  return (
    <HashRouter>
      <Routes>
        <Route element={<AuthLayout />}>
          <Route path={ROUTES.LOGIN} element={<LoginPage />} />
          <Route path="/" element={<Navigate to={ROUTES.LOGIN} />} />
        </Route>

        <Route
          element={
            <ProtectedRoute allowedRoles={["Administrador", "Asesor"]}>
              <DashboardLayout />
            </ProtectedRoute>
          }
        >
          <Route
            path={ROUTES.DASHBOARD_ADMIN}
            element={
              <ProtectedRoute allowedRoles={["Administrador"]}>
                <DashboardAdminPage />
              </ProtectedRoute>
            }
          />
          <Route
            path={ROUTES.DASHBOARD_ASESOR}
            element={
              <ProtectedRoute allowedRoles={["Asesor"]}>
                <DashboardAsesorPage />
              </ProtectedRoute>
            }
          />
          <Route
            path={ROUTES.CLIENTES}
            element={
              <ProtectedRoute allowedRoles={["Administrador"]}>
                <ClientesPage />
              </ProtectedRoute>
            }
          />
          <Route
            path={ROUTES.ASESORES}
            element={
              <ProtectedRoute allowedRoles={["Administrador"]}>
                <AsesoresPage />
              </ProtectedRoute>
            }
          />

          <Route
            path={ROUTES.MIS_CLIENTES}
            element={
              <ProtectedRoute allowedRoles={["Asesor"]}>
                <MisClientesPage />
              </ProtectedRoute>
            }
          />
          <Route
            path={ROUTES.MOROSIDAD}
            element={
              <ProtectedRoute allowedRoles={["Administrador"]}>
                <MorosidadPage />
              </ProtectedRoute>
            }
          />
          <Route
            path={ROUTES.MIS_CLIENTES_DETALLE}
            element={
              <ProtectedRoute allowedRoles={["Administrador","Asesor"]}>
                <MiClienteDetallePage />
              </ProtectedRoute>
            }
          />
          <Route
            path={ROUTES.IMPORTAR}
            element={
              <ProtectedRoute allowedRoles={["Administrador"]}>
                <ImportarPage />
              </ProtectedRoute>
            }
          />

          <Route
            path={ROUTES.PAGOS}
            element={
              <ProtectedRoute allowedRoles={["Asesor"]}>
                <PagosPage />
              </ProtectedRoute>
            }
          />
          <Route
            path={ROUTES.ALERTAS}
            element={
              <ProtectedRoute allowedRoles={["Administrador", "Asesor"]}>
                <AlertasRouter />
              </ProtectedRoute>
            }
          />
          <Route
            path={ROUTES.REPORTES}
            element={
              <ProtectedRoute allowedRoles={["Administrador", "Asesor"]}>
                <ReportesRouter />
              </ProtectedRoute>
            }
          />
        </Route>
      </Routes>
    </HashRouter>
  );
};

export default AppRouter;
