using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionCobranza_backend.Dtos.Morosidad;
using GestionCobranza_backend.Models;

namespace GestionCobranza_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MorosidadController : ControllerBase
{
    private readonly AppDbContext _context;

    public MorosidadController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardMorosidadDto>> GetDashboardMorosidad()
    {
        // Se cambió c.Deudas por c.Deuda (nombre exacto en tu modelo Cliente.cs)
        var clientesQuery = await _context.Clientes
            .Include(c => c.IdAsesorNavigation)
            .Include(c => c.Deuda.Where(d => !d.Eliminado && d.DiasAtraso > 0)) // PascalCase en propiedades
            .Where(c => !c.Eliminado && c.EstadoCliente == "MOROSO")
            .ToListAsync();

        if (!clientesQuery.Any())
        {
            return Ok(new DashboardMorosidadDto());
        }

        // Mapeo detallado adaptado a las propiedades de tus modelos
        var listaDetalle = clientesQuery.Select(c => new ClienteMorosoListaDto
        {
            IdCliente = c.IdCliente,
            NombreCompleto = $"{c.Nombres} {c.Apellidos}".Trim(),
            Correo = c.Correo ?? "",
            Telefono = c.Telefono ?? "",
            AsesorAsignado = c.IdAsesorNavigation != null
                ? $"{c.IdAsesorNavigation.Nombres} {c.IdAsesorNavigation.Apellidos}".Trim()
                : "Sin Asesor",
            DiasAtraso = c.Deuda.Any() ? c.Deuda.Max(d => d.DiasAtraso) : 0,
            DeudaPendiente = c.Deuda.Sum(d => d.SaldoPendiente), // Propiedades en PascalCase
            Riesgo = c.Riesgo,
            Estado = c.EstadoCliente
        }).ToList();

        // Construcción de las métricas superiores para las tarjetas del Dashboard
        var response = new DashboardMorosidadDto
        {
            ClientesMorosos = listaDetalle.Count,
            DeudaMorosaTotal = listaDetalle.Sum(ld => ld.DeudaPendiente),
            MorosidadCritica = listaDetalle.Count(ld => ld.DiasAtraso > 60),
            PromedioAtrasoDias = listaDetalle.Any() ? (int)listaDetalle.Average(ld => ld.DiasAtraso) : 0,
            DetalleClientes = listaDetalle
        };

        return Ok(response); // 200 OK
    }
}