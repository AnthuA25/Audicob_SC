using GestionCobranza_backend.Dtos.Roles;
using GestionCobranza_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionCobranza_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly AppDbContext _context;

    public RolesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> ListarRoles()
    {
        var roles = await _context.Rols
            .Where(r => r.Activo && !r.Eliminado)
            .OrderBy(r => r.IdRol)
            .Select(r => new
            {
                r.IdRol,
                r.Nombre,
                r.Descripcion,
                r.FechaRegistro,
                r.Activo
            })
            .ToListAsync();

        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerRolPorId(int id)
    {
        var rol = await _context.Rols
            .Where(r => r.IdRol == id && r.Activo && !r.Eliminado)
            .Select(r => new
            {
                r.IdRol,
                r.Nombre,
                r.Descripcion,
                r.FechaRegistro,
                r.Activo
            })
            .FirstOrDefaultAsync();

        if (rol == null)
            return NotFound(new { message = "Rol no encontrado." });

        return Ok(rol);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CreateRolDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            return BadRequest(new { message = "El nombre del rol es obligatorio." });
        }

        var existe = await _context.Rols.AnyAsync(r =>
            r.Nombre.ToLower() == dto.Nombre.ToLower().Trim() &&
            !r.Eliminado);

        if (existe)
        {
            return BadRequest(new { message = "Ya existe un rol con ese nombre." });
        }

        var rol = new Rol
        {
            Nombre = dto.Nombre.Trim(),
            Descripcion = dto.Descripcion?.Trim(),
            FechaRegistro = DateTime.Now,
            UsuarioRegistro = "system",
            Activo = true,
            Eliminado = false
        };

        _context.Rols.Add(rol);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Rol creado correctamente.",
            rol = new
            {
                rol.IdRol,
                rol.Nombre,
                rol.Descripcion
            }
        });
    }
}