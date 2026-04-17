using System.Security.Claims;
using GestionCobranza_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionCobranza_backend.Controllers;

[ApiController]
[Route("api/dashboard-asesor")]
[Authorize(Roles = "Asesor")]
public class DashboardAsesorController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardAsesorController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("metricas")]
    public async Task<IActionResult> GetMetricas()
    {
        var idAsesor = ObtenerIdAsesorDesdeToken();
        if (idAsesor == null)
            return Unauthorized(new { message = "No se pudo identificar al asesor autenticado." });

        var clientes = await _context.Clientes
            .Where(c => c.Activo && !c.Eliminado && c.IdAsesor == idAsesor.Value)
            .ToListAsync();

        var idsClientes = clientes.Select(c => c.IdCliente).ToList();

        var deudas = await _context.Deuda
            .Where(d => d.Activo && !d.Eliminado && idsClientes.Contains(d.IdCliente))
            .ToListAsync();

        var idsDeudas = deudas.Select(d => d.IdDeuda).ToList();

        var pagos = await _context.Pagos
            .Where(p => p.Activo && !p.Eliminado && p.EstadoPago == "CONFIRMADO" && idsDeudas.Contains(p.IdDeuda))
            .ToListAsync();

        var totalClientes = clientes.Count;

        var deudasPendientes = deudas.Sum(d => d.SaldoPendiente);
        var pagosRealizados = pagos.Sum(p => p.Monto);

        var clientesMorosidad = deudas
            .Where(d => d.DiasAtraso > 0)
            .Select(d => d.IdCliente)
            .Distinct()
            .Count();

        var response = new
        {
            totalClientes,
            deudasPendientes = $"S/. {deudasPendientes:N2}",
            pagosRealizados = $"S/. {pagosRealizados:N2}",
            clientesMorosidad,
            variacionClientes = $"+{totalClientes} este mes",
            variacionDeudas = "-0% vs mes anterior",
            variacionPagos = "+0% este mes",
            variacionMorosidad = "-0 vs semana pasada"
        };

        return Ok(response);
    }

    [HttpGet("distribucion-clientes")]
    public async Task<IActionResult> GetDistribucionClientes()
    {
        var idAsesor = ObtenerIdAsesorDesdeToken();
        if (idAsesor == null)
            return Unauthorized(new { message = "No se pudo identificar al asesor autenticado." });

        var clientes = await _context.Clientes
            .Where(c => c.Activo && !c.Eliminado && c.IdAsesor == idAsesor.Value)
            .ToListAsync();

        var data = clientes
            .GroupBy(c => c.FechaRegistro.Month)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                mes = ObtenerNombreMesCorto(g.Key),
                clientes = g.Count()
            })
            .ToList();

        return Ok(data);
    }

    [HttpGet("clasificacion-deudores")]
    public async Task<IActionResult> GetClasificacionDeudores()
    {
        var idAsesor = ObtenerIdAsesorDesdeToken();
        if (idAsesor == null)
            return Unauthorized(new { message = "No se pudo identificar al asesor autenticado." });

        var idsClientes = await _context.Clientes
            .Where(c => c.Activo && !c.Eliminado && c.IdAsesor == idAsesor.Value)
            .Select(c => c.IdCliente)
            .ToListAsync();

        var deudas = await _context.Deuda
            .Where(d => d.Activo && !d.Eliminado && idsClientes.Contains(d.IdCliente))
            .ToListAsync();

        var alDia = 0;
        var atrasoLeve = 0;
        var morosidad = 0;
        var critico = 0;

        foreach (var deuda in deudas)
        {
            if (deuda.DiasAtraso <= 0)
                alDia++;
            else if (deuda.DiasAtraso <= 30)
                atrasoLeve++;
            else if (deuda.DiasAtraso <= 60)
                morosidad++;
            else
                critico++;
        }

        var total = alDia + atrasoLeve + morosidad + critico;

        if (total == 0)
        {
            return Ok(new[]
            {
                new { name = "Al día 0%", valor = 0 },
                new { name = "Atraso leve 0%", valor = 0 },
                new { name = "Morosidad 0%", valor = 0 },
                new { name = "Crítico 0%", valor = 0 }
            });
        }

        var data = new[]
        {
            new { name = $"Al día {Porcentaje(alDia, total)}%", valor = alDia },
            new { name = $"Atraso leve {Porcentaje(atrasoLeve, total)}%", valor = atrasoLeve },
            new { name = $"Morosidad {Porcentaje(morosidad, total)}%", valor = morosidad },
            new { name = $"Crítico {Porcentaje(critico, total)}%", valor = critico }
        };

        return Ok(data);
    }

    [HttpGet("tendencia-morosidad")]
    public async Task<IActionResult> GetTendenciaMorosidad()
    {
        var idAsesor = ObtenerIdAsesorDesdeToken();
        if (idAsesor == null)
            return Unauthorized(new { message = "No se pudo identificar al asesor autenticado." });

        var idsClientes = await _context.Clientes
            .Where(c => c.Activo && !c.Eliminado && c.IdAsesor == idAsesor.Value)
            .Select(c => c.IdCliente)
            .ToListAsync();

        var deudas = await _context.Deuda
            .Where(d => d.Activo && !d.Eliminado && idsClientes.Contains(d.IdCliente))
            .ToListAsync();

        var data = deudas
            .GroupBy(d => d.FechaVencimiento.Month)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                mes = ObtenerNombreMesCorto(g.Key),
                morosidad = g.Sum(x => x.SaldoPendiente)
            })
            .ToList();

        return Ok(data);
    }

    private int? ObtenerIdAsesorDesdeToken()
    {
        var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(idUsuarioClaim))
            return null;

        if (!int.TryParse(idUsuarioClaim, out var idAsesor))
            return null;

        return idAsesor;
    }

    private static int Porcentaje(int parte, int total)
    {
        if (total == 0) return 0;
        return (int)Math.Round((double)parte * 100 / total, 0);
    }

    private static string ObtenerNombreMesCorto(int mes)
    {
        return mes switch
        {
            1 => "Ene",
            2 => "Feb",
            3 => "Mar",
            4 => "Abr",
            5 => "May",
            6 => "Jun",
            7 => "Jul",
            8 => "Ago",
            9 => "Sep",
            10 => "Oct",
            11 => "Nov",
            12 => "Dic",
            _ => "Mes"
        };
    }
}