using ClosedXML.Excel;
using GestionCobranza_backend.Dtos.Reporte;
using GestionCobranza_backend.Models;
using GestionCobranza_backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GestionCobranza_backend.Services
{
    public class ReporteService : IReporteService
    {
        private readonly IReporteRepository _reporteRepository;
        private readonly AppDbContext _context;

        public ReporteService(IReporteRepository reporteRepository, AppDbContext context)
        {
            _reporteRepository = reporteRepository;
            _context = context;
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
            string tipo = NormalizarTipo(request.TipoReporte);

            string nombreArchivo = tipo switch
            {
                "RENDIMIENTO POR ASESOR" => $"reporte_rendimiento_asesor_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                "RESUMEN DE CLIENTES" => $"reporte_resumen_clientes_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                "REPORTE DE DEUDAS" => $"reporte_deudas_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                "REPORTE DE PAGOS" => $"reporte_pagos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                "REPORTE DE MOROSIDAD" => $"reporte_morosidad_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                "REPORTE DE ALERTAS" => $"reporte_alertas_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                _ => $"reporte_general_admin_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            string urlDescarga = tipo switch
            {
                "RENDIMIENTO POR ASESOR" => CrearExcelRendimientoAsesores(nombreArchivo, baseUrl, request.FechaDesde, request.FechaHasta),
                "RESUMEN DE CLIENTES" => CrearExcelResumenClientes(nombreArchivo, baseUrl, request.FechaDesde, request.FechaHasta),
                "REPORTE DE DEUDAS" => CrearExcelDeudas(nombreArchivo, baseUrl, request.FechaDesde, request.FechaHasta),
                "REPORTE DE PAGOS" => CrearExcelPagos(nombreArchivo, baseUrl, request.FechaDesde, request.FechaHasta),
                "REPORTE DE MOROSIDAD" => CrearExcelMorosidad(nombreArchivo, baseUrl, request.FechaDesde, request.FechaHasta),
                "REPORTE DE ALERTAS" => CrearExcelAlertas(nombreArchivo, baseUrl, request.FechaDesde, request.FechaHasta),
                _ => CrearExcelAdmin(nombreArchivo, baseUrl, request.FechaDesde, request.FechaHasta)
            };

            var fecha = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

            var reporte = new ReporteGenerado
            {
                IdUsuario = usuario.IdUsuario,
                NombreReporte = request.TipoReporte,
                TipoReporte = request.TipoReporte,
                FechaDesde = request.FechaDesde.HasValue ? DateOnly.FromDateTime(request.FechaDesde.Value) : null,
                FechaHasta = request.FechaHasta.HasValue ? DateOnly.FromDateTime(request.FechaHasta.Value) : null,
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
            string tipo = NormalizarTipo(request.TipoReporte);

            string nombreArchivo = tipo switch
            {
                "MI CARTERA" => $"reporte_mi_cartera_asesor_{idAsesor}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                "MIS CLIENTES" => $"reporte_mis_clientes_asesor_{idAsesor}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                "MIS DEUDAS" => $"reporte_mis_deudas_asesor_{idAsesor}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                "MIS PAGOS RECUPERADOS" => $"reporte_mis_pagos_recuperados_asesor_{idAsesor}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                "MIS GESTIONES" => $"reporte_mis_gestiones_asesor_{idAsesor}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                "MIS ALERTAS" => $"reporte_mis_alertas_asesor_{idAsesor}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                _ => $"reporte_general_asesor_{idAsesor}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            string urlDescarga = tipo switch
            {
                "MI CARTERA" => CrearExcelResumenCartera(nombreArchivo, baseUrl, idAsesor, request.FechaDesde, request.FechaHasta),
                "MIS CLIENTES" => CrearExcelMisClientesAsesor(nombreArchivo, baseUrl, idAsesor, request.FechaDesde, request.FechaHasta),
                "MIS DEUDAS" => CrearExcelDeudasAsesor(nombreArchivo, baseUrl, idAsesor, request.FechaDesde, request.FechaHasta),
                "MIS PAGOS RECUPERADOS" => CrearExcelPagosAsesor(nombreArchivo, baseUrl, idAsesor, request.FechaDesde, request.FechaHasta),
                "MIS GESTIONES" => CrearExcelGestionesAsesor(nombreArchivo, baseUrl, idAsesor, request.FechaDesde, request.FechaHasta),
                "MIS ALERTAS" => CrearExcelAlertasAsesor(nombreArchivo, baseUrl, idAsesor, request.FechaDesde, request.FechaHasta),
                _ => CrearExcelAsesor(nombreArchivo, data, baseUrl, request.FechaDesde, request.FechaHasta)
            };

            var fecha = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

            var reporte = new ReporteGenerado
            {
                IdUsuario = asesor.IdUsuario,
                NombreReporte = request.TipoReporte,
                TipoReporte = request.TipoReporte,
                FechaDesde = request.FechaDesde.HasValue ? DateOnly.FromDateTime(request.FechaDesde.Value) : null,
                FechaHasta = request.FechaHasta.HasValue ? DateOnly.FromDateTime(request.FechaHasta.Value) : null,
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

        private static string NormalizarTipo(string tipo)
        {
            return (tipo ?? "")
                .Trim()
                .ToUpper()
                .Replace("Á", "A")
                .Replace("É", "E")
                .Replace("Í", "I")
                .Replace("Ó", "O")
                .Replace("Ú", "U");
        }

        private static DateOnly? ToDateOnly(DateTime? fecha)
        {
            return fecha.HasValue ? DateOnly.FromDateTime(fecha.Value) : null;
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

        private static string GuardarExcel(XLWorkbook workbook, string nombreArchivo, string baseUrl)
        {
            string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reportes");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            string rutaFisica = Path.Combine(carpeta, nombreArchivo);
            workbook.SaveAs(rutaFisica);

            return $"{baseUrl}/reportes/{nombreArchivo}";
        }

        private static void DarFormatoTabla(
            IXLWorksheet ws,
            string titulo,
            int columnas,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null)
        {
            ws.Cell(1, 1).Value = titulo;
            ws.Range(1, 1, 1, columnas).Merge();

            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 16;
            ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            string periodo;

            if (fechaDesde.HasValue && fechaHasta.HasValue)
                periodo = $"Periodo: Desde {fechaDesde.Value:dd/MM/yyyy} hasta {fechaHasta.Value:dd/MM/yyyy}";
            else
                periodo = $"Periodo: {DateTime.Now:dd/MM/yyyy}";

            ws.Cell(2, 1).Value = periodo;
            ws.Range(2, 1, 2, columnas).Merge();
            ws.Cell(2, 1).Style.Font.Italic = true;
            ws.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            var encabezado = ws.Range(4, 1, 4, columnas);
            encabezado.Style.Font.Bold = true;
            encabezado.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E3A5F");
            encabezado.Style.Font.FontColor = XLColor.White;
            encabezado.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.SheetView.FreezeRows(4);
        }

        private static void AplicarBordes(IXLWorksheet ws)
        {
            var rango = ws.RangeUsed();

            if (rango != null)
            {
                rango.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                rango.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                ws.Columns().AdjustToContents();
            }
        }

        private string CrearExcelAdmin(string nombreArchivo, string baseUrl, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Reporte General");

            DarFormatoTabla(ws, "REPORTE GENERAL DE COBRANZA", 10, fechaDesde, fechaHasta);

            ws.Cell(4, 1).Value = "Cliente";
            ws.Cell(4, 2).Value = "DNI";
            ws.Cell(4, 3).Value = "Correo";
            ws.Cell(4, 4).Value = "Teléfono";
            ws.Cell(4, 5).Value = "Asesor";
            ws.Cell(4, 6).Value = "Deuda Total";
            ws.Cell(4, 7).Value = "Monto Pagado";
            ws.Cell(4, 8).Value = "Saldo Pendiente";
            ws.Cell(4, 9).Value = "Días Atraso";
            ws.Cell(4, 10).Value = "Estado Cliente";

            var query = _context.Clientes
                .Where(c => c.Activo && !c.Eliminado);

            if (fechaDesde.HasValue)
                query = query.Where(c => c.FechaRegistro.Date >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                query = query.Where(c => c.FechaRegistro.Date <= fechaHasta.Value.Date);

            var clientes = query
                .Select(c => new
                {
                    Cliente = c.Nombres + " " + c.Apellidos,
                    c.Dni,
                    c.Correo,
                    c.Telefono,
                    Asesor = c.IdAsesorNavigation != null
                        ? c.IdAsesorNavigation.Nombres + " " + c.IdAsesorNavigation.Apellidos
                        : "-",
                    DeudaTotal = c.Deuda.Where(d => d.Activo && !d.Eliminado).Sum(d => d.MontoTotal),
                    MontoPagado = c.Deuda.Where(d => d.Activo && !d.Eliminado).Sum(d => d.MontoPagado),
                    SaldoPendiente = c.Deuda.Where(d => d.Activo && !d.Eliminado).Sum(d => d.SaldoPendiente),
                    DiasAtraso = c.Deuda.Where(d => d.Activo && !d.Eliminado).Any()
                        ? c.Deuda.Where(d => d.Activo && !d.Eliminado).Max(d => d.DiasAtraso)
                        : 0,
                    c.EstadoCliente
                })
                .ToList();

            int fila = 5;

            foreach (var c in clientes)
            {
                ws.Cell(fila, 1).Value = c.Cliente;
                ws.Cell(fila, 2).Value = c.Dni;
                ws.Cell(fila, 3).Value = c.Correo ?? "-";
                ws.Cell(fila, 4).Value = c.Telefono ?? "-";
                ws.Cell(fila, 5).Value = c.Asesor;
                ws.Cell(fila, 6).Value = c.DeudaTotal;
                ws.Cell(fila, 7).Value = c.MontoPagado;
                ws.Cell(fila, 8).Value = c.SaldoPendiente;
                ws.Cell(fila, 9).Value = c.DiasAtraso;
                ws.Cell(fila, 10).Value = c.EstadoCliente;
                fila++;
            }

            if (fila > 5)
                ws.Range(5, 6, fila - 1, 8).Style.NumberFormat.Format = "\"S/.\" #,##0.00";

            AplicarBordes(ws);

            CrearHojaRendimientoAsesores(workbook, fechaDesde, fechaHasta);
            CrearHojaResumenClientes(workbook, fechaDesde, fechaHasta);

            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private string CrearExcelAsesor(
            string nombreArchivo,
            ReporteRendimientoIndividualDto data,
            string baseUrl,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
        {
            using var workbook = new XLWorkbook();

            CrearHojaResumenCartera(workbook, data, fechaDesde, fechaHasta);
            CrearHojaDistribucionClientes(workbook, data, fechaDesde, fechaHasta);

            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private string CrearExcelRendimientoAsesores(
            string nombreArchivo,
            string baseUrl,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
        {
            using var workbook = new XLWorkbook();

            CrearHojaRendimientoAsesores(workbook, fechaDesde, fechaHasta);

            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private string CrearExcelResumenClientes(
            string nombreArchivo,
            string baseUrl,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
        {
            using var workbook = new XLWorkbook();

            CrearHojaResumenClientes(workbook, fechaDesde, fechaHasta);

            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private string CrearExcelResumenCartera(
            string nombreArchivo,
            string baseUrl,
            int idAsesor,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Mi Cartera");

            DarFormatoTabla(ws, "REPORTE DE MI CARTERA", 4, fechaDesde, fechaHasta);

            ws.Cell(4, 1).Value = "Total Clientes";
            ws.Cell(4, 2).Value = "Total Deuda Asignada";
            ws.Cell(4, 3).Value = "Pagos Recuperados";
            ws.Cell(4, 4).Value = "Eficiencia";

            var clientesQuery = _context.Clientes
                .Where(c => c.IdAsesor == idAsesor && c.Activo && !c.Eliminado);

            if (fechaDesde.HasValue)
                clientesQuery = clientesQuery.Where(c => c.FechaRegistro.Date >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                clientesQuery = clientesQuery.Where(c => c.FechaRegistro.Date <= fechaHasta.Value.Date);

            var clientesIds = clientesQuery.Select(c => c.IdCliente).ToList();

            var deudasQuery = _context.Deuda
                .Where(d => clientesIds.Contains(d.IdCliente) && d.Activo && !d.Eliminado);

            var pagosQuery = _context.Pagos
                .Where(p => p.Activo && !p.Eliminado &&
                            clientesIds.Contains(p.IdDeudaNavigation.IdCliente));

            if (fechaDesde.HasValue)
            {
                var desde = DateOnly.FromDateTime(fechaDesde.Value);
                deudasQuery = deudasQuery.Where(d => d.FechaEmision >= desde);
                pagosQuery = pagosQuery.Where(p => p.FechaPago >= desde);
            }

            if (fechaHasta.HasValue)
            {
                var hasta = DateOnly.FromDateTime(fechaHasta.Value);
                deudasQuery = deudasQuery.Where(d => d.FechaEmision <= hasta);
                pagosQuery = pagosQuery.Where(p => p.FechaPago <= hasta);
            }

            decimal deudaTotal = deudasQuery.Sum(d => d.MontoTotal);
            decimal pagos = pagosQuery.Sum(p => p.Monto);
            string eficiencia = deudaTotal > 0 ? $"{Math.Round((pagos / deudaTotal) * 100, 1)}%" : "0%";

            ws.Cell(5, 1).Value = clientesIds.Count;
            ws.Cell(5, 2).Value = deudaTotal;
            ws.Cell(5, 3).Value = pagos;
            ws.Cell(5, 4).Value = eficiencia;

            ws.Range("B5:C5").Style.NumberFormat.Format = "\"S/.\" #,##0.00";

            AplicarBordes(ws);
            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private static void CrearHojaResumenCartera(
            XLWorkbook workbook,
            ReporteRendimientoIndividualDto data,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
        {
            var ws = workbook.Worksheets.Add("Resumen Cartera");

            DarFormatoTabla(ws, "REPORTE DE RESUMEN DE CARTERA", 4, fechaDesde, fechaHasta);

            ws.Cell(4, 1).Value = "Total Clientes";
            ws.Cell(4, 2).Value = "Total Deuda Asignada";
            ws.Cell(4, 3).Value = "Pagos Recuperados";
            ws.Cell(4, 4).Value = "Eficiencia";

            ws.Cell(5, 1).Value = data.ResumenCartera.TotalClientesAsignados;
            ws.Cell(5, 2).Value = data.ResumenCartera.TotalDeudaAsignada;
            ws.Cell(5, 3).Value = data.ResumenCartera.TotalPagosRecuperados;
            ws.Cell(5, 4).Value = data.ResumenCartera.EficienciaIndividual;

            ws.Range("B5:C5").Style.NumberFormat.Format = "\"S/.\" #,##0.00";

            AplicarBordes(ws);
        }

        private static void CrearHojaDistribucionClientes(
            XLWorkbook workbook,
            ReporteRendimientoIndividualDto data,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
        {
            var ws = workbook.Worksheets.Add("Distribución Clientes");

            DarFormatoTabla(ws, "REPORTE DE DISTRIBUCIÓN DE CLIENTES", 4, fechaDesde, fechaHasta);

            ws.Cell(4, 1).Value = "Estado";
            ws.Cell(4, 2).Value = "Cantidad";
            ws.Cell(4, 3).Value = "Deuda Total";
            ws.Cell(4, 4).Value = "Porcentaje";

            int fila = 5;

            foreach (var item in data.DistribucionClientes)
            {
                ws.Cell(fila, 1).Value = item.Estado;
                ws.Cell(fila, 2).Value = item.Cantidad;
                ws.Cell(fila, 3).Value = item.DeudaTotal;
                ws.Cell(fila, 4).Value = item.Porcentaje / 100;
                fila++;
            }

            if (fila > 5)
            {
                ws.Range(5, 3, fila - 1, 3).Style.NumberFormat.Format = "\"S/.\" #,##0.00";
                ws.Range(5, 4, fila - 1, 4).Style.NumberFormat.Format = "0.0%";
            }

            AplicarBordes(ws);
        }

        private void CrearHojaRendimientoAsesores(
            XLWorkbook workbook,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
        {
            var ws = workbook.Worksheets.Add("Rendimiento Asesores");

            DarFormatoTabla(ws, "REPORTE DE RENDIMIENTO POR ASESOR", 5, fechaDesde, fechaHasta);

            ws.Cell(4, 1).Value = "Asesor";
            ws.Cell(4, 2).Value = "Clientes";
            ws.Cell(4, 3).Value = "Deuda Gestionada";
            ws.Cell(4, 4).Value = "Pagos Recuperados";
            ws.Cell(4, 5).Value = "Eficiencia";

            var asesores = _context.Usuarios
                .Where(u => u.Activo && !u.Eliminado && u.IdRol != 1)
                .Select(u => new
                {
                    u.IdUsuario,
                    Asesor = u.Nombres + " " + u.Apellidos
                })
                .ToList();

            int fila = 5;

            foreach (var asesor in asesores)
            {
                var clientesQuery = _context.Clientes
                    .Where(c => c.IdAsesor == asesor.IdUsuario && c.Activo && !c.Eliminado);

                if (fechaDesde.HasValue)
                    clientesQuery = clientesQuery.Where(c => c.FechaRegistro.Date >= fechaDesde.Value.Date);

                if (fechaHasta.HasValue)
                    clientesQuery = clientesQuery.Where(c => c.FechaRegistro.Date <= fechaHasta.Value.Date);

                var clientesIds = clientesQuery.Select(c => c.IdCliente).ToList();

                var deudasQuery = _context.Deuda
                    .Where(d => clientesIds.Contains(d.IdCliente) && d.Activo && !d.Eliminado);

                var pagosQuery = _context.Pagos
                    .Where(p => p.Activo && !p.Eliminado &&
                                clientesIds.Contains(p.IdDeudaNavigation.IdCliente));

                if (fechaDesde.HasValue)
                {
                    var desde = DateOnly.FromDateTime(fechaDesde.Value);
                    deudasQuery = deudasQuery.Where(d => d.FechaEmision >= desde);
                    pagosQuery = pagosQuery.Where(p => p.FechaPago >= desde);
                }

                if (fechaHasta.HasValue)
                {
                    var hasta = DateOnly.FromDateTime(fechaHasta.Value);
                    deudasQuery = deudasQuery.Where(d => d.FechaEmision <= hasta);
                    pagosQuery = pagosQuery.Where(p => p.FechaPago <= hasta);
                }

                decimal deudaTotal = deudasQuery.Sum(d => d.MontoTotal);
                decimal pagos = pagosQuery.Sum(p => p.Monto);
                string eficiencia = deudaTotal > 0 ? $"{Math.Round((pagos / deudaTotal) * 100, 1)}%" : "0%";

                ws.Cell(fila, 1).Value = asesor.Asesor;
                ws.Cell(fila, 2).Value = clientesIds.Count;
                ws.Cell(fila, 3).Value = deudaTotal;
                ws.Cell(fila, 4).Value = pagos;
                ws.Cell(fila, 5).Value = eficiencia;
                fila++;
            }

            if (fila > 5)
                ws.Range(5, 3, fila - 1, 4).Style.NumberFormat.Format = "\"S/.\" #,##0.00";

            AplicarBordes(ws);
        }

        private void CrearHojaResumenClientes(
            XLWorkbook workbook,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
        {
            var ws = workbook.Worksheets.Add("Resumen Clientes");

            DarFormatoTabla(ws, "REPORTE DE RESUMEN DE CLIENTES", 4, fechaDesde, fechaHasta);

            ws.Cell(4, 1).Value = "Estado";
            ws.Cell(4, 2).Value = "Cantidad";
            ws.Cell(4, 3).Value = "Deuda Total";
            ws.Cell(4, 4).Value = "Porcentaje";

            var clientesQuery = _context.Clientes
                .Where(c => c.Activo && !c.Eliminado);

            if (fechaDesde.HasValue)
                clientesQuery = clientesQuery.Where(c => c.FechaRegistro.Date >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                clientesQuery = clientesQuery.Where(c => c.FechaRegistro.Date <= fechaHasta.Value.Date);

            var clientes = clientesQuery
                .Select(c => new
                {
                    c.IdCliente,
                    c.EstadoCliente
                })
                .ToList();

            int total = clientes.Count;
            int fila = 5;

            var grupos = clientes
                .GroupBy(c => c.EstadoCliente)
                .Select(g => new
                {
                    Estado = g.Key,
                    Cantidad = g.Count(),
                    Ids = g.Select(x => x.IdCliente).ToList()
                })
                .ToList();

            foreach (var g in grupos)
            {
                decimal deudaTotal = _context.Deuda
                    .Where(d => g.Ids.Contains(d.IdCliente) && d.Activo && !d.Eliminado)
                    .Sum(d => d.MontoTotal);

                double porcentaje = total > 0 ? Math.Round((double)g.Cantidad / total, 3) : 0;

                ws.Cell(fila, 1).Value = g.Estado;
                ws.Cell(fila, 2).Value = g.Cantidad;
                ws.Cell(fila, 3).Value = deudaTotal;
                ws.Cell(fila, 4).Value = porcentaje;
                fila++;
            }

            if (fila > 5)
            {
                ws.Range(5, 3, fila - 1, 3).Style.NumberFormat.Format = "\"S/.\" #,##0.00";
                ws.Range(5, 4, fila - 1, 4).Style.NumberFormat.Format = "0.0%";
            }

            AplicarBordes(ws);
        }

        private string CrearExcelDeudas(string nombreArchivo, string baseUrl, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Deudas");

            DarFormatoTabla(ws, "REPORTE DE DEUDAS", 8, fechaDesde, fechaHasta);

            ws.Cell(4, 1).Value = "Cliente";
            ws.Cell(4, 2).Value = "Asesor";
            ws.Cell(4, 3).Value = "Monto Total";
            ws.Cell(4, 4).Value = "Monto Pagado";
            ws.Cell(4, 5).Value = "Saldo Pendiente";
            ws.Cell(4, 6).Value = "Fecha Vencimiento";
            ws.Cell(4, 7).Value = "Días Atraso";
            ws.Cell(4, 8).Value = "Estado Deuda";

            var query = _context.Deuda
                .Where(d => d.Activo && !d.Eliminado);

            var desde = ToDateOnly(fechaDesde);
            var hasta = ToDateOnly(fechaHasta);

            if (desde.HasValue)
                query = query.Where(d => d.FechaVencimiento >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(d => d.FechaVencimiento <= hasta.Value);

            var deudas = query
                .Select(d => new
                {
                    Cliente = d.IdClienteNavigation.Nombres + " " + d.IdClienteNavigation.Apellidos,
                    Asesor = d.IdClienteNavigation.IdAsesorNavigation != null
                        ? d.IdClienteNavigation.IdAsesorNavigation.Nombres + " " + d.IdClienteNavigation.IdAsesorNavigation.Apellidos
                        : "-",
                    d.MontoTotal,
                    d.MontoPagado,
                    d.SaldoPendiente,
                    d.FechaVencimiento,
                    d.DiasAtraso,
                    d.EstadoDeuda
                })
                .ToList();

            int fila = 5;

            foreach (var d in deudas)
            {
                ws.Cell(fila, 1).Value = d.Cliente;
                ws.Cell(fila, 2).Value = d.Asesor;
                ws.Cell(fila, 3).Value = d.MontoTotal;
                ws.Cell(fila, 4).Value = d.MontoPagado;
                ws.Cell(fila, 5).Value = d.SaldoPendiente;
                ws.Cell(fila, 6).Value = d.FechaVencimiento.ToString("dd/MM/yyyy");
                ws.Cell(fila, 7).Value = d.DiasAtraso;
                ws.Cell(fila, 8).Value = d.EstadoDeuda;
                fila++;
            }

            if (fila > 5)
                ws.Range(5, 3, fila - 1, 5).Style.NumberFormat.Format = "\"S/.\" #,##0.00";

            AplicarBordes(ws);
            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private string CrearExcelPagos(string nombreArchivo, string baseUrl, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Pagos");

            DarFormatoTabla(ws, "REPORTE DE PAGOS", 7, fechaDesde, fechaHasta);

            ws.Cell(4, 1).Value = "Cliente";
            ws.Cell(4, 2).Value = "Asesor";
            ws.Cell(4, 3).Value = "Monto";
            ws.Cell(4, 4).Value = "Fecha Pago";
            ws.Cell(4, 5).Value = "Método Pago";
            ws.Cell(4, 6).Value = "Estado Pago";
            ws.Cell(4, 7).Value = "Nota";

            var query = _context.Pagos
                .Where(p => p.Activo && !p.Eliminado);

            var desde = ToDateOnly(fechaDesde);
            var hasta = ToDateOnly(fechaHasta);

            if (desde.HasValue)
                query = query.Where(p => p.FechaPago >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(p => p.FechaPago <= hasta.Value);

            var pagos = query
                .Select(p => new
                {
                    Cliente = p.IdDeudaNavigation.IdClienteNavigation.Nombres + " " + p.IdDeudaNavigation.IdClienteNavigation.Apellidos,
                    Asesor = p.IdDeudaNavigation.IdClienteNavigation.IdAsesorNavigation != null
                        ? p.IdDeudaNavigation.IdClienteNavigation.IdAsesorNavigation.Nombres + " " + p.IdDeudaNavigation.IdClienteNavigation.IdAsesorNavigation.Apellidos
                        : "-",
                    p.Monto,
                    p.FechaPago,
                    p.MetodoPago,
                    p.EstadoPago,
                    p.Nota
                })
                .ToList();

            int fila = 5;

            foreach (var p in pagos)
            {
                ws.Cell(fila, 1).Value = p.Cliente;
                ws.Cell(fila, 2).Value = p.Asesor;
                ws.Cell(fila, 3).Value = p.Monto;
                ws.Cell(fila, 4).Value = p.FechaPago.ToString("dd/MM/yyyy");
                ws.Cell(fila, 5).Value = p.MetodoPago;
                ws.Cell(fila, 6).Value = p.EstadoPago;
                ws.Cell(fila, 7).Value = p.Nota ?? "-";
                fila++;
            }

            if (fila > 5)
                ws.Range(5, 3, fila - 1, 3).Style.NumberFormat.Format = "\"S/.\" #,##0.00";

            AplicarBordes(ws);
            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private string CrearExcelMorosidad(string nombreArchivo, string baseUrl, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Morosidad");

            DarFormatoTabla(ws, "REPORTE DE MOROSIDAD", 8, fechaDesde, fechaHasta);

            ws.Cell(4, 1).Value = "Cliente";
            ws.Cell(4, 2).Value = "DNI";
            ws.Cell(4, 3).Value = "Asesor";
            ws.Cell(4, 4).Value = "Saldo Pendiente";
            ws.Cell(4, 5).Value = "Días Atraso";
            ws.Cell(4, 6).Value = "Fecha Vencimiento";
            ws.Cell(4, 7).Value = "Riesgo";
            ws.Cell(4, 8).Value = "Estado Cliente";

            var query = _context.Deuda
                .Where(d => d.Activo && !d.Eliminado && d.SaldoPendiente > 0 && d.DiasAtraso > 0);

            var desde = ToDateOnly(fechaDesde);
            var hasta = ToDateOnly(fechaHasta);

            if (desde.HasValue)
                query = query.Where(d => d.FechaVencimiento >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(d => d.FechaVencimiento <= hasta.Value);

            var morosos = query
                .Select(d => new
                {
                    Cliente = d.IdClienteNavigation.Nombres + " " + d.IdClienteNavigation.Apellidos,
                    d.IdClienteNavigation.Dni,
                    Asesor = d.IdClienteNavigation.IdAsesorNavigation != null
                        ? d.IdClienteNavigation.IdAsesorNavigation.Nombres + " " + d.IdClienteNavigation.IdAsesorNavigation.Apellidos
                        : "-",
                    d.SaldoPendiente,
                    d.DiasAtraso,
                    d.FechaVencimiento,
                    d.IdClienteNavigation.Riesgo,
                    d.IdClienteNavigation.EstadoCliente
                })
                .ToList();

            int fila = 5;

            foreach (var m in morosos)
            {
                ws.Cell(fila, 1).Value = m.Cliente;
                ws.Cell(fila, 2).Value = m.Dni;
                ws.Cell(fila, 3).Value = m.Asesor;
                ws.Cell(fila, 4).Value = m.SaldoPendiente;
                ws.Cell(fila, 5).Value = m.DiasAtraso;
                ws.Cell(fila, 6).Value = m.FechaVencimiento.ToString("dd/MM/yyyy");
                ws.Cell(fila, 7).Value = m.Riesgo;
                ws.Cell(fila, 8).Value = m.EstadoCliente;
                fila++;
            }

            if (fila > 5)
                ws.Range(5, 4, fila - 1, 4).Style.NumberFormat.Format = "\"S/.\" #,##0.00";

            AplicarBordes(ws);
            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private string CrearExcelAlertas(string nombreArchivo, string baseUrl, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Alertas");

            DarFormatoTabla(ws, "REPORTE DE ALERTAS", 7, fechaDesde, fechaHasta);

            ws.Cell(4, 1).Value = "Cliente";
            ws.Cell(4, 2).Value = "Deuda";
            ws.Cell(4, 3).Value = "Tipo Alerta";
            ws.Cell(4, 4).Value = "Prioridad";
            ws.Cell(4, 5).Value = "Mensaje";
            ws.Cell(4, 6).Value = "Fecha Alerta";
            ws.Cell(4, 7).Value = "Leído";

            var query = _context.Alerta
                .Where(a => a.Activo && !a.Eliminado);

            if (fechaDesde.HasValue)
                query = query.Where(a => a.FechaAlerta.Date >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                query = query.Where(a => a.FechaAlerta.Date <= fechaHasta.Value.Date);

            var alertas = query
                .Select(a => new
                {
                    Cliente = a.IdClienteNavigation != null
                        ? a.IdClienteNavigation.Nombres + " " + a.IdClienteNavigation.Apellidos
                        : "-",
                    Deuda = a.IdDeudaNavigation != null
                        ? a.IdDeudaNavigation.SaldoPendiente
                        : 0,
                    a.TipoAlerta,
                    a.Prioridad,
                    a.Mensaje,
                    a.FechaAlerta,
                    a.Leido
                })
                .ToList();

            int fila = 5;

            foreach (var a in alertas)
            {
                ws.Cell(fila, 1).Value = a.Cliente;
                ws.Cell(fila, 2).Value = a.Deuda;
                ws.Cell(fila, 3).Value = a.TipoAlerta;
                ws.Cell(fila, 4).Value = a.Prioridad;
                ws.Cell(fila, 5).Value = a.Mensaje;
                ws.Cell(fila, 6).Value = a.FechaAlerta.ToString("dd/MM/yyyy HH:mm");
                ws.Cell(fila, 7).Value = a.Leido ? "Sí" : "No";
                fila++;
            }

            if (fila > 5)
                ws.Range(5, 2, fila - 1, 2).Style.NumberFormat.Format = "\"S/.\" #,##0.00";

            AplicarBordes(ws);
            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private string CrearExcelMisClientesAsesor(
            string nombreArchivo,
            string baseUrl,
            int idAsesor,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Mis Clientes");

            DarFormatoTabla(ws, "REPORTE DE MIS CLIENTES", 8, fechaDesde, fechaHasta);

            ws.Cell(4, 1).Value = "Cliente";
            ws.Cell(4, 2).Value = "DNI";
            ws.Cell(4, 3).Value = "Correo";
            ws.Cell(4, 4).Value = "Teléfono";
            ws.Cell(4, 5).Value = "Deuda Total";
            ws.Cell(4, 6).Value = "Saldo Pendiente";
            ws.Cell(4, 7).Value = "Riesgo";
            ws.Cell(4, 8).Value = "Estado";

            var query = _context.Clientes
                .Where(c => c.IdAsesor == idAsesor && c.Activo && !c.Eliminado);

            if (fechaDesde.HasValue)
                query = query.Where(c => c.FechaRegistro.Date >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                query = query.Where(c => c.FechaRegistro.Date <= fechaHasta.Value.Date);

            var clientes = query
                .Select(c => new
                {
                    Cliente = c.Nombres + " " + c.Apellidos,
                    c.Dni,
                    c.Correo,
                    c.Telefono,
                    DeudaTotal = c.Deuda.Where(d => d.Activo && !d.Eliminado).Sum(d => d.MontoTotal),
                    SaldoPendiente = c.Deuda.Where(d => d.Activo && !d.Eliminado).Sum(d => d.SaldoPendiente),
                    c.Riesgo,
                    c.EstadoCliente
                })
                .ToList();

            int fila = 5;

            foreach (var c in clientes)
            {
                ws.Cell(fila, 1).Value = c.Cliente;
                ws.Cell(fila, 2).Value = c.Dni;
                ws.Cell(fila, 3).Value = c.Correo ?? "-";
                ws.Cell(fila, 4).Value = c.Telefono ?? "-";
                ws.Cell(fila, 5).Value = c.DeudaTotal;
                ws.Cell(fila, 6).Value = c.SaldoPendiente;
                ws.Cell(fila, 7).Value = c.Riesgo;
                ws.Cell(fila, 8).Value = c.EstadoCliente;
                fila++;
            }

            if (fila > 5)
                ws.Range(5, 5, fila - 1, 6).Style.NumberFormat.Format = "\"S/.\" #,##0.00";

            AplicarBordes(ws);
            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private string CrearExcelDeudasAsesor(string nombreArchivo, string baseUrl, int idAsesor, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Mis Deudas");

            DarFormatoTabla(ws, "REPORTE DE MIS DEUDAS", 7, fechaDesde, fechaHasta);

            ws.Cell(4, 1).Value = "Cliente";
            ws.Cell(4, 2).Value = "DNI";
            ws.Cell(4, 3).Value = "Monto Total";
            ws.Cell(4, 4).Value = "Monto Pagado";
            ws.Cell(4, 5).Value = "Saldo Pendiente";
            ws.Cell(4, 6).Value = "Días Atraso";
            ws.Cell(4, 7).Value = "Estado Deuda";

            var query = _context.Deuda
                .Where(d => d.Activo && !d.Eliminado && d.IdClienteNavigation.IdAsesor == idAsesor);

            var desde = ToDateOnly(fechaDesde);
            var hasta = ToDateOnly(fechaHasta);

            if (desde.HasValue)
                query = query.Where(d => d.FechaVencimiento >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(d => d.FechaVencimiento <= hasta.Value);

            var deudas = query
                .Select(d => new
                {
                    Cliente = d.IdClienteNavigation.Nombres + " " + d.IdClienteNavigation.Apellidos,
                    d.IdClienteNavigation.Dni,
                    d.MontoTotal,
                    d.MontoPagado,
                    d.SaldoPendiente,
                    d.DiasAtraso,
                    d.EstadoDeuda
                })
                .ToList();

            int fila = 5;

            foreach (var d in deudas)
            {
                ws.Cell(fila, 1).Value = d.Cliente;
                ws.Cell(fila, 2).Value = d.Dni;
                ws.Cell(fila, 3).Value = d.MontoTotal;
                ws.Cell(fila, 4).Value = d.MontoPagado;
                ws.Cell(fila, 5).Value = d.SaldoPendiente;
                ws.Cell(fila, 6).Value = d.DiasAtraso;
                ws.Cell(fila, 7).Value = d.EstadoDeuda;
                fila++;
            }

            if (fila > 5)
                ws.Range(5, 3, fila - 1, 5).Style.NumberFormat.Format = "\"S/.\" #,##0.00";

            AplicarBordes(ws);
            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private string CrearExcelPagosAsesor(string nombreArchivo, string baseUrl, int idAsesor, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Mis Pagos");

            DarFormatoTabla(ws, "REPORTE DE MIS PAGOS RECUPERADOS", 7, fechaDesde, fechaHasta);

            ws.Cell(4, 1).Value = "Cliente";
            ws.Cell(4, 2).Value = "DNI";
            ws.Cell(4, 3).Value = "Monto";
            ws.Cell(4, 4).Value = "Fecha Pago";
            ws.Cell(4, 5).Value = "Método Pago";
            ws.Cell(4, 6).Value = "Estado Pago";
            ws.Cell(4, 7).Value = "Nota";

            var query = _context.Pagos
                .Where(p => p.Activo && !p.Eliminado &&
                            p.IdDeudaNavigation.IdClienteNavigation.IdAsesor == idAsesor);

            var desde = ToDateOnly(fechaDesde);
            var hasta = ToDateOnly(fechaHasta);

            if (desde.HasValue)
                query = query.Where(p => p.FechaPago >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(p => p.FechaPago <= hasta.Value);

            var pagos = query
                .Select(p => new
                {
                    Cliente = p.IdDeudaNavigation.IdClienteNavigation.Nombres + " " +
                              p.IdDeudaNavigation.IdClienteNavigation.Apellidos,
                    p.IdDeudaNavigation.IdClienteNavigation.Dni,
                    p.Monto,
                    p.FechaPago,
                    p.MetodoPago,
                    p.EstadoPago,
                    p.Nota
                })
                .ToList();

            int fila = 5;

            foreach (var p in pagos)
            {
                ws.Cell(fila, 1).Value = p.Cliente;
                ws.Cell(fila, 2).Value = p.Dni;
                ws.Cell(fila, 3).Value = p.Monto;
                ws.Cell(fila, 4).Value = p.FechaPago.ToString("dd/MM/yyyy");
                ws.Cell(fila, 5).Value = p.MetodoPago;
                ws.Cell(fila, 6).Value = p.EstadoPago;
                ws.Cell(fila, 7).Value = p.Nota ?? "-";
                fila++;
            }

            if (fila > 5)
                ws.Range(5, 3, fila - 1, 3).Style.NumberFormat.Format = "\"S/.\" #,##0.00";

            AplicarBordes(ws);
            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private string CrearExcelGestionesAsesor(string nombreArchivo, string baseUrl, int idAsesor, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Mis Gestiones");

            DarFormatoTabla(ws, "REPORTE DE MIS GESTIONES", 7, fechaDesde, fechaHasta);

            ws.Cell(4, 1).Value = "Cliente";
            ws.Cell(4, 2).Value = "DNI";
            ws.Cell(4, 3).Value = "Tipo Gestión";
            ws.Cell(4, 4).Value = "Descripción";
            ws.Cell(4, 5).Value = "Resultado";
            ws.Cell(4, 6).Value = "Fecha Gestión";
            ws.Cell(4, 7).Value = "Próxima Acción";

            var query = _context.GestionCobranzas
                .Where(g => g.Activo && !g.Eliminado && g.IdClienteNavigation.IdAsesor == idAsesor);

            if (fechaDesde.HasValue)
                query = query.Where(g => g.FechaGestion.Date >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                query = query.Where(g => g.FechaGestion.Date <= fechaHasta.Value.Date);

            var gestiones = query
                .Select(g => new
                {
                    Cliente = g.IdClienteNavigation.Nombres + " " + g.IdClienteNavigation.Apellidos,
                    g.IdClienteNavigation.Dni,
                    g.TipoGestion,
                    g.Descripcion,
                    g.Resultado,
                    g.FechaGestion,
                    g.ProximaAccion
                })
                .ToList();

            int fila = 5;

            foreach (var g in gestiones)
            {
                ws.Cell(fila, 1).Value = g.Cliente;
                ws.Cell(fila, 2).Value = g.Dni;
                ws.Cell(fila, 3).Value = g.TipoGestion;
                ws.Cell(fila, 4).Value = g.Descripcion;
                ws.Cell(fila, 5).Value = g.Resultado ?? "-";
                ws.Cell(fila, 6).Value = g.FechaGestion.ToString("dd/MM/yyyy HH:mm");
                ws.Cell(fila, 7).Value = g.ProximaAccion.HasValue
                    ? g.ProximaAccion.Value.ToString("dd/MM/yyyy")
                    : "-";
                fila++;
            }

            AplicarBordes(ws);
            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private string CrearExcelAlertasAsesor(string nombreArchivo, string baseUrl, int idAsesor, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Mis Alertas");

            DarFormatoTabla(ws, "REPORTE DE MIS ALERTAS", 6, fechaDesde, fechaHasta);

            ws.Cell(4, 1).Value = "Cliente";
            ws.Cell(4, 2).Value = "Tipo Alerta";
            ws.Cell(4, 3).Value = "Prioridad";
            ws.Cell(4, 4).Value = "Mensaje";
            ws.Cell(4, 5).Value = "Fecha Alerta";
            ws.Cell(4, 6).Value = "Leído";

            var query = _context.Alerta
                .Where(a => a.Activo && !a.Eliminado &&
                            a.IdClienteNavigation != null &&
                            a.IdClienteNavigation.IdAsesor == idAsesor);

            if (fechaDesde.HasValue)
                query = query.Where(a => a.FechaAlerta.Date >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                query = query.Where(a => a.FechaAlerta.Date <= fechaHasta.Value.Date);

            var alertas = query
                .Select(a => new
                {
                    Cliente = a.IdClienteNavigation!.Nombres + " " + a.IdClienteNavigation.Apellidos,
                    a.TipoAlerta,
                    a.Prioridad,
                    a.Mensaje,
                    a.FechaAlerta,
                    a.Leido
                })
                .ToList();

            int fila = 5;

            foreach (var a in alertas)
            {
                ws.Cell(fila, 1).Value = a.Cliente;
                ws.Cell(fila, 2).Value = a.TipoAlerta;
                ws.Cell(fila, 3).Value = a.Prioridad;
                ws.Cell(fila, 4).Value = a.Mensaje;
                ws.Cell(fila, 5).Value = a.FechaAlerta.ToString("dd/MM/yyyy HH:mm");
                ws.Cell(fila, 6).Value = a.Leido ? "Sí" : "No";
                fila++;
            }

            AplicarBordes(ws);
            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }
    }
}