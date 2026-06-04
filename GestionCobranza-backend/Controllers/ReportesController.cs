using GestionCobranza_backend.Dtos.Reporte;
using GestionCobranza_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestionCobranza_backend.Controllers
{
    [ApiController]
    [Route("api/reportes")]
    [Authorize]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReportesController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        [HttpGet("admin")]
        public async Task<ActionResult<ReporteGerencialDto>> ObtenerVistaAdministrador()
        {
            if (!EsAdministrador())
                return Forbid();

            var resultado = await _reporteService.GetDashboardGerencialAsync();
            return Ok(resultado);
        }

        [HttpGet("asesor")]
        public async Task<ActionResult<ReporteRendimientoIndividualDto>> ObtenerVistaAsesor()
        {
            int idAsesor = ObtenerIdUsuario();

            var resultado = await _reporteService.GetDashboardIndividualAsync(idAsesor);
            return Ok(resultado);
        }

        [HttpPost("admin/generar")]
        public async Task<ActionResult<ReporteGeneradoResponseDto>> GenerarReporteAdministrador(
            [FromBody] GenerarReporteRequestDto request)
        {
            if (!EsAdministrador())
                return Forbid();

            int idUsuario = ObtenerIdUsuario();
            string baseUrl = $"{Request.Scheme}://{Request.Host}";

            var resultado = await _reporteService.GenerarReporteAdminAsync(request, idUsuario, baseUrl);
            return Ok(resultado);
        }

        [HttpPost("asesor/generar")]
        public async Task<ActionResult<ReporteGeneradoResponseDto>> GenerarReporteAsesor(
            [FromBody] GenerarReporteRequestDto request)
        {
            int idAsesor = ObtenerIdUsuario();
            string baseUrl = $"{Request.Scheme}://{Request.Host}";

            var resultado = await _reporteService.GenerarReporteAsesorAsync(request, idAsesor, baseUrl);
            return Ok(resultado);
        }

        private int ObtenerIdUsuario()
        {
            var id = User.FindFirst("idUsuario")?.Value
                     ?? User.FindFirst("IdUsuario")?.Value
                     ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(id))
                throw new Exception("El token no contiene el id del usuario.");

            return int.Parse(id);
        }

        private bool EsAdministrador()
        {
            var rol = User.FindFirst(ClaimTypes.Role)?.Value
                      ?? User.FindFirst("rol")?.Value
                      ?? User.FindFirst("role")?.Value
                      ?? "";

            return rol.ToUpper() == "ADMIN"
                || rol.ToUpper() == "ADMINISTRADOR"
                || rol == "1";
        }
    }
}