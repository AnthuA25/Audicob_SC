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
                "RENDIMIENTO POR ASESOR" => CrearExcelRendimientoAsesores(nombreArchivo, data, baseUrl),
                "RESUMEN DE CLIENTES" => CrearExcelResumenClientes(nombreArchivo, data, baseUrl),
                "REPORTE DE DEUDAS" => CrearExcelDeudas(nombreArchivo, baseUrl),
                "REPORTE DE PAGOS" => CrearExcelPagos(nombreArchivo, baseUrl),
                "REPORTE DE MOROSIDAD" => CrearExcelMorosidad(nombreArchivo, baseUrl),
                "REPORTE DE ALERTAS" => CrearExcelAlertas(nombreArchivo, baseUrl),
                _ => CrearExcelAdmin(nombreArchivo, data, baseUrl)
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
                "RESUMEN DE CARTERA" => $"reporte_resumen_cartera_asesor_{idAsesor}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                "DISTRIBUCION DE CLIENTES" => $"reporte_distribucion_clientes_asesor_{idAsesor}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                _ => $"reporte_general_asesor_{idAsesor}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            string urlDescarga = tipo switch
            {
                "RESUMEN DE CARTERA" => CrearExcelResumenCartera(nombreArchivo, data, baseUrl),
                "DISTRIBUCION DE CLIENTES" => CrearExcelDistribucionClientes(nombreArchivo, data, baseUrl),
                _ => CrearExcelAsesor(nombreArchivo, data, baseUrl)
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

        private static void DarFormatoTabla(IXLWorksheet ws, string titulo, int columnas)
        {
            ws.Cell(1, 1).Value = titulo;
            ws.Range(1, 1, 1, columnas).Merge();

            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 16;
            ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(2, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
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

        private string CrearExcelAdmin(string nombreArchivo, ReporteGerencialDto data, string baseUrl)
        {
            using var workbook = new XLWorkbook();

            var ws = workbook.Worksheets.Add("Reporte General");

            DarFormatoTabla(ws, "REPORTE GENERAL DE COBRANZA", 10);

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

            var clientes = _context.Clientes
                .Where(c => c.Activo && !c.Eliminado)
                .Select(c => new
                {
                    Cliente = c.Nombres + " " + c.Apellidos,
                    c.Dni,
                    c.Correo,
                    c.Telefono,
                    Asesor = c.IdAsesorNavigation != null
                        ? c.IdAsesorNavigation.Nombres + " " + c.IdAsesorNavigation.Apellidos
                        : "-",
                    DeudaTotal = c.Deuda
                        .Where(d => d.Activo && !d.Eliminado)
                        .Sum(d => d.MontoTotal),
                    MontoPagado = c.Deuda
                        .Where(d => d.Activo && !d.Eliminado)
                        .Sum(d => d.MontoPagado),
                    SaldoPendiente = c.Deuda
                        .Where(d => d.Activo && !d.Eliminado)
                        .Sum(d => d.SaldoPendiente),
                    DiasAtraso = c.Deuda
                        .Where(d => d.Activo && !d.Eliminado)
                        .Any()
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

            CrearHojaRendimientoAsesores(workbook, data);
            CrearHojaResumenClientes(workbook, data);

            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private static string CrearExcelAsesor(string nombreArchivo, ReporteRendimientoIndividualDto data, string baseUrl)
        {
            using var workbook = new XLWorkbook();
            CrearHojaResumenCartera(workbook, data);
            CrearHojaDistribucionClientes(workbook, data);
            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private static string CrearExcelRendimientoAsesores(string nombreArchivo, ReporteGerencialDto data, string baseUrl)
        {
            using var workbook = new XLWorkbook();
            CrearHojaRendimientoAsesores(workbook, data);
            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private static string CrearExcelResumenClientes(string nombreArchivo, ReporteGerencialDto data, string baseUrl)
        {
            using var workbook = new XLWorkbook();
            CrearHojaResumenClientes(workbook, data);
            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private static string CrearExcelResumenCartera(string nombreArchivo, ReporteRendimientoIndividualDto data, string baseUrl)
        {
            using var workbook = new XLWorkbook();
            CrearHojaResumenCartera(workbook, data);
            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private static string CrearExcelDistribucionClientes(string nombreArchivo, ReporteRendimientoIndividualDto data, string baseUrl)
        {
            using var workbook = new XLWorkbook();
            CrearHojaDistribucionClientes(workbook, data);
            return GuardarExcel(workbook, nombreArchivo, baseUrl);
        }

        private static void CrearHojaRendimientoAsesores(XLWorkbook workbook, ReporteGerencialDto data)
        {
            var ws = workbook.Worksheets.Add("Rendimiento Asesores");

            DarFormatoTabla(ws, "REPORTE DE RENDIMIENTO POR ASESOR", 5);

            ws.Cell(4, 1).Value = "Asesor";
            ws.Cell(4, 2).Value = "Clientes";
            ws.Cell(4, 3).Value = "Deuda Gestionada";
            ws.Cell(4, 4).Value = "Pagos Recuperados";
            ws.Cell(4, 5).Value = "Eficiencia";

            int fila = 5;

            foreach (var item in data.RendimientoAsesores)
            {
                ws.Cell(fila, 1).Value = item.Asesor;
                ws.Cell(fila, 2).Value = item.Clientes;
                ws.Cell(fila, 3).Value = item.DeudaGestionada;
                ws.Cell(fila, 4).Value = item.PagosRecuperados;
                ws.Cell(fila, 5).Value = item.Eficiencia;
                fila++;
            }

            if (fila > 5)
                ws.Range(5, 3, fila - 1, 4).Style.NumberFormat.Format = "\"S/.\" #,##0.00";

            AplicarBordes(ws);
        }

        private static void CrearHojaResumenClientes(XLWorkbook workbook, ReporteGerencialDto data)
        {
            var ws = workbook.Worksheets.Add("Resumen Clientes");

            DarFormatoTabla(ws, "REPORTE DE RESUMEN DE CLIENTES", 4);

            ws.Cell(4, 1).Value = "Estado";
            ws.Cell(4, 2).Value = "Cantidad";
            ws.Cell(4, 3).Value = "Deuda Total";
            ws.Cell(4, 4).Value = "Porcentaje";

            int fila = 5;

            foreach (var item in data.ResumenClientes)
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

        private static void CrearHojaResumenCartera(XLWorkbook workbook, ReporteRendimientoIndividualDto data)
        {
            var ws = workbook.Worksheets.Add("Resumen Cartera");

            DarFormatoTabla(ws, "REPORTE DE RESUMEN DE CARTERA", 4);

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

        private static void CrearHojaDistribucionClientes(XLWorkbook workbook, ReporteRendimientoIndividualDto data)
        {
            var ws = workbook.Worksheets.Add("Distribución Clientes");

            DarFormatoTabla(ws, "REPORTE DE DISTRIBUCIÓN DE CLIENTES", 4);

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

        private string CrearExcelDeudas(string nombreArchivo, string baseUrl)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Deudas");

            DarFormatoTabla(ws, "REPORTE DE DEUDAS", 8);

            ws.Cell(4, 1).Value = "Cliente";
            ws.Cell(4, 2).Value = "Asesor";
            ws.Cell(4, 3).Value = "Monto Total";
            ws.Cell(4, 4).Value = "Monto Pagado";
            ws.Cell(4, 5).Value = "Saldo Pendiente";
            ws.Cell(4, 6).Value = "Fecha Vencimiento";
            ws.Cell(4, 7).Value = "Días Atraso";
            ws.Cell(4, 8).Value = "Estado Deuda";

            var deudas = _context.Deuda
                .Where(d => d.Activo && !d.Eliminado)
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

        private string CrearExcelPagos(string nombreArchivo, string baseUrl)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Pagos");

            DarFormatoTabla(ws, "REPORTE DE PAGOS", 7);

            ws.Cell(4, 1).Value = "Cliente";
            ws.Cell(4, 2).Value = "Asesor";
            ws.Cell(4, 3).Value = "Monto";
            ws.Cell(4, 4).Value = "Fecha Pago";
            ws.Cell(4, 5).Value = "Método Pago";
            ws.Cell(4, 6).Value = "Estado Pago";
            ws.Cell(4, 7).Value = "Nota";

            var pagos = _context.Pagos
                .Where(p => p.Activo && !p.Eliminado)
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

        private string CrearExcelMorosidad(string nombreArchivo, string baseUrl)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Morosidad");

            DarFormatoTabla(ws, "REPORTE DE MOROSIDAD", 8);

            ws.Cell(4, 1).Value = "Cliente";
            ws.Cell(4, 2).Value = "DNI";
            ws.Cell(4, 3).Value = "Asesor";
            ws.Cell(4, 4).Value = "Saldo Pendiente";
            ws.Cell(4, 5).Value = "Días Atraso";
            ws.Cell(4, 6).Value = "Fecha Vencimiento";
            ws.Cell(4, 7).Value = "Riesgo";
            ws.Cell(4, 8).Value = "Estado Cliente";

            var morosos = _context.Deuda
                .Where(d => d.Activo && !d.Eliminado && d.SaldoPendiente > 0 && d.DiasAtraso > 0)
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

        private string CrearExcelAlertas(string nombreArchivo, string baseUrl)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Alertas");

            DarFormatoTabla(ws, "REPORTE DE ALERTAS", 7);

            ws.Cell(4, 1).Value = "Cliente";
            ws.Cell(4, 2).Value = "Deuda";
            ws.Cell(4, 3).Value = "Tipo Alerta";
            ws.Cell(4, 4).Value = "Prioridad";
            ws.Cell(4, 5).Value = "Mensaje";
            ws.Cell(4, 6).Value = "Fecha Alerta";
            ws.Cell(4, 7).Value = "Leído";

            var alertas = _context.Alerta
                .Where(a => a.Activo && !a.Eliminado)
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
    }
}