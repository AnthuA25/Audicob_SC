using ClosedXML.Excel;
using GestionCobranza_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace GestionCobranza_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class ImportacionesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ImportacionesController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet("plantilla")]
    public IActionResult DescargarPlantilla()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Clientes");

        worksheet.Cell(1, 1).Value = "Nombres";
        worksheet.Cell(1, 2).Value = "Apellidos";
        worksheet.Cell(1, 3).Value = "DNI";
        worksheet.Cell(1, 4).Value = "Correo";
        worksheet.Cell(1, 5).Value = "Telefono";
        worksheet.Cell(1, 6).Value = "Direccion";
        worksheet.Cell(1, 7).Value = "IdAsesor";
        worksheet.Cell(1, 8).Value = "Observacion";

        worksheet.Range("A1:H1").Style.Font.Bold = true;
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "plantilla_clientes.xlsx"
        );
    }

    [HttpPost("subir")]
    public async Task<IActionResult> SubirArchivo(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest(new { message = "Debe seleccionar un archivo." });

        var extension = Path.GetExtension(archivo.FileName).ToLower();

        if (extension != ".xlsx")
            return BadRequest(new { message = "Solo se permite archivo Excel .xlsx." });

        var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(idUsuarioClaim))
            return Unauthorized(new { message = "No se pudo identificar al usuario autenticado." });

        var idUsuario = int.Parse(idUsuarioClaim);

        var carpetaUploads = Path.Combine(
            _environment.ContentRootPath,
            "Uploads",
            "importaciones"
        );

        if (!Directory.Exists(carpetaUploads))
            Directory.CreateDirectory(carpetaUploads);

        var nombreGuardado = $"{Guid.NewGuid()}_{Path.GetFileName(archivo.FileName)}";
        var rutaCompleta = Path.Combine(carpetaUploads, nombreGuardado);

        await using (var stream = new FileStream(rutaCompleta, FileMode.Create))
        {
            await archivo.CopyToAsync(stream);
        }

        var totalRegistros = 0;
        var registrosOmitidos = 0;

        using var workbook = new XLWorkbook(rutaCompleta);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RangeUsed()?.RowsUsed().Skip(1);

        if (rows == null)
            return BadRequest(new { message = "El archivo está vacío." });

        foreach (var row in rows)
        {
            var nombres = row.Cell(1).GetString().Trim();
            var apellidos = row.Cell(2).GetString().Trim();
            var dni = row.Cell(3).GetString().Trim();
            var correo = row.Cell(4).GetString().Trim();
            var telefono = row.Cell(5).GetString().Trim();
            var direccion = row.Cell(6).GetString().Trim();
            var idAsesorTexto = row.Cell(7).GetString().Trim();
            var observacion = row.Cell(8).GetString().Trim();

            if (string.IsNullOrWhiteSpace(nombres) ||
                string.IsNullOrWhiteSpace(apellidos) ||
                string.IsNullOrWhiteSpace(dni))
            {
                registrosOmitidos++;
                continue;
            }

            if (!Regex.IsMatch(dni, @"^\d{8}$"))
            {
                registrosOmitidos++;
                continue;
            }

            var existeCliente = await _context.Clientes.AnyAsync(c =>
                c.Dni == dni && !c.Eliminado);

            if (existeCliente)
            {
                registrosOmitidos++;
                continue;
            }

            int? idAsesor = null;

            if (int.TryParse(idAsesorTexto, out var asesorId))
            {
                var asesorExiste = await _context.Usuarios
                    .Include(u => u.IdRolNavigation)
                    .AnyAsync(u =>
                        u.IdUsuario == asesorId &&
                        u.Activo &&
                        !u.Eliminado &&
                        u.IdRolNavigation.Nombre == "Asesor");

                if (asesorExiste)
                    idAsesor = asesorId;
            }

            var cliente = new Cliente
            {
                IdAsesor = idAsesor,
                Nombres = nombres,
                Apellidos = apellidos,
                Dni = dni,
                Correo = string.IsNullOrWhiteSpace(correo) ? null : correo.ToLower(),
                Telefono = string.IsNullOrWhiteSpace(telefono) ? null : telefono,
                Direccion = string.IsNullOrWhiteSpace(direccion) ? null : direccion,
                EstadoCliente = "NUEVO",
                Riesgo = "BAJO",
                Observacion = string.IsNullOrWhiteSpace(observacion) ? null : observacion,
                FechaRegistro = DateTime.Now,
                UsuarioRegistro = User.Identity?.Name ?? "system",
                Activo = true,
                Eliminado = false
            };

            _context.Clientes.Add(cliente);
            totalRegistros++;
        }

        var importacion = new ImportacionDato
        {
            IdUsuario = idUsuario,
            NombreArchivo = archivo.FileName,
            RutaArchivo = rutaCompleta,
            TotalRegistros = totalRegistros,
            EstadoImportacion = "PROCESADO",
            Observacion = $"Se importaron {totalRegistros} clientes. Omitidos: {registrosOmitidos}.",
            FechaRegistro = DateTime.Now,
            UsuarioRegistro = User.Identity?.Name ?? "system",
            Activo = true,
            Eliminado = false
        };

        _context.ImportacionDatos.Add(importacion);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Archivo importado correctamente.",
            totalRegistros,
            registrosOmitidos,
            importacion = new
            {
                importacion.IdImportacion,
                importacion.NombreArchivo,
                importacion.TotalRegistros,
                importacion.EstadoImportacion,
                importacion.Observacion,
                importacion.FechaRegistro
            }
        });
    }

    [HttpGet("recientes")]
    public async Task<IActionResult> ListarRecientes()
    {
        var importaciones = await _context.ImportacionDatos
            .Where(i => i.Activo && !i.Eliminado)
            .OrderByDescending(i => i.FechaRegistro)
            .Take(10)
            .Select(i => new
            {
                i.IdImportacion,
                i.NombreArchivo,
                i.TotalRegistros,
                i.EstadoImportacion,
                i.Observacion,
                i.FechaRegistro
            })
            .ToListAsync();

        return Ok(importaciones);
    }

    [HttpGet("{id}/descargar")]
    public async Task<IActionResult> DescargarImportacion(int id)
    {
        var importacion = await _context.ImportacionDatos
            .FirstOrDefaultAsync(i =>
                i.IdImportacion == id &&
                i.Activo &&
                !i.Eliminado);

        if (importacion == null)
            return NotFound(new { message = "Importación no encontrada." });

        if (string.IsNullOrWhiteSpace(importacion.RutaArchivo))
            return NotFound(new { message = "La importación no tiene archivo asociado." });

        var rutaArchivo = importacion.RutaArchivo;

        if (!System.IO.File.Exists(rutaArchivo))
        {
            return NotFound(new
            {
                message = "El archivo no existe en el servidor.",
                ruta = rutaArchivo
            });
        }

        var bytes = await System.IO.File.ReadAllBytesAsync(rutaArchivo);

        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            importacion.NombreArchivo
        );
    }

}