using GestionCobranza_backend.Dtos.Pagos;
using GestionCobranza_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionCobranza_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PagosController : ControllerBase
{
    private readonly AppDbContext _context;

    public PagosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("asesor/{idAsesor}")]
    [Authorize(Roles = "Asesor,Administrador")]
    public async Task<IActionResult> ListarPagosPorAsesor(int idAsesor)
    {
        var pagos = await _context.Pagos
            .Where(p => !p.Eliminado
                     && p.EstadoPago == "CONFIRMADO"
                     && !p.IdDeudaNavigation.Eliminado
                     && !p.IdDeudaNavigation.IdClienteNavigation.Eliminado
                     && p.IdDeudaNavigation.IdClienteNavigation.IdAsesor == idAsesor)
            .OrderByDescending(p => p.FechaPago)
            .Select(p => new PagoResponseDto
            {
                IdPago = p.IdPago,
                IdDeuda = p.IdDeuda,
                IdCliente = p.IdDeudaNavigation.IdCliente,
                NombreCliente = p.IdDeudaNavigation.IdClienteNavigation.Nombres + " "
                              + p.IdDeudaNavigation.IdClienteNavigation.Apellidos,
                Monto = p.Monto,
                FechaPago = p.FechaPago,
                MetodoPago = p.MetodoPago,
                Nota = p.Nota,
                EstadoPago = p.EstadoPago
            })
            .ToListAsync();

        return Ok(pagos);
    }

    [HttpGet("asesor/{idAsesor}/resumen")]
    [Authorize(Roles = "Asesor,Administrador")]
    public async Task<IActionResult> ObtenerResumenPagos(int idAsesor)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Now);
        var diasDesdeMonday = ((int)DateTime.Now.DayOfWeek - 1 + 7) % 7;
        var inicioSemana = hoy.AddDays(-diasDesdeMonday);
        var finSemana = inicioSemana.AddDays(6);
        var inicioMes = new DateOnly(hoy.Year, hoy.Month, 1);
        var finMes = inicioMes.AddMonths(1).AddDays(-1);

        var pagosConfirmados = await _context.Pagos
            .Where(p => !p.Eliminado
                     && p.EstadoPago == "CONFIRMADO"
                     && !p.IdDeudaNavigation.Eliminado
                     && !p.IdDeudaNavigation.IdClienteNavigation.Eliminado
                     && p.IdDeudaNavigation.IdClienteNavigation.IdAsesor == idAsesor)
            .Select(p => new { p.Monto, p.FechaPago })
            .ToListAsync();

        var resumen = new ResumenPagosDto
        {
            TotalPagosHoy = pagosConfirmados
                .Where(p => p.FechaPago == hoy)
                .Sum(p => p.Monto),
            TransaccionesHoy = pagosConfirmados
                .Count(p => p.FechaPago == hoy),
            TotalPagosSemana = pagosConfirmados
                .Where(p => p.FechaPago >= inicioSemana && p.FechaPago <= finSemana)
                .Sum(p => p.Monto),
            TransaccionesSemana = pagosConfirmados
                .Count(p => p.FechaPago >= inicioSemana && p.FechaPago <= finSemana),
            TotalPagosMes = pagosConfirmados
                .Where(p => p.FechaPago >= inicioMes && p.FechaPago <= finMes)
                .Sum(p => p.Monto),
            TransaccionesMes = pagosConfirmados
                .Count(p => p.FechaPago >= inicioMes && p.FechaPago <= finMes)
        };

        return Ok(resumen);
    }

    [HttpPost("registrar")]
    [Authorize(Roles = "Asesor")]
    public async Task<IActionResult> RegistrarPago([FromBody] RegistrarPagoDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var idUsuarioClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(idUsuarioClaim) || !int.TryParse(idUsuarioClaim, out var idAsesor))
            return Unauthorized(new { mensaje = "No se pudo identificar al asesor autenticado." });

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var deudum = await _context.Deuda
                .Include(d => d.IdClienteNavigation)
                .FirstOrDefaultAsync(d =>
                    d.IdDeuda == dto.IdDeuda &&
                    !d.Eliminado &&
                    d.Activo);

            if (deudum == null)
                return NotFound(new { mensaje = "La deuda especificada no existe o fue eliminada." });

            if (deudum.IdClienteNavigation.IdAsesor != idAsesor)
                return Forbid();

            if (dto.MontoPagado <= 0)
                return BadRequest(new { mensaje = "El monto pagado debe ser mayor a 0." });

            if (dto.MontoPagado > deudum.SaldoPendiente)
                return BadRequest(new { mensaje = $"El monto no puede superar el saldo pendiente actual (S/. {deudum.SaldoPendiente})." });

            var fechaActualParaBd = DateTime.Now;

            var nuevoPago = new Pago
            {
                IdDeuda = deudum.IdDeuda,
                Monto = dto.MontoPagado,
                FechaPago = DateOnly.FromDateTime(DateTime.Today),
                MetodoPago = dto.MetodoPago.Trim().ToUpper(),
                ComprobanteUrl = dto.NroOperacion,
                Nota = dto.Observacion,
                EstadoPago = "CONFIRMADO",
                FechaRegistro = fechaActualParaBd,
                UsuarioRegistro = $"ASESOR_{idAsesor}",
                Activo = true,
                Eliminado = false
            };

            _context.Pagos.Add(nuevoPago);

            deudum.MontoPagado += dto.MontoPagado;
            deudum.SaldoPendiente -= dto.MontoPagado;
            deudum.FechaModificacion = fechaActualParaBd;
            deudum.UsuarioModificacion = $"ASESOR_{idAsesor}";

            if (deudum.SaldoPendiente == 0)
            {
                deudum.EstadoDeuda = "PAGADO";
                deudum.DiasAtraso = 0;
            }

            var cliente = deudum.IdClienteNavigation;

            var tieneOtrasDeudasMorosas = await _context.Deuda
                .AnyAsync(d =>
                    d.IdCliente == cliente.IdCliente &&
                    d.IdDeuda != deudum.IdDeuda &&
                    d.SaldoPendiente > 0 &&
                    d.DiasAtraso > 0 &&
                    d.Activo &&
                    !d.Eliminado);

            if (!tieneOtrasDeudasMorosas && deudum.SaldoPendiente == 0)
            {
                cliente.EstadoCliente = "AL DIA";
                cliente.Riesgo = "BAJO";
                cliente.FechaModificacion = fechaActualParaBd;
                cliente.UsuarioModificacion = $"ASESOR_{idAsesor}";
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return StatusCode(201, new
            {
                mensaje = "Pago registrado exitosamente.",
                idPago = nuevoPago.IdPago,
                idDeuda = nuevoPago.IdDeuda,
                monto = nuevoPago.Monto,
                saldoPendiente = deudum.SaldoPendiente
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            return StatusCode(500, new
            {
                mensaje = "Error interno al procesar el pago.",
                detalle = ex.Message,
                interno = ex.InnerException?.Message
            });
        }
    }
}
