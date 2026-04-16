using GestionCobranza.Application.DTOs;
using GestionCobranza.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestionCobranza.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AsesoresController : ControllerBase
{
    private readonly IUsuarioService _service;
    public AsesoresController(IUsuarioService service) => _service = service;

    // HU-06: Listar todos los asesores
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
        => Ok(await _service.ListarAsesoresAsync(search));

    // HU-06: Crear nuevo asesor
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearAsesorRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var response = await _service.CrearAsesorAsync(request);
            return CreatedAtAction(nameof(GetAll), new { }, response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // HU-06: Editar asesor existente
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, [FromBody] EditarAsesorRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _service.EditarAsesorAsync(id, request);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // HU-06: Eliminar asesor (soft delete)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            await _service.EliminarAsesorAsync(id);
            return Ok(new { mensaje = "Asesor eliminado correctamente." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    // HU-06: Verificar disponibilidad de DNI en tiempo real
    [HttpGet("verificar-dni/{dni}")]
    public async Task<IActionResult> VerificarDni(string dni, [FromQuery] int? excluirId = null)
    {
        if (!System.Text.RegularExpressions.Regex.IsMatch(dni, @"^\d{8}$"))
            return BadRequest(new { mensaje = "El DNI debe tener exactamente 8 dígitos numéricos." });

        var disponible = await _service.VerificarDniDisponibleAsync(dni, excluirId);
        return Ok(new
        {
            disponible,
            mensaje = disponible
                ? "DNI disponible."
                : "Ya existe un usuario registrado con este DNI."
        });
    }

    // HU-06: Verificar disponibilidad de email en tiempo real
    [HttpGet("verificar-email/{email}")]
    public async Task<IActionResult> VerificarEmail(string email, [FromQuery] int? excluirId = null)
    {
        var disponible = await _service.VerificarCorreoDisponibleAsync(email, excluirId);
        return Ok(new
        {
            disponible,
            mensaje = disponible
                ? "Email disponible."
                : "Ya existe un usuario registrado con este email."
        });
    }
}
