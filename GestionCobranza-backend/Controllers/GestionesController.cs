using GestionCobranza_backend.Dtos.Gestiones;
using GestionCobranza_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionCobranza_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GestionesController : ControllerBase
{
    private readonly AppDbContext _context;

    private static readonly HashSet<string> TiposValidos =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "Llamada", "Mensaje", "Acuerdo", "Visita", "Otro"
        };

    public GestionesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Authorize(Roles = "Asesor,Administrador")]
    public async Task<IActionResult> RegistrarGestion([FromBody] CreateGestionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Descripcion))
            return BadRequest(new { message = "La descripción es obligatoria." });

        if (string.IsNullOrWhiteSpace(dto.TipoGestion) || !TiposValidos.Contains(dto.TipoGestion))
            return BadRequest(new { message = "El tipo de gestión debe ser: Llamada, Mensaje, Acuerdo, Visita u Otro." });

        var clienteExiste = await _context.Clientes
            .AnyAsync(c => c.IdCliente == dto.IdCliente && c.Activo && !c.Eliminado);

        if (!clienteExiste)
            return NotFound(new { message = "Cliente no encontrado." });

        var idUsuarioClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(idUsuarioClaim) || !int.TryParse(idUsuarioClaim, out var idUsuario))
            return Unauthorized(new { message = "No se pudo identificar al usuario autenticado." });

        var gestion = new GestionCobranza
        {
            IdCliente = dto.IdCliente,
            IdUsuario = idUsuario,
            TipoGestion = dto.TipoGestion.Trim(),
            Descripcion = dto.Descripcion.Trim(),
            FechaGestion = DateTime.Now,
            Resultado = dto.Resultado?.Trim(),
            ProximaAccion = dto.ProximaAccion,
            FechaRegistro = DateTime.Now,
            UsuarioRegistro = "SISTEMA",
            Activo = true,
            Eliminado = false
        };

        _context.GestionCobranzas.Add(gestion);
        var cliente = await _context.Clientes
    .FirstOrDefaultAsync(c =>
        c.IdCliente == dto.IdCliente &&
        c.Activo &&
        !c.Eliminado);

        if (cliente != null && !string.IsNullOrWhiteSpace(dto.Resultado))
        {
            var resultado = dto.Resultado.Trim().ToUpper();

            if (resultado == "CONTACTADO")
                cliente.EstadoCliente = "CONTACTADO";
            else if (resultado == "NEGOCIACION" || resultado == "NEGOCIACIÓN")
                cliente.EstadoCliente = "NEGOCIACION";
            else if (resultado == "PROMESA DE PAGO")
                cliente.EstadoCliente = "PROMESA DE PAGO";
            else if (resultado == "PAGADO")
                cliente.EstadoCliente = "PAGADO";
            else if (resultado == "MOROSO")
                cliente.EstadoCliente = "MOROSO";

            cliente.FechaModificacion = DateTime.Now;
            cliente.UsuarioModificacion = User.Identity?.Name ?? "system";
        }

        await _context.SaveChangesAsync();

        var respuesta = MapToDto(gestion);

        return CreatedAtAction(nameof(ObtenerGestion), new { id = gestion.IdGestion }, new
        {
            message = "Gestión registrada correctamente.",
            gestion = respuesta
        });
    }

    [HttpGet("cliente/{idCliente}")]
    [Authorize(Roles = "Asesor,Administrador")]
    public async Task<IActionResult> ListarGestionesPorCliente(int idCliente)
    {
        var clienteExiste = await _context.Clientes
            .AnyAsync(c => c.IdCliente == idCliente && c.Activo && !c.Eliminado);

        if (!clienteExiste)
            return NotFound(new { message = "Cliente no encontrado." });

        var gestiones = await _context.GestionCobranzas
            .Where(g => g.IdCliente == idCliente && !g.Eliminado)
            .OrderByDescending(g => g.FechaGestion)
            .Select(g => new GestionResponseDto
            {
                IdGestion = g.IdGestion,
                IdCliente = g.IdCliente,
                IdUsuario = g.IdUsuario,
                TipoGestion = g.TipoGestion,
                Descripcion = g.Descripcion,
                FechaGestion = g.FechaGestion,
                Resultado = g.Resultado,
                ProximaAccion = g.ProximaAccion,
                FechaRegistro = g.FechaRegistro,
                UsuarioRegistro = g.UsuarioRegistro,
                FechaModificacion = g.FechaModificacion,
                UsuarioModificacion = g.UsuarioModificacion,
                Activo = g.Activo,
                Eliminado = g.Eliminado
            })
            .ToListAsync();

        return Ok(gestiones);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Asesor,Administrador")]
    public async Task<IActionResult> ObtenerGestion(int id)
    {
        var gestion = await _context.GestionCobranzas
            .Where(g => g.IdGestion == id && !g.Eliminado)
            .Select(g => new GestionResponseDto
            {
                IdGestion = g.IdGestion,
                IdCliente = g.IdCliente,
                IdUsuario = g.IdUsuario,
                TipoGestion = g.TipoGestion,
                Descripcion = g.Descripcion,
                FechaGestion = g.FechaGestion,
                Resultado = g.Resultado,
                ProximaAccion = g.ProximaAccion,
                FechaRegistro = g.FechaRegistro,
                UsuarioRegistro = g.UsuarioRegistro,
                FechaModificacion = g.FechaModificacion,
                UsuarioModificacion = g.UsuarioModificacion,
                Activo = g.Activo,
                Eliminado = g.Eliminado
            })
            .FirstOrDefaultAsync();

        if (gestion == null)
            return NotFound(new { message = "Gestión no encontrada." });

        return Ok(gestion);
    }

    private static GestionResponseDto MapToDto(GestionCobranza g) => new()
    {
        IdGestion = g.IdGestion,
        IdCliente = g.IdCliente,
        IdUsuario = g.IdUsuario,
        TipoGestion = g.TipoGestion,
        Descripcion = g.Descripcion,
        FechaGestion = g.FechaGestion,
        Resultado = g.Resultado,
        ProximaAccion = g.ProximaAccion,
        FechaRegistro = g.FechaRegistro,
        UsuarioRegistro = g.UsuarioRegistro,
        FechaModificacion = g.FechaModificacion,
        UsuarioModificacion = g.UsuarioModificacion,
        Activo = g.Activo,
        Eliminado = g.Eliminado
    };
}
