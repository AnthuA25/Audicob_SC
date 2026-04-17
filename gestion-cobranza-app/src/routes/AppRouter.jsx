import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import AuthLayout from "../layouts/AuthLayout";
import DashboardLayout from "../layouts/DashboardLayout";
import LoginPage from "../pages/auth/LoginPage";
import DashboardAdminPage from "../pages/dashboard/DashboardAdminPage";
import ClientesPage from "../pages/clientes/ClientesPage";
import AsesoresPage from "../pages/asesores/AsesoresPage";
import DashboardAsesorPage from "../pages/dashboard/DashboardAsesorPage";
import MisClientesPage from "../pages/clientes/MisClientesPage";
import MiClienteDetallePage from "../pages/clientes/MiClienteDetallePage";
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

const AppRouter = () => {
  return (
    <BrowserRouter>
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
            path={ROUTES.MIS_CLIENTES_DETALLE}
            element={
              <ProtectedRoute allowedRoles={["Asesor"]}>
                <MiClienteDetallePage />
              </ProtectedRoute>
            }
          />
        </Route>
      </Routes>
    </BrowserRouter>
  );
};

export default AppRouter;
