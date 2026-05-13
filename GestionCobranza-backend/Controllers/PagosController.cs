using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionCobranza_backend.Dtos.Pago;
using GestionCobranza_backend.Models;

namespace GestionCobranza_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PagosController : ControllerBase
{
    private readonly AppDbContext _context;

    public PagosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> RegistrarPago([FromBody] RegistrarPagoDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Validar la existencia de la deuda activa
            var deudum = await _context.Deuda
                .Include(d => d.IdClienteNavigation)
                .FirstOrDefaultAsync(d => d.IdDeuda == dto.IdDeuda && !d.Eliminado);

            if (deudum == null)
            {
                return NotFound(new { mensaje = "La deuda especificada no existe o fue eliminada." });
            }

            if (dto.MontoPagado > deudum.SaldoPendiente)
            {
                return BadRequest(new { mensaje = $"El monto no puede superar el saldo pendiente actual (S/. {deudum.SaldoPendiente})" });
            }

            // Usamos DateTime.Now para que sea compatible con 'timestamp without time zone'
            DateTime fechaActualParaBd = DateTime.Now;

            // 2. Crear la entidad de Pago usando campos 100% reales de tu DB
            var nuevoPago = new Models.Pago
            {
                IdDeuda = deudum.IdDeuda,
                Monto = dto.MontoPagado,
                FechaPago = DateOnly.FromDateTime(DateTime.Today),
                MetodoPago = dto.MetodoPago.ToUpper(),
                ComprobanteUrl = dto.NroOperacion,
                Nota = dto.Observacion,
                EstadoPago = "CONFIRMADO",
                FechaRegistro = fechaActualParaBd,
                UsuarioRegistro = "ASESOR_LOGUEADO",
                Activo = true,
                Eliminado = false
            };

            _context.Pagos.Add(nuevoPago);

            // 3. Actualizar la Deuda (Recálculo)
            deudum.MontoPagado += dto.MontoPagado;
            deudum.SaldoPendiente -= dto.MontoPagado;
            deudum.FechaModificacion = fechaActualParaBd;
            deudum.UsuarioModificacion = "ASESOR_LOGUEADO";

            if (deudum.SaldoPendiente == 0)
            {
                deudum.EstadoDeuda = "PAGADO";
                deudum.DiasAtraso = 0;
            }

            // 4. Verificar estado del Cliente asociado
            var cliente = deudum.IdClienteNavigation;
            if (cliente != null)
            {
                var tieneOtrasDeudasMorosas = await _context.Deuda
                    .AnyAsync(d => d.IdCliente == cliente.IdCliente && d.IdDeuda != deudum.IdDeuda && d.SaldoPendiente > 0 && d.DiasAtraso > 0 && !d.Eliminado);

                if (!tieneOtrasDeudasMorosas && deudum.SaldoPendiente == 0)
                {
                    cliente.EstadoCliente = "AL DIA";
                    cliente.Riesgo = "BAJO";
                    cliente.FechaModificacion = fechaActualParaBd;
                    cliente.UsuarioModificacion = "ASESOR_LOGUEADO";
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return StatusCode(201, new { mensaje = "Pago registrado exitosamente.", idPago = nuevoPago.IdPago });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { mensaje = "Error interno al procesar el pago.", detalle = ex.Message, interno = ex.InnerException?.Message });
        }
    }
}