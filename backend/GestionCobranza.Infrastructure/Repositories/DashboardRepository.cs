using GestionCobranza.Application.Interfaces;
using GestionCobranza.Domain.Entities;
using GestionCobranza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionCobranza.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _ctx;
    public DashboardRepository(ApplicationDbContext ctx) => _ctx = ctx;

    // ─── Tarjeta 1: Total de clientes activos ────────────────────────────────
    public Task<int> ContarClientesActivosAsync(int idAsesor)
        => _ctx.Clientes.CountAsync(c => c.id_asesor == idAsesor && c.activo && !c.eliminado);

    // Nuevos clientes del mes actual (para comparativo "+12 este mes")
    public Task<int> ContarClientesNuevosMesActualAsync(int idAsesor)
    {
        var hoy     = DateTime.UtcNow;
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
        return _ctx.Clientes.CountAsync(c =>
            c.id_asesor == idAsesor && c.activo && !c.eliminado &&
            c.fecha_registro >= inicioMes);
    }

    // ─── Tarjeta 2: Deuda pendiente ──────────────────────────────────────────
    public async Task<decimal> SumarSaldoPendienteAsync(int idAsesor)
        => await _ctx.Clientes
            .Where(c => c.id_asesor == idAsesor && c.activo && !c.eliminado)
            .SumAsync(c => (decimal?)c.saldo_pendiente) ?? 0m;

    // Saldo pendiente del mes anterior (para comparativo porcentual)
    public async Task<decimal> SumarSaldoPendienteMesAnteriorAsync(int idAsesor)
    {
        var hoy          = DateTime.UtcNow;
        var inicioMesAnt = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(-1);
        var finMesAnt    = new DateTime(hoy.Year, hoy.Month, 1).AddDays(-1);
        return await _ctx.Clientes
            .Where(c => c.id_asesor == idAsesor && !c.eliminado &&
                        c.fecha_registro <= finMesAnt)
            .SumAsync(c => (decimal?)c.saldo_pendiente) ?? 0m;
    }

    // ─── Tarjeta 3: Pagos realizados ─────────────────────────────────────────
    public async Task<decimal> SumarPagosConfirmadosAsync(int idAsesor)
        => await _ctx.Pagos
            .Where(p => p.id_asesor == idAsesor && p.estado_pago == "Confirmado")
            .SumAsync(p => (decimal?)p.monto) ?? 0m;

    public async Task<decimal> SumarPagosConfirmadosMesAnteriorAsync(int idAsesor)
    {
        var hoy          = DateTime.UtcNow;
        var inicioMesAnt = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(-1);
        var finMesAnt    = new DateTime(hoy.Year, hoy.Month, 1).AddDays(-1);
        return await _ctx.Pagos
            .Where(p => p.id_asesor == idAsesor && p.estado_pago == "Confirmado" &&
                        p.fecha_registro >= inicioMesAnt && p.fecha_registro <= finMesAnt)
            .SumAsync(p => (decimal?)p.monto) ?? 0m;
    }

    // ─── Tarjeta 4: Clientes en morosidad ────────────────────────────────────
    public Task<int> ContarClientesMorososAsync(int idAsesor)
        => _ctx.Clientes.CountAsync(c =>
            c.id_asesor == idAsesor && c.activo && !c.eliminado &&
            c.estado_cliente == "Moroso");

    // Morosos de hace 7 días (para comparativo "-5 vs semana pasada")
    public Task<int> ContarClientesMorososSemanaAnteriorAsync(int idAsesor)
    {
        var hace7dias = DateTime.UtcNow.AddDays(-7);
        return _ctx.Clientes.CountAsync(c =>
            c.id_asesor == idAsesor && !c.eliminado &&
            c.estado_cliente == "Moroso" &&
            c.fecha_registro <= hace7dias);
    }

    // ─── Gráfico de líneas: nuevos clientes por mes ──────────────────────────
    public async Task<List<(int Year, int Month, int NuevosClientes, int ClientesConPago)>>
        ObtenerDistribucionMensualAsync(int idAsesor)
    {
        var desde = DateTime.UtcNow.AddMonths(-5).Date;
        var datos = await _ctx.Clientes
            .Where(c => c.id_asesor == idAsesor && !c.eliminado &&
                        c.fecha_registro >= desde)
            .GroupBy(c => new { c.fecha_registro.Year, c.fecha_registro.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                NuevosClientes = g.Count()
            })
            .ToListAsync();

        return datos.Select(d => (d.Year, d.Month, d.NuevosClientes, 0)).ToList();
    }

    // ─── Gráfico de barras: morosos por mes ──────────────────────────────────
    public async Task<List<(int Year, int Month, int Morosos)>>
        ObtenerTendenciaMorosidadAsync(int idAsesor)
    {
        var desde = DateTime.UtcNow.AddMonths(-5).Date;
        var datos = await _ctx.Pagos
            .Where(p => p.id_asesor == idAsesor && p.fecha_registro >= desde)
            .Join(_ctx.Clientes,
                  p => p.id_cliente,
                  c => c.id_cliente,
                  (p, c) => new { p.fecha_registro.Year, p.fecha_registro.Month, c.estado_cliente })
            .Where(x => x.estado_cliente == "Moroso")
            .GroupBy(x => new { x.Year, x.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Morosos = g.Count() })
            .ToListAsync();

        return datos.Select(d => (d.Year, d.Month, d.Morosos)).ToList();
    }

    // ─── Clasificación: clientes con vencimiento para calcular días de atraso ─
    public Task<List<Cliente>> ObtenerClientesConVencimientoAsync(int idAsesor)
        => _ctx.Clientes
            .Where(c => c.id_asesor == idAsesor && c.activo && !c.eliminado)
            .Select(c => new Cliente
            {
                id_cliente       = c.id_cliente,
                fecha_vencimiento = c.fecha_vencimiento,
                saldo_pendiente  = c.saldo_pendiente
            })
            .ToListAsync();

    // ─── Clientes con pago por mes (segunda línea del gráfico de distribución) ─
    public async Task<List<(int Year, int Month, int ClienteId)>>
        ObtenerPagosClientesPorMesAsync(int idAsesor)
    {
        var desde = DateTime.UtcNow.AddMonths(-5).Date;
        var datos = await _ctx.Pagos
            .Where(p => p.id_asesor == idAsesor &&
                        p.estado_pago == "Confirmado" &&
                        p.fecha_registro >= desde)
            .Select(p => new { p.fecha_registro.Year, p.fecha_registro.Month, p.id_cliente })
            .Distinct()
            .ToListAsync();

        return datos.Select(d => (d.Year, d.Month, d.id_cliente)).ToList();
    }
}
