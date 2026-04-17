using GestionCobranza_backend.Dtos.Clientes;
using GestionCobranza_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionCobranza_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClientesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ListarClientes()
    {
        var clientes = await _context.Clientes
            .Include(c => c.IdAsesorNavigation)
            .Where(c => c.Activo && !c.Eliminado)
            .Select(c => new
            {
                c.IdCliente,
                c.Nombres,
                c.Apellidos,
                c.Dni,
                c.Correo,
                c.Telefono,
                c.Direccion,
                c.EstadoCliente,
                c.Riesgo,
                Asesor = c.IdAsesorNavigation != null
                    ? c.IdAsesorNavigation.Nombres + " " + c.IdAsesorNavigation.Apellidos
                    : null
            })
            .ToListAsync();

        return Ok(clientes);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Asesor")]
    public async Task<IActionResult> ObtenerClientePorId(int id)
    {
        var cliente = await _context.Clientes
            .Include(c => c.IdAsesorNavigation)
            .Where(c => c.IdCliente == id && c.Activo && !c.Eliminado)
            .Select(c => new
            {
                c.IdCliente,
                c.Nombres,
                c.Apellidos,
                c.Dni,
                c.Correo,
                c.Telefono,
                c.Direccion,
                c.EstadoCliente,
                c.Riesgo,
                c.Observacion,
                c.FechaRegistro,
                c.UsuarioRegistro,
                Asesor = c.IdAsesorNavigation != null
                    ? c.IdAsesorNavigation.Nombres + " " + c.IdAsesorNavigation.Apellidos
                    : null
            })
            .FirstOrDefaultAsync();

        if (cliente == null)
            return NotFound(new { message = "Cliente no encontrado." });

        return Ok(cliente);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> RegistrarCliente([FromBody] CreateClienteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombres) ||
            string.IsNullOrWhiteSpace(dto.Apellidos) ||
            string.IsNullOrWhiteSpace(dto.Dni))
        {
            return BadRequest(new { message = "Nombres, apellidos y DNI son obligatorios." });
        }

        var dniExiste = await _context.Clientes.AnyAsync(c =>
            c.Dni == dto.Dni.Trim() && !c.Eliminado);

        if (dniExiste)
            return BadRequest(new { message = "Ya existe un cliente con ese DNI." });

        if (dto.IdAsesor.HasValue)
        {
            var asesorExiste = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .AnyAsync(u =>
                    u.IdUsuario == dto.IdAsesor.Value &&
                    u.Activo &&
                    !u.Eliminado &&
                    u.IdRolNavigation.Nombre == "Asesor");

            if (!asesorExiste)
                return BadRequest(new { message = "El asesor asignado no existe o no tiene rol Asesor." });
        }

        var cliente = new Cliente
        {
            IdAsesor = dto.IdAsesor,
            Nombres = dto.Nombres.Trim(),
            Apellidos = dto.Apellidos.Trim(),
            Dni = dto.Dni.Trim(),
            Correo = dto.Correo?.Trim().ToLower(),
            Telefono = dto.Telefono?.Trim(),
            Direccion = dto.Direccion?.Trim(),
            EstadoCliente = "NUEVO",
            Riesgo = "BAJO",
            Observacion = dto.Observacion?.Trim(),
            FechaRegistro = DateTime.Now,
            UsuarioRegistro = User.Identity?.Name ?? "system",
            Activo = true,
            Eliminado = false
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Cliente registrado correctamente.",
            cliente
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ActualizarCliente(int id, [FromBody] UpdateClienteDto dto)
    {
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.IdCliente == id && c.Activo && !c.Eliminado);

        if (cliente == null)
            return NotFound(new { message = "Cliente no encontrado." });

        var dniDuplicado = await _context.Clientes.AnyAsync(c =>
            c.IdCliente != id &&
            c.Dni == dto.Dni.Trim() &&
            !c.Eliminado);

        if (dniDuplicado)
            return BadRequest(new { message = "Ya existe otro cliente con ese DNI." });

        if (dto.IdAsesor.HasValue)
        {
            var asesorExiste = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .AnyAsync(u =>
                    u.IdUsuario == dto.IdAsesor.Value &&
                    u.Activo &&
                    !u.Eliminado &&
                    u.IdRolNavigation.Nombre == "Asesor");

            if (!asesorExiste)
                return BadRequest(new { message = "El asesor asignado no existe o no tiene rol Asesor." });
        }

        cliente.IdAsesor = dto.IdAsesor;
        cliente.Nombres = dto.Nombres.Trim();
        cliente.Apellidos = dto.Apellidos.Trim();
        cliente.Dni = dto.Dni.Trim();
        cliente.Correo = dto.Correo?.Trim().ToLower();
        cliente.Telefono = dto.Telefono?.Trim();
        cliente.Direccion = dto.Direccion?.Trim();
        cliente.EstadoCliente = dto.EstadoCliente.Trim();
        cliente.Riesgo = dto.Riesgo.Trim();
        cliente.Observacion = dto.Observacion?.Trim();
        cliente.FechaModificacion = DateTime.Now;
        cliente.UsuarioModificacion = User.Identity?.Name ?? "system";

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Cliente actualizado correctamente.",
            cliente
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> EliminarCliente(int id)
    {
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.IdCliente == id && c.Activo && !c.Eliminado);

        if (cliente == null)
            return NotFound(new { message = "Cliente no encontrado." });

        cliente.Activo = false;
        cliente.Eliminado = true;
        cliente.FechaModificacion = DateTime.Now;
        cliente.UsuarioModificacion = User.Identity?.Name ?? "system";

        await _context.SaveChangesAsync();

        return Ok(new { message = "Cliente eliminado correctamente." });
    }

    [HttpGet("mis-clientes")]
    [Authorize(Roles = "Asesor")]
    public async Task<IActionResult> ListarMisClientes([FromQuery] string? busqueda)
    {
        var idUsuarioClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(idUsuarioClaim))
            return Unauthorized(new { message = "No se pudo identificar al usuario autenticado." });

        var idAsesor = int.Parse(idUsuarioClaim);

        var query = _context.Clientes
            .Include(c => c.IdAsesorNavigation)
            .Where(c => c.Activo && !c.Eliminado && c.IdAsesor == idAsesor);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var texto = busqueda.Trim().ToLower();

            query = query.Where(c =>
                c.Nombres.ToLower().Contains(texto) ||
                c.Apellidos.ToLower().Contains(texto) ||
                (c.Correo != null && c.Correo.ToLower().Contains(texto)) ||
                c.Dni.ToLower().Contains(texto));
        }

        var clientes = await query
            .OrderBy(c => c.Apellidos)
            .ThenBy(c => c.Nombres)
            .Select(c => new
            {
                c.IdCliente,
                c.Nombres,
                c.Apellidos,
                c.Dni,
                c.Correo,
                c.Telefono,
                c.Direccion,
                c.EstadoCliente,
                c.Riesgo,
                Asesor = c.IdAsesorNavigation != null
                    ? c.IdAsesorNavigation.Nombres + " " + c.IdAsesorNavigation.Apellidos
                    : null
            })
            .ToListAsync();

        return Ok(clientes);
    }
    [HttpGet("mis-clientes/{id}")]
    [Authorize(Roles = "Asesor")]
    public async Task<IActionResult> ObtenerMiClienteDetalle(int id)
    {
        var idUsuarioClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(idUsuarioClaim))
            return Unauthorized(new { message = "No se pudo identificar al asesor autenticado." });

        var idAsesor = int.Parse(idUsuarioClaim);

        var cliente = await _context.Clientes
            .Include(c => c.IdAsesorNavigation)
            .Where(c =>
                c.IdCliente == id &&
                c.Activo &&
                !c.Eliminado &&
                c.IdAsesor == idAsesor)
            .Select(c => new
            {
                idCliente = c.IdCliente,
                nombreCompleto = c.Nombres + " " + c.Apellidos,
                correo = c.Correo,
                telefono = c.Telefono,
                fechaRegistro = c.FechaRegistro.ToString("dd MMM yyyy"),
                asesorActual = c.IdAsesorNavigation != null
                    ? c.IdAsesorNavigation.Nombres + " " + c.IdAsesorNavigation.Apellidos
                    : "-",
                deudaTotal = "S/. 15,000",
                deudaPendiente = "S/. 12,280",
                diasAtraso = 45,
                scoreRiesgo = 40,
                estadoActual = "Contactado",
                ultimoContacto = "14 mar 2026",
                proximoSeguimiento = "24 mar 2026",
                bitacora = new[]
                {
                new
                {
                    titulo = "Acuerdo",
                    descripcion = "Acuerdo de pago: 4 cuotas de S/. 3,750",
                    fecha = "15 mar 2026 10:45"
                },
                new
                {
                    titulo = "Llamada",
                    descripcion = "Contacto telefónico. Cliente solicita plan de pagos.",
                    fecha = "15 mar 2026 10:30"
                }
                }
            })
            .FirstOrDefaultAsync();

        if (cliente == null)
            return NotFound(new { message = "Cliente no encontrado o no pertenece al asesor." });

        return Ok(cliente);
    }
}