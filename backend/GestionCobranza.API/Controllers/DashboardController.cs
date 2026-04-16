using GestionCobranza.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestionCobranza.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;
    public DashboardController(IDashboardService service) => _service = service;

    /// <summary>
    /// HU-10: Retorna todas las métricas del panel de control del asesor.
    /// Incluye tarjetas resumen, gráfico de distribución, tendencia de morosidad
    /// y clasificación de deudores.
    /// </summary>
    [HttpGet("asesor/{idAsesor:int}")]
    public async Task<IActionResult> GetDashboard(int idAsesor)
    {
        if (idAsesor <= 0)
            return BadRequest(new { mensaje = "El ID del asesor no es válido." });

        var resultado = await _service.ObtenerDashboardAsync(idAsesor);
        return Ok(resultado);
    }
}
