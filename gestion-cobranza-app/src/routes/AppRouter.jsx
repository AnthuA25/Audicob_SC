import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import AuthLayout from "../layouts/AuthLayout";
import DashboardLayout from "../layouts/DashboardLayout";
import LoginPage from "../pages/auth/LoginPage";
import CreateAsesorPage from "../pages/admin/CreateAsesorPage";
import useAuth from "../hooks/useAuth";
import { ROUTES } from "../constants/routes";

const ProtectedRoute = ({ children }) => {
  const { token, loading } = useAuth();

  if (loading) return <div>Cargando...</div>;
  if (!token) return <Navigate to={ROUTES.LOGIN} />;

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
            <ProtectedRoute>
              <DashboardLayout />
            </ProtectedRoute>
          }
        >
          <Route
            path={ROUTES.DASHBOARD_ADMIN}
            element={<div>Dashboard Admin</div>}
          />
          <Route
            path={ROUTES.DASHBOARD_ASESOR}
            element={<div>Dashboard Asesor</div>}
          />
          <Route
            path={ROUTES.ASESORES}
            element={<div>Lista de Asesores</div>}
          />
          <Route
            path="/asesores/crear"
            element={<CreateAsesorPage />}
          />
        </Route>
      </Routes>
    </BrowserRouter>
  );
};

export default AppRouter;
