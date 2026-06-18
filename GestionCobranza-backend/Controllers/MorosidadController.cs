using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionCobranza_backend.Dtos.Morosidad;
using GestionCobranza_backend.Models;
using ClosedXML.Excel;

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

    [HttpGet("reporte")]
    public async Task<IActionResult> GenerarReporteMorosidad()
    {
        var clientesQuery = await _context.Clientes
            .Include(c => c.IdAsesorNavigation)
            .Include(c => c.Deuda.Where(d => !d.Eliminado && d.DiasAtraso > 0))
            .Where(c => !c.Eliminado && c.EstadoCliente == "MOROSO")
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Morosidad");

        ws.Cell(1, 1).Value = "REPORTE DE SEGUIMIENTO DE MOROSIDAD";
        ws.Range(1, 1, 1, 8).Merge();

        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 16;
        ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        ws.Cell(2, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
        ws.Range(2, 1, 2, 8).Merge();
        ws.Cell(2, 1).Style.Font.Italic = true;
        ws.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        ws.Cell(4, 1).Value = "Cliente";
        ws.Cell(4, 2).Value = "Correo";
        ws.Cell(4, 3).Value = "Teléfono";
        ws.Cell(4, 4).Value = "Asesor Asignado";
        ws.Cell(4, 5).Value = "Días Atraso";
        ws.Cell(4, 6).Value = "Deuda Pendiente";
        ws.Cell(4, 7).Value = "Riesgo";
        ws.Cell(4, 8).Value = "Estado";

        var encabezado = ws.Range(4, 1, 4, 8);
        encabezado.Style.Font.Bold = true;
        encabezado.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E3A5F");
        encabezado.Style.Font.FontColor = XLColor.White;
        encabezado.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        int fila = 5;

        foreach (var c in clientesQuery)
        {
            var diasAtraso = c.Deuda.Any() ? c.Deuda.Max(d => d.DiasAtraso) : 0;
            var deudaPendiente = c.Deuda.Sum(d => d.SaldoPendiente);

            ws.Cell(fila, 1).Value = $"{c.Nombres} {c.Apellidos}".Trim();
            ws.Cell(fila, 2).Value = c.Correo ?? "-";
            ws.Cell(fila, 3).Value = c.Telefono ?? "-";
            ws.Cell(fila, 4).Value = c.IdAsesorNavigation != null
                ? $"{c.IdAsesorNavigation.Nombres} {c.IdAsesorNavigation.Apellidos}".Trim()
                : "Sin Asesor";
            ws.Cell(fila, 5).Value = diasAtraso;
            ws.Cell(fila, 6).Value = deudaPendiente;
            ws.Cell(fila, 7).Value = c.Riesgo;
            ws.Cell(fila, 8).Value = c.EstadoCliente;

            fila++;
        }

        if (fila > 5)
        {
            ws.Range(5, 6, fila - 1, 6).Style.NumberFormat.Format = "\"S/.\" #,##0.00";
        }

        var rango = ws.RangeUsed();
        if (rango != null)
        {
            rango.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rango.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ws.Columns().AdjustToContents();
        }

        ws.SheetView.FreezeRows(4);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var nombreArchivo = $"reporte_seguimiento_morosidad_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            nombreArchivo
        );
    }
}