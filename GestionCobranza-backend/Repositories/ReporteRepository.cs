using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GestionCobranza_backend.Models;
using GestionCobranza_backend.Dtos.Reporte;

namespace GestionCobranza_backend.Repositories
{
    public class ReporteRepository : IReporteRepository
    {
        private readonly AppDbContext _context;

        public ReporteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RendimientoAsesorDto>> GetRendimientoAsesoresAsync()
        {
            return await _context.Set<Usuario>()
                .AsNoTracking()
                .Where(u => u.Activo && !u.Eliminado)
                .Select(u => new RendimientoAsesorDto
                {
                    Asesor = $"{u.Nombres} {u.Apellidos}",
                    Clientes = _context.Set<Cliente>().Count(c => c.IdAsesor == u.IdUsuario && c.Activo && !c.Eliminado),

                    DeudaGestionada = _context.Set<Deudum>()
                        .Where(d => _context.Set<Cliente>().Any(c => c.IdCliente == d.IdCliente && c.IdAsesor == u.IdUsuario))
                        .Sum(d => d.MontoTotal),

                    PagosRecuperados = _context.Set<Deudum>()
                        .Where(d => _context.Set<Cliente>().Any(c => c.IdCliente == d.IdCliente && c.IdAsesor == u.IdUsuario))
                        .Sum(d => d.MontoPagado),

                    Eficiencia = "85%"
                }).ToListAsync();
        }

        public async Task<List<ResumenClienteDto>> GetResumenClientesAsync()
        {
            return await _context.Set<Cliente>()
                .AsNoTracking()
                .Where(c => c.Activo && !c.Eliminado)
                .GroupBy(c => c.EstadoCliente)
                .Select(g => new ResumenClienteDto
                {
                    Estado = g.Key ?? "Desconocido",
                    Cantidad = g.Count(),

                    DeudaTotal = _context.Set<Deudum>()
                        .Where(d => g.Select(c => c.IdCliente).Contains(d.IdCliente))
                        .Sum(d => d.MontoTotal),

                    Porcentaje = 0.0
                }).ToListAsync();
        }

        public async Task<List<ReporteRecienteDto>> GetReportesRecientesAsync()
        {
            return await _context.Set<ReporteGenerado>()
                .AsNoTracking()
                .Where(r => r.Activo && !r.Eliminado)
                .OrderByDescending(r => r.FechaGeneracion)
                .Take(5)
                .Select(r => new ReporteRecienteDto
                {
                    IdReporte = r.IdReporte,
                    NombreReporte = r.NombreReporte,
                    FechaGeneracion = r.FechaGeneracion,
                    ArchivoUrl = r.ArchivoUrl ?? string.Empty
                }).ToListAsync();
        }

        public async Task<bool> RegistrarReporteGeneradoAsync(ReporteGenerado reporte)
        {
            await _context.Set<ReporteGenerado>().AddAsync(reporte);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Usuario?> GetAdministradorDisponibleAsync()
        {
            return await _context.Set<Usuario>()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Activo && !u.Eliminado && u.IdRol == 1);
        }

        public async Task<ResumenAsesorDto> GetRendimientoIndividualAsync(int idAsesor)
        {
            var totalClientes = await _context.Set<Cliente>()
                .AsNoTracking()
                .CountAsync(c => c.IdAsesor == idAsesor && c.Activo && !c.Eliminado);

            var deudasAsesor = await _context.Set<Deudum>()
                .AsNoTracking()
                .Where(d => _context.Set<Cliente>().Any(c => c.IdCliente == d.IdCliente && c.IdAsesor == idAsesor && c.Activo && !c.Eliminado))
                .ToListAsync();

            decimal deudaTotal = deudasAsesor.Sum(d => d.MontoTotal);
            decimal montoPagado = deudasAsesor.Sum(d => d.MontoPagado);

            double eficienciaCalculada = deudasAsesor.Count > 0 ? (deudasAsesor.Count(d => d.EstadoDeuda == "PAGADO") / (double)deudasAsesor.Count) * 100 : 0.0;

            return new ResumenAsesorDto
            {
                TotalClientesAsignados = totalClientes,
                TotalDeudaAsignada = deudaTotal,
                TotalPagosRecuperados = montoPagado,
                EficienciaIndividual = $"{Math.Round(eficienciaCalculada, 1)}%"
            };
        }

        public async Task<List<ResumenClienteDto>> GetResumenClientesPorAsesorAsync(int idAsesor)
        {
            return await _context.Set<Cliente>()
                .AsNoTracking()
                .Where(c => c.IdAsesor == idAsesor && c.Activo && !c.Eliminado)
                .GroupBy(c => c.EstadoCliente)
                .Select(g => new ResumenClienteDto
                {
                    Estado = g.Key ?? "Desconocido",
                    Cantidad = g.Count(),
                    DeudaTotal = _context.Set<Deudum>().Where(d => g.Select(c => c.IdCliente).Contains(d.IdCliente)).Sum(d => d.MontoTotal),
                    Porcentaje = 0.0
                }).ToListAsync();
        }
        public async Task<Usuario?> GetAsesorPorIdAsync(int idAsesor)
        {
            return await _context.Set<Usuario>()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdUsuario == idAsesor && u.Activo && !u.Eliminado);
        }
    }
}