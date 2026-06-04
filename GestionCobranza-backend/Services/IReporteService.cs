using System.Threading.Tasks;
using GestionCobranza_backend.Dtos.Reporte;

namespace GestionCobranza_backend.Services
{
    public interface IReporteService
    {
        Task<ReporteGerencialDto> GetDashboardGerencialAsync();
        Task<string> GenerarYRegistrarReporteAsync(GenerarReporteRequestDto request, int idUsuario);
        Task<ReporteRendimientoIndividualDto> GetDashboardIndividualAsync(int idAsesor);
        Task<string> GenerarYRegistrarReporteAsesorAsync(GenerarReporteRequestDto request, int idAsesor);
    }
}