using System.Security.Claims;
using GestionCobranza_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionCobranza_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerDashboardAdmin()
    {
        var totalAsesores = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .CountAsync(u => u.Activo && !u.Eliminado && u.IdRolNavigation.Nombre == "Asesor");

        var totalClientes = await _context.Clientes
            .CountAsync(c => c.Activo && !c.Eliminado);

        var cobranzaTotal = await _context.Pagos
            .Where(p => p.Activo && !p.Eliminado && p.EstadoPago == "CONFIRMADO")
            .SumAsync(p => (decimal?)p.Monto) ?? 0;

        var totalDeudas = await _context.Deuda
            .Where(d => d.Activo && !d.Eliminado)
            .SumAsync(d => (decimal?)d.MontoTotal) ?? 0;

        var eficienciaGlobal = totalDeudas > 0
            ? Math.Round((cobranzaTotal / totalDeudas) * 100, 2)
            : 0;

        return Ok(new
        {
            totalAsesores,
            totalClientes,
            cobranzaTotal,
            eficienciaGlobal
        });
    }

    [HttpGet("asesor")]
    [Authorize(Roles = "Asesor")]
    public async Task<IActionResult> ObtenerDashboardAsesor()
    {
        var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(idUsuarioClaim))
            return Unauthorized(new { message = "No se pudo identificar al asesor." });

        var idAsesor = int.Parse(idUsuarioClaim);

        var totalClientes = await _context.Clientes
            .CountAsync(c => c.Activo && !c.Eliminado && c.IdAsesor == idAsesor);

        var deudasPendientes = await _context.Deuda
            .Where(d => d.Activo && !d.Eliminado && d.IdClienteNavigation.IdAsesor == idAsesor)
            .SumAsync(d => (decimal?)d.SaldoPendiente) ?? 0;

        var pagosRealizados = await _context.Pagos
            .Where(p => p.Activo && !p.Eliminado &&
                        p.IdDeudaNavigation.IdClienteNavigation.IdAsesor == idAsesor &&
                        p.EstadoPago == "CONFIRMADO")
            .SumAsync(p => (decimal?)p.Monto) ?? 0;

        var clientesMorosos = await _context.Deuda
            .Where(d => d.Activo && !d.Eliminado &&
                        d.IdClienteNavigation.IdAsesor == idAsesor &&
                        d.DiasAtraso > 0)
            .Select(d => d.IdCliente)
            .Distinct()
            .CountAsync();

        return Ok(new
        {
            totalClientes,
            deudasPendientes,
            pagosRealizados,
            clientesMorosos
        });
    }
}