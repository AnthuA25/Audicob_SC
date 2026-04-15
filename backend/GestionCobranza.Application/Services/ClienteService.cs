using GestionCobranza.Application.DTOs;
using GestionCobranza.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionCobranza.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;
    public ClienteService(IClienteRepository repository) => _repository = repository;

    // HU03: Listar todos
    public async Task<IEnumerable<ClienteDto>> ListarTodosAsync(string? filtro)
    {
        var clientes = await _repository.ObtenerTodosAsync(filtro);
        return clientes.Select(c => new ClienteDto(
            c.id_cliente,
            $"{c.nombres} {c.apellidos}",
            c.dni,
            c.correo ?? "",
            c.telefono ?? "",      // 5: Telefono
            c.riesgo ?? "",        // 6: Riesgo
            c.estado_cliente ?? "" // 7: Estado (Aquí estaba el error)
        ));
    }

    // HU11: Listar por Asesor
    public async Task<IEnumerable<ClienteDto>> ListarClientesPorAsesorAsync(int idAsesor, string? filtro)
    {
        var clientes = await _repository.ObtenerPorAsesorAsync(idAsesor, filtro);
        return clientes.Select(c => new ClienteDto(
            c.id_cliente,
            $"{c.nombres} {c.apellidos}",
            c.dni,
            c.correo ?? "",
            c.telefono ?? "",      // 5: Telefono
            c.riesgo ?? "",        // 6: Riesgo
            c.estado_cliente ?? "" // 7: Estado
        ));
    }
}