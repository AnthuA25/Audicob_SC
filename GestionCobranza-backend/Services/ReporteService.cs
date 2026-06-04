using ClosedXML.Excel;
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

            CalcularPorcentajes(resumen);

            return new ReporteGerencialDto
            {
                RendimientoAsesores = rendimiento,
                ResumenClientes = resumen,
                ReportesRecientes = recientes
            };
        }

        public async Task<ReporteRendimientoIndividualDto> GetDashboardIndividualAsync(int idAsesor)
        {
            var resumenCartera = await _reporteRepository.GetRendimientoIndividualAsync(idAsesor);
            var distribucion = await _reporteRepository.GetResumenClientesPorAsesorAsync(idAsesor);
            var recientes = await _reporteRepository.GetReportesRecientesPorUsuarioAsync(idAsesor);

            CalcularPorcentajes(distribucion);

            return new ReporteRendimientoIndividualDto
            {
                ResumenCartera = resumenCartera,
                DistribucionClientes = distribucion,
                ReportesRecientes = recientes
            };
        }

        public async Task<ReporteGeneradoResponseDto> GenerarReporteAdminAsync(
            GenerarReporteRequestDto request,
            int idUsuario,
            string baseUrl)
        {
            var usuario = await _reporteRepository.GetUsuarioPorIdAsync(idUsuario);

            if (usuario == null)
                throw new Exception("No se encontró el usuario autenticado.");

            var data = await GetDashboardGerencialAsync();

            string nombreArchivo = $"reporte_admin_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string urlDescarga = CrearExcelAdmin(nombreArchivo, data, baseUrl);

            var fecha = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

            var reporte = new ReporteGenerado
            {
                IdUsuario = usuario.IdUsuario,
                NombreReporte = request.TipoReporte,
                TipoReporte = request.TipoReporte,
                FechaDesde = request.FechaDesde.HasValue
                    ? DateOnly.FromDateTime(request.FechaDesde.Value)
                    : null,
                FechaHasta = request.FechaHasta.HasValue
                    ? DateOnly.FromDateTime(request.FechaHasta.Value)
                    : null,
                ArchivoUrl = urlDescarga,
                FechaGeneracion = fecha,
                FechaRegistro = fecha,
                UsuarioRegistro = $"{usuario.Nombres} {usuario.Apellidos}",
                Activo = true,
                Eliminado = false
            };

            int idReporte = await _reporteRepository.RegistrarReporteGeneradoAsync(reporte);

            return new ReporteGeneradoResponseDto
            {
                Mensaje = "Reporte generado correctamente",
                IdReporte = idReporte,
                NombreArchivo = nombreArchivo,
                UrlDescarga = urlDescarga
            };
        }

        public async Task<ReporteGeneradoResponseDto> GenerarReporteAsesorAsync(
            GenerarReporteRequestDto request,
            int idAsesor,
            string baseUrl)
        {
            var asesor = await _reporteRepository.GetUsuarioPorIdAsync(idAsesor);

            if (asesor == null)
                throw new Exception("No se encontró el asesor autenticado.");

            var data = await GetDashboardIndividualAsync(idAsesor);

            string nombreArchivo = $"reporte_asesor_{idAsesor}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string urlDescarga = CrearExcelAsesor(nombreArchivo, data, baseUrl);

            var fecha = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

            var reporte = new ReporteGenerado
            {
                IdUsuario = asesor.IdUsuario,
                NombreReporte = request.TipoReporte,
                TipoReporte = request.TipoReporte,
                FechaDesde = request.FechaDesde.HasValue
                    ? DateOnly.FromDateTime(request.FechaDesde.Value)
                    : null,
                FechaHasta = request.FechaHasta.HasValue
                    ? DateOnly.FromDateTime(request.FechaHasta.Value)
                    : null,
                ArchivoUrl = urlDescarga,
                FechaGeneracion = fecha,
                FechaRegistro = fecha,
                UsuarioRegistro = $"{asesor.Nombres} {asesor.Apellidos}",
                Activo = true,
                Eliminado = false
            };

            int idReporte = await _reporteRepository.RegistrarReporteGeneradoAsync(reporte);

            return new ReporteGeneradoResponseDto
            {
                Mensaje = "Reporte generado correctamente",
                IdReporte = idReporte,
                NombreArchivo = nombreArchivo,
                UrlDescarga = urlDescarga
            };
        }

        private static void CalcularPorcentajes(List<ResumenClienteDto> resumen)
        {
            int total = resumen.Sum(x => x.Cantidad);

            foreach (var item in resumen)
            {
                item.Porcentaje = total > 0
                    ? Math.Round((double)item.Cantidad / total * 100, 1)
                    : 0;
            }
        }

        private static string CrearExcelAdmin(
            string nombreArchivo,
            ReporteGerencialDto data,
            string baseUrl)
        {
            using var workbook = new XLWorkbook();

            var ws1 = workbook.Worksheets.Add("Rendimiento Asesores");

            ws1.Cell(1, 1).Value = "Asesor";
            ws1.Cell(1, 2).Value = "Clientes";
            ws1.Cell(1, 3).Value = "Deuda Gestionada";
            ws1.Cell(1, 4).Value = "Pagos Recuperados";
            ws1.Cell(1, 5).Value = "Eficiencia";

            int fila = 2;

            foreach (var item in data.RendimientoAsesores)
            {
                ws1.Cell(fila, 1).Value = item.Asesor;
                ws1.Cell(fila, 2).Value = item.Clientes;
                ws1.Cell(fila, 3).Value = item.DeudaGestionada;
                ws1.Cell(fila, 4).Value = item.PagosRecuperados;
                ws1.Cell(fila, 5).Value = item.Eficiencia;
                fila++;
            }

            var ws2 = workbook.Worksheets.Add("Resumen Clientes");

            ws2.Cell(1, 1).Value = "Estado";
            ws2.Cell(1, 2).Value = "Cantidad";
            ws2.Cell(1, 3).Value = "Deuda Total";
            ws2.Cell(1, 4).Value = "Porcentaje";

            fila = 2;

            foreach (var item in data.ResumenClientes)
            {
                ws2.Cell(fila, 1).Value = item.Estado;
                ws2.Cell(fila, 2).Value = item.Cantidad;
                ws2.Cell(fila, 3).Value = item.DeudaTotal;
                ws2.Cell(fila, 4).Value = item.Porcentaje + "%";
                fila++;
            }

            ws1.Columns().AdjustToContents();
            ws2.Columns().AdjustToContents();

            string carpeta = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "reportes"
            );

            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            string rutaFisica = Path.Combine(carpeta, nombreArchivo);

            workbook.SaveAs(rutaFisica);

            return $"{baseUrl}/reportes/{nombreArchivo}";
        }

        private static string CrearExcelAsesor(
            string nombreArchivo,
            ReporteRendimientoIndividualDto data,
            string baseUrl)
        {
            using var workbook = new XLWorkbook();

            var ws1 = workbook.Worksheets.Add("Resumen Cartera");

            ws1.Cell(1, 1).Value = "Total Clientes";
            ws1.Cell(1, 2).Value = "Total Deuda";
            ws1.Cell(1, 3).Value = "Pagos Recuperados";
            ws1.Cell(1, 4).Value = "Eficiencia";

            ws1.Cell(2, 1).Value = data.ResumenCartera.TotalClientesAsignados;
            ws1.Cell(2, 2).Value = data.ResumenCartera.TotalDeudaAsignada;
            ws1.Cell(2, 3).Value = data.ResumenCartera.TotalPagosRecuperados;
            ws1.Cell(2, 4).Value = data.ResumenCartera.EficienciaIndividual;

            var ws2 = workbook.Worksheets.Add("Distribución Clientes");

            ws2.Cell(1, 1).Value = "Estado";
            ws2.Cell(1, 2).Value = "Cantidad";
            ws2.Cell(1, 3).Value = "Deuda Total";
            ws2.Cell(1, 4).Value = "Porcentaje";

            int fila = 2;

            foreach (var item in data.DistribucionClientes)
            {
                ws2.Cell(fila, 1).Value = item.Estado;
                ws2.Cell(fila, 2).Value = item.Cantidad;
                ws2.Cell(fila, 3).Value = item.DeudaTotal;
                ws2.Cell(fila, 4).Value = item.Porcentaje + "%";
                fila++;
            }

            ws1.Columns().AdjustToContents();
            ws2.Columns().AdjustToContents();

            string carpeta = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "reportes"
            );

            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            string rutaFisica = Path.Combine(carpeta, nombreArchivo);

            workbook.SaveAs(rutaFisica);

            return $"{baseUrl}/reportes/{nombreArchivo}";
        }
    }
}