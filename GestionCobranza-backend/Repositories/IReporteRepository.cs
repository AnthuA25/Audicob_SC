using GestionCobranza_backend.Models;
using GestionCobranza_backend.Dtos.Reporte;

namespace GestionCobranza_backend.Repositories
{
    public interface IReporteRepository
    {
        Task<List<RendimientoAsesorDto>> GetRendimientoAsesoresAsync();
        Task<List<ResumenClienteDto>> GetResumenClientesAsync();
        Task<List<ReporteRecienteDto>> GetReportesRecientesAsync();
        Task<List<ReporteRecienteDto>> GetReportesRecientesPorUsuarioAsync(int idUsuario);
        Task<ResumenAsesorDto> GetRendimientoIndividualAsync(int idAsesor);
        Task<List<ResumenClienteDto>> GetResumenClientesPorAsesorAsync(int idAsesor);
        Task<Usuario?> GetUsuarioPorIdAsync(int idUsuario);
        Task<int> RegistrarReporteGeneradoAsync(ReporteGenerado reporte);
    }
}