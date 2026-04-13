using GestionCobranza.Application.DTOs; // Necesario para reconocer ClienteDto
using GestionCobranza.Application.Interfaces; // Para usar IClienteService
using Microsoft.AspNetCore.Mvc;

namespace GestionCobranza.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    // Usamos la interfaz IClienteService para que coincida con tu inyección de dependencias
    private readonly IClienteService _service;
    public ClientesController(IClienteService service) => _service = service;

    // HU03 - Lista General
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
        // Cambiado a ListarTodosAsync para que coincida con tu Service corregido
        => Ok(await _service.ListarTodosAsync(search));

    // HU11 - Listado por Asesor
    [HttpGet("asesor/{idAsesor}")]
    public async Task<IActionResult> GetByAsesor(int idAsesor, [FromQuery] string? search)
    {
        var result = await _service.ListarClientesPorAsesorAsync(idAsesor, search);

        if (result == null || !result.Any())
        {
            // Se usa List<ClienteDto> porque ya agregamos el using arriba
            return Ok(new List<ClienteDto>());
        }

        return Ok(result);
    }
}