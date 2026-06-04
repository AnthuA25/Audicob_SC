using System.Security.Claims;
using GestionCobranza_backend.Dtos.Alertas;
using GestionCobranza_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionCobranza_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlertasController : ControllerBase
{
    private readonly AppDbContext _context;

    public AlertasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("admin/resumen")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ResumenAdmin()
    {
        var resumen = await CalcularResumenAsync(idAsesor: null);
        return Ok(resumen);
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ListarAdmin([FromQuery] bool? soloNoLeidas)
    {
        var alertas = await ListarAlertasAsync(idAsesor: null, soloNoLeidas);
        return Ok(alertas);
    }

    [HttpGet("asesor/{idAsesor:int}/resumen")]
    [Authorize(Roles = "Asesor,Administrador")]
    public async Task<IActionResult> ResumenAsesor(int idAsesor)
    {
        var resumen = await CalcularResumenAsync(idAsesor);
        return Ok(resumen);
    }

    [HttpGet("asesor/{idAsesor:int}")]
    [Authorize(Roles = "Asesor,Administrador")]
    public async Task<IActionResult> ListarAsesor(int idAsesor, [FromQuery] bool? soloNoLeidas)
    {
        var alertas = await ListarAlertasAsync(idAsesor, soloNoLeidas);
        return Ok(alertas);
    }

    [HttpPatch("{idAlerta:int}/marcar-leida")]
    [Authorize(Roles = "Asesor,Administrador")]
    public async Task<IActionResult> MarcarLeida(int idAlerta)
    {
        var alerta = await _context.Alerta
            .FirstOrDefaultAsync(a => a.IdAlerta == idAlerta && !a.Eliminado);

        if (alerta == null)
            return NotFound(new { message = "Alerta no encontrada." });

        var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "USUARIO";
        var idUsuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";

        alerta.Leido = true;
        alerta.FechaModificacion = DateTime.Now;
        alerta.UsuarioModificacion = $"{rol.ToUpper()}_{idUsuario}";

        await _context.SaveChangesAsync();

        return Ok(new { message = "Alerta marcada como leída." });
    }

    private async Task<ResumenAlertasDto> CalcularResumenAsync(int? idAsesor)
    {
        var query = _context.Deuda
            .Where(d => !d.Eliminado
                     && d.EstadoDeuda != "PAGADO"
                     && !d.IdClienteNavigation.Eliminado);

        if (idAsesor.HasValue)
            query = query.Where(d => d.IdClienteNavigation.IdAsesor == idAsesor.Value);

        var diasAtraso = await query.Select(d => d.DiasAtraso).ToListAsync();

        return new ResumenAlertasDto
        {
            RiesgoMedio = diasAtraso.Count(d => d >= 5 && d <= 15),
            RiesgoAlto = diasAtraso.Count(d => d >= 16 && d <= 30),
            Critico = diasAtraso.Count(d => d > 30)
        };
    }

    private async Task<List<AlertaResponseDto>> ListarAlertasAsync(int? idAsesor, bool? soloNoLeidas)
    {
        var query = _context.Alerta
            .Where(a => !a.Eliminado);

        if (idAsesor.HasValue)
            query = query.Where(a => a.IdClienteNavigation!.IdAsesor == idAsesor.Value);

        if (soloNoLeidas == true)
            query = query.Where(a => !a.Leido);

        return await query
            .OrderByDescending(a => a.FechaAlerta)
            .Select(a => new AlertaResponseDto
            {
                IdAlerta = a.IdAlerta,
                IdCliente = a.IdCliente ?? 0,
                NombreCliente = a.IdClienteNavigation != null
                    ? a.IdClienteNavigation.Nombres + " " + a.IdClienteNavigation.Apellidos
                    : string.Empty,
                IdDeuda = a.IdDeuda,
                TipoAlerta = a.TipoAlerta,
                Mensaje = a.Mensaje,
                Prioridad = a.Prioridad,
                Leido = a.Leido,
                FechaAlerta = a.FechaAlerta
            })
            .ToListAsync();
    }
}
