using GestionCobranza.Application.DTOs;

namespace GestionCobranza.Application.Interfaces;

public interface IClienteService
{
    Task<IEnumerable<ClienteDto>> ListarTodosAsync(string? filtro);
    // Este es el método que necesitamos para la HU-11
    Task<IEnumerable<ClienteDto>> ListarClientesPorAsesorAsync(int idAsesor, string? filtro);
    Task<bool> EditarClienteAsync(int id, ClienteDto dto);
    Task<bool> EliminarClienteAsync(int id);
}