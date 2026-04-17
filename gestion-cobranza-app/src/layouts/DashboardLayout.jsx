import { Outlet } from "react-router-dom";
import Navbar from "../components/common/Navbar";
import Sidebar from "../components/common/Sidebar";
import SidebarAsesor from "../components/common/SidebarAsesor";
import useAuth from "../hooks/useAuth";
import "../styles/dashboard.css";

const DashboardLayout = () => {
  const { user } = useAuth();

  const esAdministrador = user?.rol === "Administrador";

  return (
    <div className="dashboard-layout">
      <Navbar />
      <div className="dashboard-body">
        {esAdministrador ? <Sidebar /> : <SidebarAsesor />}
        <main className="dashboard-main">
          <Outlet />
        </main>
      </div>
    </div>
  );
};

export default DashboardLayout;