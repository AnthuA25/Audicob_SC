using GestionCobranza.Application.DTOs;

namespace GestionCobranza.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardAsesorDto> ObtenerDashboardAsync(int idAsesor);
}
