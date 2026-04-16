using GestionCobranza.Application.DTOs;
using GestionCobranza.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestionCobranza.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _service;
    public ClientesController(IClienteService service) => _service = service;

    // HU-03: Lista General
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
        => Ok(await _service.ListarTodosAsync(search));

    // HU-11: Listado por Asesor
    [HttpGet("asesor/{idAsesor}")]
    public async Task<IActionResult> GetByAsesor(int idAsesor, [FromQuery] string? search)
    {
        var result = await _service.ListarClientesPorAsesorAsync(idAsesor, search);
        return Ok(result ?? new List<ClienteDto>());
    }

    // HU-04: Crear nuevo cliente
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearClienteRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var response = await _service.CrearClienteAsync(request);
            return CreatedAtAction(nameof(GetAll), new { }, response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // HU-04: Verificar disponibilidad de DNI en tiempo real
    [HttpGet("verificar-dni/{dni}")]
    public async Task<IActionResult> VerificarDni(string dni)
    {
        if (!System.Text.RegularExpressions.Regex.IsMatch(dni, @"^\d{8}$"))
            return BadRequest(new { mensaje = "El DNI debe tener exactamente 8 dígitos numéricos." });

        var disponible = await _service.VerificarDniDisponibleAsync(dni);
        return Ok(new { disponible, mensaje = disponible ? "DNI disponible." : "Ya existe un cliente registrado con este DNI." });
    }

    // HU-04: Obtener asesores activos para el selector del formulario
    [HttpGet("asesores-activos")]
    public async Task<IActionResult> GetAsesoresActivos()
        => Ok(await _service.ObtenerAsesoresActivosAsync());
}