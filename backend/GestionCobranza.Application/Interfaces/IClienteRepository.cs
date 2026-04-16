using GestionCobranza.Domain.Entities;

namespace GestionCobranza.Application.Interfaces;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> ObtenerTodosAsync(string? filtro);
    Task<IEnumerable<Cliente>> ObtenerPorAsesorAsync(int idAsesor, string? filtro);

    // HU-04: Crear cliente
    Task<bool> ExisteDniAsync(string dni);
    Task<bool> AsesorExisteYActivoAsync(int idAsesor);
    Task<Cliente> CrearAsync(Cliente cliente);
    Task<IEnumerable<Usuario>> ObtenerAsesoresActivosAsync();
}