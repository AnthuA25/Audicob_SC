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
        var resumen = await CalcularResumenAsync(null);
        return Ok(resumen);
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ListarAdmin([FromQuery] bool? soloNoLeidas)
    {
        await GenerarAlertasAutomaticasAsync(null);
        var alertas = await ListarAlertasAsync(null, soloNoLeidas);
        return Ok(alertas);
    }

    [HttpGet("asesor/resumen")]
    [Authorize(Roles = "Asesor")]
    public async Task<IActionResult> ResumenAsesor()
    {
        int idAsesor = ObtenerIdUsuarioToken();

        await GenerarAlertasAutomaticasAsync(idAsesor);

        var resumen = await CalcularResumenAsync(idAsesor);
        return Ok(resumen);
    }

    [HttpGet("asesor")]
    [Authorize(Roles = "Asesor")]
    public async Task<IActionResult> ListarAsesor([FromQuery] bool? soloNoLeidas)
    {
        int idAsesor = ObtenerIdUsuarioToken();

        await GenerarAlertasAutomaticasAsync(idAsesor);

        var alertas = await ListarAlertasAsync(idAsesor, soloNoLeidas);
        return Ok(alertas);
    }

    [HttpPatch("{idAlerta:int}/marcar-leida")]
    [Authorize(Roles = "Asesor,Administrador")]
    public async Task<IActionResult> MarcarLeida(int idAlerta)
    {
        var alerta = await _context.Alerta
            .Include(a => a.IdClienteNavigation)
            .FirstOrDefaultAsync(a =>
                a.IdAlerta == idAlerta &&
                a.Activo &&
                !a.Eliminado
            );

        if (alerta == null)
            return NotFound(new { message = "Alerta no encontrada." });

        if (EsAsesor())
        {
            int idAsesor = ObtenerIdUsuarioToken();

            if (alerta.IdClienteNavigation == null ||
                alerta.IdClienteNavigation.IdAsesor != idAsesor)
            {
                return Forbid();
            }
        }

        alerta.Leido = true;
        alerta.FechaModificacion = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
        alerta.UsuarioModificacion = $"{ObtenerRolToken()}_{ObtenerIdUsuarioToken()}";

        await _context.SaveChangesAsync();

        return Ok(new { message = "Alerta marcada como leída." });
    }


    private async Task GenerarAlertasAutomaticasAsync(int? idAsesor)
    {
        var query = _context.Deuda
            .Include(d => d.IdClienteNavigation)
            .Where(d =>
                d.Activo &&
                !d.Eliminado &&
                d.EstadoDeuda != "PAGADO" &&
                d.DiasAtraso > 0 &&
                d.IdClienteNavigation.Activo &&
                !d.IdClienteNavigation.Eliminado
            );

        if (idAsesor.HasValue)
        {
            query = query.Where(d =>
                d.IdClienteNavigation.IdAsesor == idAsesor.Value
            );
        }

        var deudas = await query.ToListAsync();

        foreach (var deuda in deudas)
        {
            bool yaExiste = await _context.Alerta.AnyAsync(a =>
                a.IdDeuda == deuda.IdDeuda &&
                a.Activo &&
                !a.Eliminado
            );

            if (yaExiste)
                continue;

            string prioridad = deuda.DiasAtraso switch
            {
                > 30 => "Alta",
                >= 16 => "Alta",
                >= 5 => "Media",
                _ => "Baja"
            };

            string tipoAlerta = deuda.DiasAtraso >= 16
                ? "Riesgo de morosidad"
                : "Recordatorio de pago";

            string nombreCliente =
                $"{deuda.IdClienteNavigation.Nombres} {deuda.IdClienteNavigation.Apellidos}";

            var alerta = new Alertum
            {
                IdCliente = deuda.IdCliente,
                IdDeuda = deuda.IdDeuda,
                TipoAlerta = tipoAlerta,
                Mensaje = $"{nombreCliente} tiene {deuda.DiasAtraso} días de atraso y S/ {deuda.SaldoPendiente} de deuda",
                Prioridad = prioridad,
                Leido = false,
                FechaAlerta = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified),
                FechaRegistro = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified),
                UsuarioRegistro = "SISTEMA",
                Activo = true,
                Eliminado = false
            };

            await _context.Alerta.AddAsync(alerta);
        }

        await _context.SaveChangesAsync();
    }


    private async Task<ResumenAlertasDto> CalcularResumenAsync(int? idAsesor)
    {
        var query = _context.Deuda
            .AsNoTracking()
            .Where(d => d.Activo && !d.Eliminado
                     && d.EstadoDeuda != "PAGADO"
                     && d.IdClienteNavigation.Activo
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
            .AsNoTracking()
            .Where(a =>  a.Activo && !a.Eliminado);


        if (idAsesor.HasValue)
        {
            query = query.Where(a =>
                a.IdClienteNavigation != null &&
                a.IdClienteNavigation.IdAsesor == idAsesor.Value
            );
        }

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
                    : "",
                IdDeuda = a.IdDeuda,
                TipoAlerta = a.TipoAlerta,
                Mensaje = a.Mensaje,
                Prioridad = a.Prioridad,
                Leido = a.Leido,
                FechaAlerta = a.FechaAlerta
            })
            .ToListAsync();
    }
    private int ObtenerIdUsuarioToken()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? User.FindFirst("idUsuario")?.Value
                 ?? User.FindFirst("IdUsuario")?.Value;

        if (string.IsNullOrEmpty(id))
            throw new Exception("El token no contiene el id del usuario.");

        return int.Parse(id);
    }

    private string ObtenerRolToken()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value
               ?? User.FindFirst("rol")?.Value
               ?? "USUARIO";
    }

    private bool EsAsesor()
    {
        var rol = ObtenerRolToken();

        return rol.Equals("Asesor", StringComparison.OrdinalIgnoreCase);
    }
}
