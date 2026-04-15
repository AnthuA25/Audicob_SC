using GestionCobranza.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionCobranza.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ClienteService _service;
    public ClientesController(ClienteService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search) 
        => Ok(await _service.ListarClientesAsync(search));
}