using System.Collections.Generic;
using System.Threading.Tasks;
using GestionCobranza_backend.Models;
using GestionCobranza_backend.Dtos.Reporte;

namespace GestionCobranza_backend.Repositories
{
    public interface IReporteRepository
    {
        Task<List<RendimientoAsesorDto>> GetRendimientoAsesoresAsync();
        Task<List<ResumenClienteDto>> GetResumenClientesAsync();
        Task<List<ReporteRecienteDto>> GetReportesRecientesAsync();
        Task<Usuario?> GetAdministradorDisponibleAsync();
        Task<ResumenAsesorDto> GetRendimientoIndividualAsync(int idAsesor);
        Task<List<ResumenClienteDto>> GetResumenClientesPorAsesorAsync(int idAsesor);
        Task<bool> RegistrarReporteGeneradoAsync(ReporteGenerado reporte);
        Task<Usuario?> GetAsesorPorIdAsync(int idAsesor);
    }
}