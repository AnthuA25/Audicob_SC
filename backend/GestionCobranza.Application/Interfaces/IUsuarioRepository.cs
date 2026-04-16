using GestionCobranza.Domain.Entities;

namespace GestionCobranza.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<IEnumerable<Usuario>> ObtenerAsesoresAsync(string? filtro);
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteDniAsync(string dni, int? excluirId = null);
    Task<bool> ExisteCorreoAsync(string correo, int? excluirId = null);
    Task<Usuario> CrearAsync(Usuario usuario);
    Task<Usuario> ActualizarAsync(Usuario usuario);
    Task<bool> EliminarAsync(int id);
}
