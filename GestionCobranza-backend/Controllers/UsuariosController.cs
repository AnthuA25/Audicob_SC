using GestionCobranza_backend.Dtos.Usuarios;
using GestionCobranza_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GestionCobranza_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuariosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ListarUsuarios()
    {
        var usuarios = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .Where(u => u.Activo && !u.Eliminado)
            .Select(u => new
            {
                u.IdUsuario,
                u.Nombres,
                u.Apellidos,
                u.Dni,
                u.Correo,
                u.Telefono,
                u.Estado,
                Rol = u.IdRolNavigation.Nombre
            })
            .ToListAsync();

        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerUsuarioPorId(int id)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .Where(u => u.IdUsuario == id && u.Activo && !u.Eliminado)
            .Select(u => new
            {
                u.IdUsuario,
                u.Nombres,
                u.Apellidos,
                u.Dni,
                u.Correo,
                u.Telefono,
                u.Estado,
                Rol = u.IdRolNavigation.Nombre
            })
            .FirstOrDefaultAsync();

        if (usuario == null)
        {
            return NotFound(new { message = "Usuario no encontrado." });
        }

        return Ok(usuario);
    }

    [HttpPost]
    // [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear([FromBody] CreateUsuarioDto dto)
    {

        if (string.IsNullOrWhiteSpace(dto.Nombres) ||
            string.IsNullOrWhiteSpace(dto.Apellidos) ||
            string.IsNullOrWhiteSpace(dto.Dni) ||
            string.IsNullOrWhiteSpace(dto.Correo) ||
            string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new { message = "Faltan campos obligatorios." });
        }

        var existeAdmin = await _context.Usuarios
        .Include(u => u.IdRolNavigation)
        .AnyAsync(u =>
            u.Activo &&
            !u.Eliminado &&
            u.IdRolNavigation.Nombre == "Administrador");

        if (existeAdmin)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized(new { message = "Debes iniciar sesión como administrador." });

            var rolUsuario = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (rolUsuario != "Administrador")
                return Forbid();
        }

        var rolExiste = await _context.Rols.AnyAsync(r =>
            r.IdRol == dto.IdRol && r.Activo && !r.Eliminado);

        if (!rolExiste)
            return BadRequest(new { message = "El rol seleccionado no existe." });

        var correoExiste = await _context.Usuarios.AnyAsync(u =>
            u.Correo.ToLower() == dto.Correo.ToLower().Trim() && !u.Eliminado);

        if (correoExiste)
            return BadRequest(new { message = "Ya existe un usuario con ese correo." });

        var dniExiste = await _context.Usuarios.AnyAsync(u =>
            u.Dni == dto.Dni.Trim() && !u.Eliminado);

        if (dniExiste)
            return BadRequest(new { message = "Ya existe un usuario con ese DNI." });

        var usuario = new Usuario
        {
            IdRol = dto.IdRol,
            Nombres = dto.Nombres.Trim(),
            Apellidos = dto.Apellidos.Trim(),
            Dni = dto.Dni.Trim(),
            Correo = dto.Correo.Trim().ToLower(),
            Telefono = dto.Telefono?.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Estado = "ACTIVO",
            FechaRegistro = DateTime.Now,
            UsuarioRegistro = existeAdmin
            ? User.Identity?.Name ?? "Administrador"
            : "system",
            Activo = true,
            Eliminado = false
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = existeAdmin ? "Usuario creado correctamente." : "Primer administrador creado correctamente",
            usuario = new
            {
                usuario.IdUsuario,
                usuario.Nombres,
                usuario.Apellidos,
                usuario.Correo,
                usuario.Estado
            }
        });
    }

    [HttpGet("asesores")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ListarAsesores([FromQuery] string? busqueda)
    {
        var query = _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .Where(u =>
                u.Activo &&
                !u.Eliminado &&
                u.IdRolNavigation.Nombre == "Asesor");

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var texto = busqueda.Trim().ToLower();

            query = query.Where(u =>
                u.Nombres.ToLower().Contains(texto) ||
                u.Apellidos.ToLower().Contains(texto) ||
                u.Dni.ToLower().Contains(texto) ||
                u.Correo.ToLower().Contains(texto));
        }

        var asesores = await query
            .OrderBy(u => u.Apellidos)
            .ThenBy(u => u.Nombres)
            .Select(u => new
            {
                u.IdUsuario,
                u.Nombres,
                u.Apellidos,
                u.Dni,
                u.Correo,
                u.Telefono,
                u.Estado,
                ClientesAsignados = _context.Clientes
                    .Count(c => c.IdAsesor == u.IdUsuario && c.Activo && !c.Eliminado),

                DeudaGestionada = _context.Clientes
                    .Where(c => c.IdAsesor == u.IdUsuario && c.Activo && !c.Eliminado)
                    .SelectMany(c => c.Deuda)
                    .Where(d => d.Activo && !d.Eliminado)
                    .Sum(d => d.MontoTotal),

                PagosRecuperados = _context.Clientes
                    .Where(c => c.IdAsesor == u.IdUsuario && c.Activo && !c.Eliminado)
                    .SelectMany(c => c.Deuda)
                    .SelectMany(d => d.Pagos)
                    .Where(p => p.Activo && !p.Eliminado)
                    .Sum(p => p.Monto)

            })
            .ToListAsync();

        return Ok(asesores);
    }

    [HttpGet("asesores/{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerAsesorPorId(int id)
    {
        var asesor = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .Where(u =>
                u.IdUsuario == id &&
                u.Activo &&
                !u.Eliminado &&
                u.IdRolNavigation.Nombre == "Asesor")
            .Select(u => new
            {
                u.IdUsuario,
                u.Nombres,
                u.Apellidos,
                u.Dni,
                u.Correo,
                u.Telefono,
                u.Estado,
                Rol = u.IdRolNavigation.Nombre
            })
            .FirstOrDefaultAsync();

        if (asesor == null)
            return NotFound(new { message = "Asesor no encontrado." });

        return Ok(asesor);
    }

    [HttpPost("asesores")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CrearAsesor([FromBody] CreateAsesorDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombres) ||
            string.IsNullOrWhiteSpace(dto.Apellidos) ||
            string.IsNullOrWhiteSpace(dto.Dni) ||
            string.IsNullOrWhiteSpace(dto.Correo) ||
            string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new { message = "Nombres, apellidos, DNI, correo y contraseña son obligatorios." });
        }

        if (dto.Dni.Trim().Length < 8)
            return BadRequest(new { message = "El DNI debe tener al menos 8 caracteres." });

        if (!dto.Correo.Contains("@"))
            return BadRequest(new { message = "El correo no es válido." });

        if (dto.Password.Length < 6)
            return BadRequest(new { message = "La contraseña debe tener al menos 6 caracteres." });

        var correoExiste = await _context.Usuarios.AnyAsync(u =>
            u.Correo.ToLower() == dto.Correo.ToLower().Trim() && !u.Eliminado);

        if (correoExiste)
            return BadRequest(new { message = "Ya existe un usuario con ese correo." });

        var dniExiste = await _context.Usuarios.AnyAsync(u =>
            u.Dni == dto.Dni.Trim() && !u.Eliminado);

        if (dniExiste)
            return BadRequest(new { message = "Ya existe un usuario con ese DNI." });

        var rolAsesor = await _context.Rols
            .FirstOrDefaultAsync(r =>
                r.Nombre == "Asesor" &&
                r.Activo &&
                !r.Eliminado);

        if (rolAsesor == null)
            return BadRequest(new { message = "No existe el rol Asesor." });

        var asesor = new Usuario
        {
            IdRol = rolAsesor.IdRol,
            Nombres = dto.Nombres.Trim(),
            Apellidos = dto.Apellidos.Trim(),
            Dni = dto.Dni.Trim(),
            Correo = dto.Correo.Trim().ToLower(),
            Telefono = dto.Telefono?.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Estado = "ACTIVO",
            FechaRegistro = DateTime.Now,
            UsuarioRegistro = User.Identity?.Name ?? "system",
            Activo = true,
            Eliminado = false
        };

        _context.Usuarios.Add(asesor);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Asesor creado correctamente.",
            asesor = new
            {
                asesor.IdUsuario,
                asesor.Nombres,
                asesor.Apellidos,
                asesor.Dni,
                asesor.Correo,
                asesor.Telefono,
                asesor.Estado
            }
        });
    }

    [HttpPut("asesores/{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ActualizarAsesor(int id, [FromBody] UpdateAsesorDto dto)
    {
        var asesor = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .FirstOrDefaultAsync(u =>
                u.IdUsuario == id &&
                u.Activo &&
                !u.Eliminado &&
                u.IdRolNavigation.Nombre == "Asesor");

        if (asesor == null)
            return NotFound(new { message = "Asesor no encontrado." });

        if (string.IsNullOrWhiteSpace(dto.Nombres) ||
            string.IsNullOrWhiteSpace(dto.Apellidos) ||
            string.IsNullOrWhiteSpace(dto.Dni) ||
            string.IsNullOrWhiteSpace(dto.Correo) ||
            string.IsNullOrWhiteSpace(dto.Estado))
        {
            return BadRequest(new { message = "Nombres, apellidos, DNI, correo y estado son obligatorios." });
        }

        if (dto.Dni.Trim().Length < 8)
            return BadRequest(new { message = "El DNI debe tener al menos 8 caracteres." });

        if (!dto.Correo.Contains("@"))
            return BadRequest(new { message = "El correo no es válido." });

        if (dto.Estado.Trim() != "ACTIVO" && dto.Estado.Trim() != "INACTIVO")
            return BadRequest(new { message = "El estado solo puede ser ACTIVO o INACTIVO." });

        var correoDuplicado = await _context.Usuarios.AnyAsync(u =>
            u.IdUsuario != id &&
            u.Correo.ToLower() == dto.Correo.ToLower().Trim() &&
            !u.Eliminado);

        if (correoDuplicado)
            return BadRequest(new { message = "Ya existe otro usuario con ese correo." });

        var dniDuplicado = await _context.Usuarios.AnyAsync(u =>
            u.IdUsuario != id &&
            u.Dni == dto.Dni.Trim() &&
            !u.Eliminado);

        if (dniDuplicado)
            return BadRequest(new { message = "Ya existe otro usuario con ese DNI." });

        asesor.Nombres = dto.Nombres.Trim();
        asesor.Apellidos = dto.Apellidos.Trim();
        asesor.Dni = dto.Dni.Trim();
        asesor.Correo = dto.Correo.Trim().ToLower();
        asesor.Telefono = dto.Telefono?.Trim();
        asesor.Estado = dto.Estado.Trim();
        asesor.FechaModificacion = DateTime.Now;
        asesor.UsuarioModificacion = User.Identity?.Name ?? "system";

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            if (dto.Password.Length < 6)
                return BadRequest(new { message = "La nueva contraseña debe tener al menos 6 caracteres." });

            asesor.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Asesor actualizado correctamente.",
            asesor = new
            {
                asesor.IdUsuario,
                asesor.Nombres,
                asesor.Apellidos,
                asesor.Dni,
                asesor.Correo,
                asesor.Telefono,
                asesor.Estado
            }
        });
    }

    [HttpDelete("asesores/{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> EliminarAsesor(int id)
    {
        var asesor = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .FirstOrDefaultAsync(u =>
                u.IdUsuario == id &&
                u.Activo &&
                !u.Eliminado &&
                u.IdRolNavigation.Nombre == "Asesor");

        if (asesor == null)
            return NotFound(new { message = "Asesor no encontrado." });

        var tieneClientesAsignados = await _context.Clientes.AnyAsync(c =>
            c.IdAsesor == id &&
            c.Activo &&
            !c.Eliminado);

        if (tieneClientesAsignados)
        {
            return BadRequest(new
            {
                message = "No se puede eliminar el asesor porque tiene clientes asignados."
            });
        }

        asesor.Activo = false;
        asesor.Eliminado = true;
        asesor.Estado = "INACTIVO";
        asesor.FechaModificacion = DateTime.Now;
        asesor.UsuarioModificacion = User.Identity?.Name ?? "system";

        await _context.SaveChangesAsync();

        return Ok(new { message = "Asesor eliminado correctamente." });
    }
}