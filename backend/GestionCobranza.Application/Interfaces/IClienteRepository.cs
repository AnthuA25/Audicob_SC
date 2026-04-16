using GestionCobranza.Domain.Entities;

namespace GestionCobranza.Application.Interfaces;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> ObtenerTodosAsync(string? filtro);
    Task<IEnumerable<Cliente>> ObtenerPorAsesorAsync(int idAsesor, string? filtro);
    Task UpdateAsync(Cliente cliente);
    Task DeleteLogicoAsync(int id);
    Task<Cliente?> ObtenerPorIdAsync(int id);

}