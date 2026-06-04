using System.Threading.Tasks;
using GestionCobranza_backend.Dtos.Reporte;

namespace GestionCobranza_backend.Services
{
    public interface IReporteService
    {
        Task<ReporteGerencialDto> GetDashboardGerencialAsync();
        Task<ReporteRendimientoIndividualDto> GetDashboardIndividualAsync(int idAsesor);

        Task<ReporteGeneradoResponseDto> GenerarReporteAdminAsync(GenerarReporteRequestDto request, int idUsuario, string baseUrl);
        Task<ReporteGeneradoResponseDto> GenerarReporteAsesorAsync(GenerarReporteRequestDto request, int idAsesor, string baseUrl);
    }
}