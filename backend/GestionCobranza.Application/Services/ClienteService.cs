using GestionCobranza.Application.DTOs;
using GestionCobranza.Application.Interfaces;

namespace GestionCobranza.Application.Services;

public class ClienteService
{
    private readonly IClienteRepository _repository;

    public ClienteService(IClienteRepository repository) => _repository = repository;

    public async Task<IEnumerable<ClienteDto>> ListarClientesAsync(string? filtro)
    {
        var clientes = await _repository.ObtenerTodosAsync(filtro);
        return clientes.Select(c => new ClienteDto(
            c.id_cliente, 
            $"{c.nombres} {c.apellidos}", 
            c.dni, 
            c.correo ?? "Sin correo", 
            c.riesgo, 
            c.estado_cliente));
    }
}