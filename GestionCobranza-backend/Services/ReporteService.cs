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
            // 1. Definimos la URL de descarga para el archivo Excel
            string urlFicticiaExcel = $"https://storage.audicob.com/reportes/Reporte_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            // 2. Buscamos el Administrador verídico en la BD
            var adminReal = await _reporteRepository.GetAdministradorDisponibleAsync();

            if (adminReal == null)
            {
                throw new Exception("Error: No se encontró ningún usuario con rol de Administrador (id_rol = 1) activo en la base de datos.");
            }

            // ====================================================================
            // SOLUCIÓN AL ERROR DE TIMESTAMP:
            // Creamos la fecha local actual y le removemos explícitamente el Kind UTC
            // transformándolo en 'Unspecified' para que coincida con 'timestamp without time zone'
            // ====================================================================
            DateTime fechaSeguraPostgres = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

            // 3. Mapeo seguro con tipos primitivos purificados para tu base de datos
            var nuevoReporte = new ReporteGenerado
            {
                IdUsuario = adminReal.IdUsuario,
                NombreReporte = $"{request.TipoReporte}_{DateTime.Now:yyyyMMdd}",
                TipoReporte = request.TipoReporte,

                // Conversión limpia a DateOnly? compatible con tu Scaffold
                FechaDesde = request.FechaDesde.HasValue ? DateOnly.FromDateTime(request.FechaDesde.Value) : null,
                FechaHasta = request.FechaHasta.HasValue ? DateOnly.FromDateTime(request.FechaHasta.Value) : null,

                ArchivoUrl = urlFicticiaExcel,

                // Asignamos las estampas de tiempo compatibles sin zona horaria
                FechaGeneracion = fechaSeguraPostgres,
                FechaRegistro = fechaSeguraPostgres,
                UsuarioRegistro = $"{adminReal.Nombres} {adminReal.Apellidos}",

                Activo = true,
                Eliminado = false,

                // Anulamos la navegación para evitar bucles de rastreo gráficos de EF Core
                IdUsuarioNavigation = null!
            };

            // 4. Guardamos los cambios en la base de datos de manera asíncrona
            bool exito = await _reporteRepository.RegistrarReporteGeneradoAsync(nuevoReporte);

            if (!exito)
                throw new Exception("El motor de base de datos rechazó el registro físico del reporte.");

            return urlFicticiaExcel;
        }
    }
}