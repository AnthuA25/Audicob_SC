using GestionCobranza_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionCobranza_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("metricas")]
    public async Task<IActionResult> GetMetricas()
    {
        var totalAsesores = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .CountAsync(u =>
                u.Activo &&
                !u.Eliminado &&
                u.IdRolNavigation.Nombre == "Asesor");

        var totalClientes = await _context.Clientes
            .CountAsync(c => c.Activo && !c.Eliminado);

        var cobranzaTotal = await _context.Pagos
            .Where(p =>
                p.Activo &&
                !p.Eliminado &&
                p.EstadoPago == "CONFIRMADO")
            .SumAsync(p => (decimal?)p.Monto) ?? 0;

        var totalDeuda = await _context.Deuda
            .Where(d => d.Activo && !d.Eliminado)
            .SumAsync(d => (decimal?)d.MontoTotal) ?? 0;

        var eficienciaGlobal = totalDeuda > 0
            ? Math.Round((cobranzaTotal / totalDeuda) * 100, 1)
            : 0;

        var response = new
        {
            totalAsesores,
            totalClientes,
            cobranzaTotal = $"S/. {cobranzaTotal:N2}",
            eficienciaGlobal = $"{eficienciaGlobal}%",
            variacionAsesores = "+0 este mes",
            variacionClientes = "+0 este mes",
            variacionCobranza = "+0%",
            variacionEficiencia = "+0%"
        };

        return Ok(response);
    }

    [HttpGet("cobranza-evolucion")]
    public async Task<IActionResult> GetCobranzaEvolucion()
    {
        var pagos = await _context.Pagos
            .Where(p =>
                p.Activo &&
                !p.Eliminado &&
                p.EstadoPago == "CONFIRMADO")
            .ToListAsync();

        var data = pagos
            .GroupBy(p => p.FechaPago.Month)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                mes = ObtenerNombreMes(g.Key),
                cobranza = g.Sum(x => x.Monto)
            })
            .ToList();

        return Ok(data);
    }

    [HttpGet("distribucion-clientes")]
    public async Task<IActionResult> GetDistribucionClientes()
    {
        var clientesIds = await _context.Clientes
            .Where(c => c.Activo && !c.Eliminado)
            .Select(c => c.IdCliente)
            .ToListAsync();

        var deudas = await _context.Deuda
            .Where(d => d.Activo && !d.Eliminado)
            .ToListAsync();

        var alDia = 0;
        var alerta = 0;
        var morosos = 0;

        foreach (var idCliente in clientesIds)
        {
            var deudaCliente = deudas
                .Where(d => d.IdCliente == idCliente)
                .OrderByDescending(d => d.DiasAtraso)
                .FirstOrDefault();

            if (deudaCliente == null || deudaCliente.DiasAtraso <= 0)
            {
                alDia++;
            }
            else if (deudaCliente.DiasAtraso > 0 && deudaCliente.DiasAtraso <= 30)
            {
                alerta++;
            }
            else
            {
                morosos++;
            }
        }

        var data = new[]
        {
            new { valor = alDia },
            new { valor = alerta },
            new { valor = morosos }
        };

        return Ok(data);
    }

    [HttpGet("rendimiento-asesores")]
    public async Task<IActionResult> GetRendimientoAsesores()
    {
        var asesores = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .Where(u =>
                u.Activo &&
                !u.Eliminado &&
                u.IdRolNavigation.Nombre == "Asesor")
            .ToListAsync();

        var clientes = await _context.Clientes
            .Where(c => c.Activo && !c.Eliminado)
            .ToListAsync();

        var deudas = await _context.Deuda
            .Where(d => d.Activo && !d.Eliminado)
            .ToListAsync();

        var pagos = await _context.Pagos
            .Where(p =>
                p.Activo &&
                !p.Eliminado &&
                p.EstadoPago == "CONFIRMADO")
            .ToListAsync();

        var resultado = asesores.Select(asesor =>
        {
            var clientesAsesor = clientes
                .Where(c => c.IdAsesor == asesor.IdUsuario)
                .ToList();

            var idsClientes = clientesAsesor.Select(c => c.IdCliente).ToList();

            var deudasAsesor = deudas
                .Where(d => idsClientes.Contains(d.IdCliente))
                .ToList();

            var idsDeudas = deudasAsesor.Select(d => d.IdDeuda).ToList();

            var pagosAsesor = pagos
                .Where(p => idsDeudas.Contains(p.IdDeuda))
                .ToList();

            var deudaGestionada = pagosAsesor.Sum(p => p.Monto);
            var totalDeuda = deudasAsesor.Sum(d => d.MontoTotal);

            var eficiencia = totalDeuda > 0
                ? (int)Math.Round((deudaGestionada / totalDeuda) * 100, 0)
                : 0;

            return new
            {
                nombre = $"{asesor.Nombres} {asesor.Apellidos}",
                deudaGestionada = $"S/. {deudaGestionada:N2}",
                clientes = clientesAsesor.Count,
                eficiencia
            };
        })
        .OrderByDescending(x => x.eficiencia)
        .ToList();

        return Ok(resultado);
    }

    private static string ObtenerNombreMes(int mes)
    {
        return mes switch
        {
            1 => "Enero",
            2 => "Febrero",
            3 => "Marzo",
            4 => "Abril",
            5 => "Mayo",
            6 => "Junio",
            7 => "Julio",
            8 => "Agosto",
            9 => "Septiembre",
            10 => "Octubre",
            11 => "Noviembre",
            12 => "Diciembre",
            _ => "Mes"
        };
    }
}