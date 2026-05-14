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
            .Include(c => c.Deuda)
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
                    : null,

                DeudaTotal = c.Deuda
                .Where(d => d.Activo && !d.Eliminado)
                .Sum(d => d.MontoTotal),
                
                DeudaPendiente = c.Deuda
                .Where(d => d.Activo && !d.Eliminado)
                .Sum(d => d.SaldoPendiente),


                DiasAtraso = c.Deuda
                .Where(d => d.Activo && !d.Eliminado)
                .Any()
                    ? c.Deuda
                        .Where(d => d.Activo && !d.Eliminado)
                        .Max(d => d.DiasAtraso)
                    : 0,
                FechaUltimoPago = c.Deuda
                .SelectMany(d => d.Pagos)
                .Where(p => p.Activo && !p.Eliminado)
                .OrderByDescending(p => p.FechaPago)
                .Select(p => p.FechaPago)
                .FirstOrDefault()
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

        if (dto.MontoDeuda.HasValue && dto.MontoDeuda.Value <= 0)
            return BadRequest(new { message = "El monto de deuda debe ser mayor a 0." });

        if (dto.FechaVencimiento.HasValue && dto.FechaEmision.HasValue &&
            dto.FechaVencimiento.Value < dto.FechaEmision.Value)
        {
            return BadRequest(new { message = "La fecha de vencimiento no puede ser menor a la fecha de emisión." });
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var fechaActual = DateTime.Now;

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
                FechaRegistro = fechaActual,
                UsuarioRegistro = User.Identity?.Name ?? "system",
                Activo = true,
                Eliminado = false
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            Deudum? deuda = null;

            if (dto.MontoDeuda.HasValue)
            {
                var fechaEmision = dto.FechaEmision ?? DateOnly.FromDateTime(DateTime.Today);
                var fechaVencimiento = dto.FechaVencimiento ?? fechaEmision.AddDays(30);

                var hoy = DateOnly.FromDateTime(DateTime.Today);
                var diasAtraso = hoy > fechaVencimiento
                    ? hoy.DayNumber - fechaVencimiento.DayNumber
                    : 0;

                deuda = new Deudum
                {
                    IdCliente = cliente.IdCliente,
                    MontoTotal = dto.MontoDeuda.Value,
                    MontoPagado = 0,
                    SaldoPendiente = dto.MontoDeuda.Value,
                    FechaEmision = fechaEmision,
                    FechaVencimiento = fechaVencimiento,
                    DiasAtraso = diasAtraso,
                    EstadoDeuda = diasAtraso > 0 ? "VENCIDA" : "PENDIENTE",
                    Descripcion = dto.DescripcionDeuda?.Trim(),
                    FechaRegistro = fechaActual,
                    UsuarioRegistro = User.Identity?.Name ?? "system",
                    Activo = true,
                    Eliminado = false
                };

                _context.Deuda.Add(deuda);
                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();

            return Ok(new
            {
                message = deuda == null
                    ? "Cliente registrado correctamente."
                    : "Cliente y deuda registrados correctamente.",
                cliente = new
                {
                    cliente.IdCliente,
                    cliente.Nombres,
                    cliente.Apellidos,
                    cliente.Dni,
                    cliente.Correo,
                    cliente.Telefono,
                    cliente.Direccion,
                    cliente.EstadoCliente,
                    cliente.Riesgo
                },
                deuda = deuda == null ? null : new
                {
                    deuda.IdDeuda,
                    deuda.MontoTotal,
                    deuda.MontoPagado,
                    deuda.SaldoPendiente,
                    deuda.FechaEmision,
                    deuda.FechaVencimiento,
                    deuda.DiasAtraso,
                    deuda.EstadoDeuda
                }
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            return StatusCode(500, new
            {
                message = "Error al registrar cliente.",
                detalle = ex.Message,
                interno = ex.InnerException?.Message
            });
        }
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

        if (string.IsNullOrWhiteSpace(idUsuarioClaim) || !int.TryParse(idUsuarioClaim, out var idAsesor))
            return Unauthorized(new { message = "No se pudo identificar al usuario autenticado." });


        var query = _context.Clientes
            .Include(c => c.IdAsesorNavigation)
            .Include(c => c.Deuda)
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
                    : null,
                DeudaPendiente = c.Deuda
                .Where(d => d.Activo && !d.Eliminado)
                .Sum(d => d.SaldoPendiente),

                DiasAtraso = c.Deuda
                .Where(d => d.Activo && !d.Eliminado)
                .Any()
                    ? c.Deuda
                        .Where(d => d.Activo && !d.Eliminado)
                        .Max(d => d.DiasAtraso)
                    : 0
            })
            .ToListAsync();

        return Ok(clientes);
    }
    [HttpGet("mis-clientes/{id}")]
    [Authorize(Roles = "Asesor")]
    public async Task<IActionResult> ObtenerMiClienteDetalle(int id)
    {
        var idUsuarioClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(idUsuarioClaim) || !int.TryParse(idUsuarioClaim, out var idAsesor))
        {
            return Unauthorized(new
            {
                message = "No se pudo identificar al asesor autenticado."
            });
        }

        var cliente = await _context.Clientes
            .Include(c => c.IdAsesorNavigation)
            .Include(c => c.Deuda)
                .ThenInclude(d => d.Pagos)
            .Include(c => c.GestionCobranzas)
                .ThenInclude(g => g.IdUsuarioNavigation)
            .FirstOrDefaultAsync(c =>
                c.IdCliente == id &&
                c.Activo &&
                !c.Eliminado &&
                c.IdAsesor == idAsesor);

        if (cliente == null)
        {
            return NotFound(new
            {
                message = "Cliente no encontrado o no pertenece al asesor."
            });
        }

        var deudasActivas = cliente.Deuda
            .Where(d => d.Activo && !d.Eliminado)
            .ToList();

        var deudaTotal = deudasActivas.Sum(d => d.MontoTotal);
        var deudaPendiente = deudasActivas.Sum(d => d.SaldoPendiente);
        var montoPagado = deudasActivas.Sum(d => d.MontoPagado);

        var diasAtraso = deudasActivas.Any()
            ? deudasActivas.Max(d => d.DiasAtraso)
            : 0;

        var deudas = deudasActivas
         .OrderByDescending(d => d.FechaVencimiento)
         .Select(d => new
         {
             idDeuda = d.IdDeuda,
             montoTotal = d.MontoTotal,
             montoPagado = d.MontoPagado,
             saldoPendiente = d.SaldoPendiente,
             fechaEmision = d.FechaEmision.ToString("dd MMM yyyy"),
             fechaVencimiento = d.FechaVencimiento.ToString("dd MMM yyyy"),
             diasAtraso = d.DiasAtraso,
             estadoDeuda = d.EstadoDeuda,
             descripcion = d.Descripcion
         })
         .ToList();

        var bitacora = cliente.GestionCobranzas
            .Where(g => g.Activo && !g.Eliminado)
            .OrderByDescending(g => g.FechaGestion)
            .Select(g => new
            {
                idGestion = g.IdGestion,
                tipoGestion = g.TipoGestion,
                titulo = g.TipoGestion,
                descripcion = g.Descripcion,
                resultado = g.Resultado,
                fechaGestion = g.FechaGestion,
                fechaTexto = g.FechaGestion.ToString("dd MMM yyyy HH:mm"),
                proximaAccion = g.ProximaAccion != null
                    ? g.ProximaAccion.Value.ToString("dd MMM yyyy")
                    : null,
                asesor = g.IdUsuarioNavigation != null
                    ? g.IdUsuarioNavigation.Nombres + " " + g.IdUsuarioNavigation.Apellidos
                    : "-"
            })
            .ToList();

        var historialPagos = deudasActivas
            .SelectMany(d => d.Pagos)
            .Where(p => p.Activo && !p.Eliminado)
            .OrderByDescending(p => p.FechaPago)
            .Select(p => new
            {
                idPago = p.IdPago,
                idDeuda = p.IdDeuda,
                monto = p.Monto,
                fechaPago = p.FechaPago,
                fechaTexto = p.FechaPago.ToString("dd MMM yyyy"),
                metodoPago = p.MetodoPago,
                comprobanteUrl = p.ComprobanteUrl,
                nota = p.Nota,
                estadoPago = p.EstadoPago
            })
            .ToList();

        var timelineGestiones = cliente.GestionCobranzas
            .Where(g => g.Activo && !g.Eliminado)
            .Select(g => new
            {
                tipo = "GESTION",
                titulo = g.TipoGestion,
                descripcion = g.Descripcion,
                fechaOrden = g.FechaGestion,
                fecha = g.FechaGestion.ToString("dd MMM yyyy HH:mm")
            })
            .ToList();

        var timelinePagos = deudasActivas
            .SelectMany(d => d.Pagos)
            .Where(p => p.Activo && !p.Eliminado)
            .Select(p => new
            {
                tipo = "PAGO",
                titulo = "Pago registrado",
                descripcion = $"Pago de S/. {p.Monto} por {p.MetodoPago}",
                fechaOrden = p.FechaPago.ToDateTime(TimeOnly.MinValue),
                fecha = p.FechaPago.ToString("dd MMM yyyy")
            })
            .ToList();

        var timeline = timelineGestiones
            .Concat(timelinePagos)
            .OrderByDescending(t => t.fechaOrden)
            .Select(t => new
            {
                t.tipo,
                t.titulo,
                t.descripcion,
                t.fecha
            })
            .ToList();

        var ultimoContacto = cliente.GestionCobranzas
            .Where(g => g.Activo && !g.Eliminado)
            .OrderByDescending(g => g.FechaGestion)
            .Select(g => g.FechaGestion.ToString("dd MMM yyyy"))
            .FirstOrDefault();

        var proximoSeguimiento = cliente.GestionCobranzas
            .Where(g => g.Activo && !g.Eliminado && g.ProximaAccion != null)
            .OrderBy(g => g.ProximaAccion)
            .Select(g => g.ProximaAccion!.Value.ToString("dd MMM yyyy"))
            .FirstOrDefault();

        return Ok(new
        {
            idCliente = cliente.IdCliente,
            nombreCompleto = cliente.Nombres + " " + cliente.Apellidos,
            dni = cliente.Dni,
            correo = cliente.Correo,
            telefono = cliente.Telefono,
            direccion = cliente.Direccion,
            fechaRegistro = cliente.FechaRegistro.ToString("dd MMM yyyy"),

            asesorActual = cliente.IdAsesorNavigation != null
                ? cliente.IdAsesorNavigation.Nombres + " " + cliente.IdAsesorNavigation.Apellidos
                : "-",

            estadoActual = cliente.EstadoCliente,
            riesgo = cliente.Riesgo,
            observacion = cliente.Observacion,

            deudaTotal = deudaTotal,
            deudaPendiente = deudaPendiente,
            montoPagado = montoPagado,
            diasAtraso = diasAtraso,

            ultimoContacto = ultimoContacto,
            proximoSeguimiento = proximoSeguimiento,

            deudas = deudas,
            bitacora = bitacora,
            historialPagos = historialPagos,
            timeline = timeline
        });
    }
}