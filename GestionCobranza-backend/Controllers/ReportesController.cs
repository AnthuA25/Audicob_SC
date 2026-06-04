using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using GestionCobranza_backend.Dtos.Reporte;
using GestionCobranza_backend.Services;

namespace GestionCobranza_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReportesController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        // GET: api/reportes/gerencial
        [HttpGet("gerencial")]
        public async Task<ActionResult<ReporteGerencialDto>> ObtenerReporteGerencial()
        {
            try
            {
                var resultado = await _reporteService.GetDashboardGerencialAsync();
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al obtener el reporte gerencial: {ex.Message}");
            }
        }

        // POST: api/reportes/descargar-excel
        [HttpPost("descargar-excel")]
        public async Task<IActionResult> DescargarYRegistrarReporte([FromBody] GenerarReporteRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Los parámetros para el reporte no son válidos.");
            }

            try
            {
                int idUsuarioSimulado = 2;

                string urlArchivo = await _reporteService.GenerarYRegistrarReporteAsync(request, idUsuarioSimulado);

                return Created("", new { mensaje = "Reporte registrado con éxito en el historial", url = urlArchivo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al procesar el registro de descarga: {ex.Message}");
            }
        }

        // GET: api/reportes/asesor
        [HttpGet("asesor")]
        public async Task<ActionResult<ReporteRendimientoIndividualDto>> ObtenerReporteAsesor()
        {
            try
            {
                int idAsesorSimulado = 3;

                var resultado = await _reporteService.GetDashboardIndividualAsync(idAsesorSimulado);
                return Ok(resultado); // HTTP 200
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al obtener el rendimiento del asesor: {ex.Message}");
            }
        }

        // POST: api/reportes/asesor/descargar-excel
        [HttpPost("asesor/descargar-excel")]
        public async Task<IActionResult> DescargarYRegistrarReporteAsesor([FromBody] GenerarReporteRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Los parámetros para el reporte del asesor no son válidos.");
            }

            try
            {
                // ID simulado de Asesor (Marcelo Panduro = ID 3) según tus tablas de PostgreSQL
                int idAsesorSimulado = 3;

                string urlArchivo = await _reporteService.GenerarYRegistrarReporteAsesorAsync(request, idAsesorSimulado);

                return Created("", new { mensaje = "Reporte de rendimiento individual registrado con éxito", url = urlArchivo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al procesar la descarga del asesor: {ex.Message}");
            }
        }
    }
}