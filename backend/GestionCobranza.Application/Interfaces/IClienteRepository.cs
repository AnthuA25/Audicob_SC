using GestionCobranza.Domain.Entities;

namespace GestionCobranza.Application.Interfaces;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> ObtenerTodosAsync(string? filtro);
}