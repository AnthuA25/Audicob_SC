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
}
