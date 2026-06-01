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

        [HttpPost("descargar-excel")]
        public async Task<IActionResult> DescargarYRegistrarReporte([FromBody] GenerarReporteRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Los parámetros para el reporte no son válidos.");
            }

            try
            {
                // ====================================================================
                // CORRECCIÓN CLAVE: Cambiamos el ID simulado a 2.
                // Según tus datos reales de PostgreSQL, Jimena Rodríguez (Admin) tiene id_usuario = 2.
                // ====================================================================
                int idUsuarioSimulado = 2;

                string urlArchivo = await _reporteService.GenerarYRegistrarReporteAsync(request, idUsuarioSimulado);

                return Created("", new { mensaje = "Reporte registrado con éxito en el historial", url = urlArchivo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al procesar el registro de descarga: {ex.Message}");
            }
        }
    }
}