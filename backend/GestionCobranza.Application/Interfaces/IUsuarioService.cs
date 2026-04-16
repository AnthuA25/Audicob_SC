using GestionCobranza.Application.DTOs;

namespace GestionCobranza.Application.Interfaces;

public interface IUsuarioService
{
    // HU-06: Crear asesor
    Task<CrearAsesorResponseDto> CrearAsesorAsync(CrearAsesorRequestDto request);

    // HU-06: Listar asesores
    Task<IEnumerable<AsesorDetalleDto>> ListarAsesoresAsync(string? filtro);

    // HU-06: Editar asesor
    Task<AsesorDetalleDto> EditarAsesorAsync(int id, EditarAsesorRequestDto request);

    // HU-06: Eliminar asesor (soft delete)
    Task EliminarAsesorAsync(int id);

    // HU-06: Verificaciones en tiempo real
    Task<bool> VerificarDniDisponibleAsync(string dni, int? excluirId = null);
    Task<bool> VerificarCorreoDisponibleAsync(string correo, int? excluirId = null);
}
