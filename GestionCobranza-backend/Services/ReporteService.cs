using System;
using System.Linq;
using System.Threading.Tasks;
using GestionCobranza_backend.Dtos.Reporte;
using GestionCobranza_backend.Models;
using GestionCobranza_backend.Repositories;

namespace GestionCobranza_backend.Services
{
    public class ReporteService : IReporteService
    {
        private readonly IReporteRepository _reporteRepository;

        public ReporteService(IReporteRepository reporteRepository)
        {
            _reporteRepository = reporteRepository;
        }

        public async Task<ReporteGerencialDto> GetDashboardGerencialAsync()
        {
            var rendimiento = await _reporteRepository.GetRendimientoAsesoresAsync();
            var resumen = await _reporteRepository.GetResumenClientesAsync();
            var recientes = await _reporteRepository.GetReportesRecientesAsync();

            int totalClientes = resumen.Sum(r => r.Cantidad);
            if (totalClientes > 0)
            {
                foreach (var item in resumen)
                {
                    item.Porcentaje = Math.Round((double)item.Cantidad / totalClientes * 100, 1);
                }
            }

            return new ReporteGerencialDto
            {
                RendimientoAsesores = rendimiento,
                ResumenClientes = resumen,
                ReportesRecientes = recientes
            };
        }

        public async Task<string> GenerarYRegistrarReporteAsync(GenerarReporteRequestDto request, int idUsuario)
        {
            string urlFicticiaExcel = $"https://storage.audicob.com/reportes/Reporte_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            var adminReal = await _reporteRepository.GetAdministradorDisponibleAsync();

            if (adminReal == null)
            {
                throw new Exception("Error: No se encontró ningún usuario con rol de Administrador (id_rol = 1) activo en la base de datos.");
            }

            DateTime fechaSeguraPostgres = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

            var nuevoReporte = new ReporteGenerado
            {
                IdUsuario = adminReal.IdUsuario,
                NombreReporte = $"{request.TipoReporte}_{DateTime.Now:yyyyMMdd}",
                TipoReporte = request.TipoReporte,

                FechaDesde = request.FechaDesde.HasValue ? DateOnly.FromDateTime(request.FechaDesde.Value) : null,
                FechaHasta = request.FechaHasta.HasValue ? DateOnly.FromDateTime(request.FechaHasta.Value) : null,

                ArchivoUrl = urlFicticiaExcel,

                FechaGeneracion = fechaSeguraPostgres,
                FechaRegistro = fechaSeguraPostgres,
                UsuarioRegistro = $"{adminReal.Nombres} {adminReal.Apellidos}",

                Activo = true,
                Eliminado = false,
                IdUsuarioNavigation = null!
            };

            bool exito = await _reporteRepository.RegistrarReporteGeneradoAsync(nuevoReporte);

            if (!exito)
                throw new Exception("El motor de base de datos rechazó el registro físico del reporte.");

            return urlFicticiaExcel;
        }

        public async Task<ReporteRendimientoIndividualDto> GetDashboardIndividualAsync(int idAsesor)
        {
            var resumenCartera = await _reporteRepository.GetRendimientoIndividualAsync(idAsesor);
            var distribucion = await _reporteRepository.GetResumenClientesPorAsesorAsync(idAsesor);

            int totalClientesAsesor = distribucion.Sum(d => d.Cantidad);
            if (totalClientesAsesor > 0)
            {
                foreach (var item in distribucion)
                {
                    item.Porcentaje = Math.Round((double)item.Cantidad / totalClientesAsesor * 100, 1);
                }
            }

            return new ReporteRendimientoIndividualDto
            {
                ResumenCartera = resumenCartera,
                DistribucionClientes = distribucion
            };
        }

        public async Task<string> GenerarYRegistrarReporteAsesorAsync(GenerarReporteRequestDto request, int idAsesor)
        {
            // 1. URL de descarga simulada para el reporte del asesor
            string urlExcelAsesor = $"https://storage.audicob.com/reportes/Asesor_{idAsesor}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            // 2. CORRECCIÓN: Buscamos al asesor usando el REPOSITORIO, no el contexto directo
            var asesorReal = await _reporteRepository.GetAsesorPorIdAsync(idAsesor);

            if (asesorReal == null)
            {
                throw new Exception($"Error: No se encontró un asesor activo con el ID {idAsesor} en el sistema.");
            }

            // 3. Formateamos la estampa de tiempo compatible sin Kind UTC
            DateTime fechaSeguraPostgres = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

            // 4. Mapeo estructural de la entidad reporte_generado
            var nuevoReporte = new ReporteGenerado
            {
                IdUsuario = asesorReal.IdUsuario,
                NombreReporte = $"Rendimiento_{asesorReal.Apellidos}_{DateTime.Now:yyyyMMdd}",
                TipoReporte = request.TipoReporte ?? "Rendimiento Individual",

                FechaDesde = request.FechaDesde.HasValue ? DateOnly.FromDateTime(request.FechaDesde.Value) : null,
                FechaHasta = request.FechaHasta.HasValue ? DateOnly.FromDateTime(request.FechaHasta.Value) : null,

                ArchivoUrl = urlExcelAsesor,

                FechaGeneracion = fechaSeguraPostgres,
                FechaRegistro = fechaSeguraPostgres,
                UsuarioRegistro = $"{asesorReal.Nombres} {asesorReal.Apellidos}",

                Activo = true,
                Eliminado = false,
                IdUsuarioNavigation = null!
            };

            // 5. Guardamos en la base de datos a través del repositorio
            bool exito = await _reporteRepository.RegistrarReporteGeneradoAsync(nuevoReporte);

            if (!exito)
                throw new Exception("La base de datos rechazó el registro de auditoría de la descarga del asesor.");

            return urlExcelAsesor;
        }
    }
}