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

    public async Task<bool> EditarClienteAsync(int id, ClienteDto dto)
    {
        var cliente = await _repository.ObtenerPorIdAsync(id);
        if (cliente == null) return false;

        // Actualizamos los campos permitidos según la HU03
        cliente.dni = dto.Dni;
        cliente.correo = dto.Correo;
        cliente.telefono = dto.Telefono;
        cliente.riesgo = dto.Riesgo;
        cliente.estado_cliente = dto.Estado;

        cliente.fecha_modificacion = DateTime.UtcNow;

        await _repository.UpdateAsync(cliente);
        return true;
    }

    public async Task<bool> EliminarClienteAsync(int id)
    {
        var cliente = await _repository.ObtenerPorIdAsync(id);
        if (cliente == null) return false;

        await _repository.DeleteLogicoAsync(id);
        return true;
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