using GestionCobranza_backend.Dtos.Reporte;
using GestionCobranza_backend.Models;
using Microsoft.EntityFrameworkCore;

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
            var asesores = await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.Activo && !u.Eliminado && u.IdRol != 1)
                .Select(u => new
                {
                    u.IdUsuario,
                    Nombre = u.Nombres + " " + u.Apellidos
                })
                .ToListAsync();

            var resultado = new List<RendimientoAsesorDto>();

            foreach (var asesor in asesores)
            {
                var clientesIds = await _context.Clientes
                    .Where(c => c.IdAsesor == asesor.IdUsuario && c.Activo && !c.Eliminado)
                    .Select(c => c.IdCliente)
                    .ToListAsync();

                var deudas = await _context.Deuda
                    .Where(d => clientesIds.Contains(d.IdCliente) && d.Activo && !d.Eliminado)
                    .ToListAsync();

                decimal deudaTotal = deudas.Sum(d => d.MontoTotal);
                decimal pagos = deudas.Sum(d => d.MontoPagado);

                string eficiencia = deudaTotal > 0
                    ? $"{Math.Round((pagos / deudaTotal) * 100, 1)}%"
                    : "0%";

                resultado.Add(new RendimientoAsesorDto
                {
                    Asesor = asesor.Nombre,
                    Clientes = clientesIds.Count,
                    DeudaGestionada = deudaTotal,
                    PagosRecuperados = pagos,
                    Eficiencia = eficiencia
                });
            }

            return resultado;
        }

        public async Task<List<ResumenClienteDto>> GetResumenClientesAsync()
        {
            return await _context.Clientes
                .AsNoTracking()
                .Where(c => c.Activo && !c.Eliminado)
                .GroupBy(c => c.EstadoCliente)
                .Select(g => new ResumenClienteDto
                {
                    Estado = g.Key,
                    Cantidad = g.Count(),
                    DeudaTotal = _context.Deuda
                        .Where(d => g.Select(c => c.IdCliente).Contains(d.IdCliente)
                                    && d.Activo && !d.Eliminado)
                        .Sum(d => d.MontoTotal),
                    Porcentaje = 0
                })
                .ToListAsync();
        }

        public async Task<ResumenAsesorDto> GetRendimientoIndividualAsync(int idAsesor)
        {
            var clientesIds = await _context.Clientes
                .Where(c => c.IdAsesor == idAsesor && c.Activo && !c.Eliminado)
                .Select(c => c.IdCliente)
                .ToListAsync();

            var deudas = await _context.Deuda
                .Where(d => clientesIds.Contains(d.IdCliente) && d.Activo && !d.Eliminado)
                .ToListAsync();

            decimal deudaTotal = deudas.Sum(d => d.MontoTotal);
            decimal pagos = deudas.Sum(d => d.MontoPagado);

            string eficiencia = deudaTotal > 0
                ? $"{Math.Round((pagos / deudaTotal) * 100, 1)}%"
                : "0%";

            return new ResumenAsesorDto
            {
                TotalClientesAsignados = clientesIds.Count,
                TotalDeudaAsignada = deudaTotal,
                TotalPagosRecuperados = pagos,
                EficienciaIndividual = eficiencia
            };
        }

        public async Task<List<ResumenClienteDto>> GetResumenClientesPorAsesorAsync(int idAsesor)
        {
            return await _context.Clientes
                .AsNoTracking()
                .Where(c => c.IdAsesor == idAsesor && c.Activo && !c.Eliminado)
                .GroupBy(c => c.EstadoCliente)
                .Select(g => new ResumenClienteDto
                {
                    Estado = g.Key,
                    Cantidad = g.Count(),
                    DeudaTotal = _context.Deuda
                        .Where(d => g.Select(c => c.IdCliente).Contains(d.IdCliente)
                                    && d.Activo && !d.Eliminado)
                        .Sum(d => d.MontoTotal),
                    Porcentaje = 0
                })
                .ToListAsync();
        }

        public async Task<List<ReporteRecienteDto>> GetReportesRecientesAsync()
        {
            return await _context.ReporteGenerados
                .AsNoTracking()
                .Where(r => r.Activo && !r.Eliminado)
                .OrderByDescending(r => r.FechaGeneracion)
                .Take(5)
                .Select(r => new ReporteRecienteDto
                {
                    IdReporte = r.IdReporte,
                    NombreReporte = r.NombreReporte,
                    TipoReporte = r.TipoReporte,
                    FechaGeneracion = r.FechaGeneracion,
                    ArchivoUrl = r.ArchivoUrl ?? ""
                })
                .ToListAsync();
        }

        public async Task<List<ReporteRecienteDto>> GetReportesRecientesPorUsuarioAsync(int idUsuario)
        {
            return await _context.ReporteGenerados
                .AsNoTracking()
                .Where(r => r.IdUsuario == idUsuario && r.Activo && !r.Eliminado)
                .OrderByDescending(r => r.FechaGeneracion)
                .Take(5)
                .Select(r => new ReporteRecienteDto
                {
                    IdReporte = r.IdReporte,
                    NombreReporte = r.NombreReporte,
                    TipoReporte = r.TipoReporte,
                    FechaGeneracion = r.FechaGeneracion,
                    ArchivoUrl = r.ArchivoUrl ?? ""
                })
                .ToListAsync();
        }

        public async Task<Usuario?> GetUsuarioPorIdAsync(int idUsuario)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario && u.Activo && !u.Eliminado);
        }

        public async Task<int> RegistrarReporteGeneradoAsync(ReporteGenerado reporte)
        {
            await _context.ReporteGenerados.AddAsync(reporte);
            await _context.SaveChangesAsync();
            return reporte.IdReporte;
        }
    }
}