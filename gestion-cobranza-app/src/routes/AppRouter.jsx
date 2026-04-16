import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import AuthLayout from "../layouts/AuthLayout";
import DashboardLayout from "../layouts/DashboardLayout";
import LoginPage from "../pages/auth/LoginPage";
import DashboardAdminPage from "../pages/dashboard/DashboardAdminPage";
import ClientesPage from "../pages/clientes/ClientesPage";
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
            element={<div>Dashboard Asesor</div>}
          />
          <Route path={ROUTES.CLIENTES} element={<ClientesPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
};

export default AppRouter;