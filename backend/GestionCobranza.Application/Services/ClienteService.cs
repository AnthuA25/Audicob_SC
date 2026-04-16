using GestionCobranza.Application.DTOs;
using GestionCobranza.Application.Interfaces;
using GestionCobranza.Domain.Entities;

namespace GestionCobranza.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;
    public ClienteService(IClienteRepository repository) => _repository = repository;

    // HU-03: Listar todos
    public async Task<IEnumerable<ClienteDto>> ListarTodosAsync(string? filtro)
    {
        var clientes = await _repository.ObtenerTodosAsync(filtro);
        return clientes.Select(c => new ClienteDto(
            c.id_cliente,
            $"{c.nombres} {c.apellidos}",
            c.dni,
            c.correo ?? "",
            c.telefono ?? "",
            c.riesgo ?? "",
            c.estado_cliente ?? ""
        ));
    }

    // HU-11: Listar por Asesor
    public async Task<IEnumerable<ClienteDto>> ListarClientesPorAsesorAsync(int idAsesor, string? filtro)
    {
        var clientes = await _repository.ObtenerPorAsesorAsync(idAsesor, filtro);
        return clientes.Select(c => new ClienteDto(
            c.id_cliente,
            $"{c.nombres} {c.apellidos}",
            c.dni,
            c.correo ?? "",
            c.telefono ?? "",
            c.riesgo ?? "",
            c.estado_cliente ?? ""
        ));
    }

    // HU-04: Crear cliente
    public async Task<CrearClienteResponseDto> CrearClienteAsync(CrearClienteRequestDto request)
    {
        if (await _repository.ExisteDniAsync(request.Dni))
            throw new InvalidOperationException("Ya existe un cliente registrado con este DNI.");

        if (!await _repository.AsesorExisteYActivoAsync(request.IdAsesor))
            throw new InvalidOperationException("El asesor seleccionado no existe o no está activo.");

        var cliente = new Cliente
        {
            nombres          = request.Nombres.Trim(),
            apellidos        = request.Apellidos.Trim(),
            dni              = request.Dni.Trim(),
            correo           = request.Correo?.Trim(),
            telefono         = request.Telefono?.Trim(),
            id_asesor        = request.IdAsesor,
            deuda_total      = request.DeudaPendiente,
            saldo_pendiente  = request.DeudaPendiente,
            fecha_vencimiento = request.FechaPago,
            estado_cliente   = "Nuevo",
            riesgo           = "Bajo",
            activo           = true,
            eliminado        = false,
            fecha_registro   = DateTime.UtcNow,
            usuario_registro = "SISTEMA"
        };

        var creado = await _repository.CrearAsync(cliente);

        return new CrearClienteResponseDto
        {
            IdCliente = creado.id_cliente,
            Mensaje   = "Cliente registrado correctamente."
        };
    }

    // HU-04: Verificar DNI en tiempo real
    public async Task<bool> VerificarDniDisponibleAsync(string dni)
        => !await _repository.ExisteDniAsync(dni);

    // HU-04: Listar asesores activos para el selector
    public async Task<IEnumerable<AsesorDto>> ObtenerAsesoresActivosAsync()
    {
        var asesores = await _repository.ObtenerAsesoresActivosAsync();
        return asesores.Select(u => new AsesorDto(u.id, $"{u.nombres} {u.apellidos}"));
    }
}