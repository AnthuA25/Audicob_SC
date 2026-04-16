using GestionCobranza.Application.DTOs;

namespace GestionCobranza.Application.Interfaces;

public interface IClienteService
{
    Task<IEnumerable<ClienteDto>> ListarTodosAsync(string? filtro);
    Task<IEnumerable<ClienteDto>> ListarClientesPorAsesorAsync(int idAsesor, string? filtro);

    // HU-04: Crear cliente
    Task<CrearClienteResponseDto> CrearClienteAsync(CrearClienteRequestDto request);
    Task<bool> VerificarDniDisponibleAsync(string dni);
    Task<IEnumerable<AsesorDto>> ObtenerAsesoresActivosAsync();
}