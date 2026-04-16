using GestionCobranza.Application.Interfaces;
using GestionCobranza.Domain.Entities;
using GestionCobranza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionCobranza.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;
    public UsuarioRepository(ApplicationDbContext context) => _context = context;

    // Solo asesores (id_perfil = 2), no eliminados
    public async Task<IEnumerable<Usuario>> ObtenerAsesoresAsync(string? filtro)
    {
        var query = _context.Usuarios
            .Where(u => u.id_perfil == 2 && !u.eliminado)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            filtro = filtro.ToLower();
            query = query.Where(u =>
                u.nombres.ToLower().Contains(filtro) ||
                u.apellidos.ToLower().Contains(filtro) ||
                u.dni.Contains(filtro) ||
                u.correo.ToLower().Contains(filtro));
        }

        return await query.OrderBy(u => u.nombres).ToListAsync();
    }

    public async Task<Usuario?> ObtenerPorIdAsync(int id)
        => await _context.Usuarios.FirstOrDefaultAsync(u => u.id == id && !u.eliminado);

    // excluirId permite ignorar el propio registro al editar
    public async Task<bool> ExisteDniAsync(string dni, int? excluirId = null)
        => await _context.Usuarios.AnyAsync(u =>
            u.dni == dni && !u.eliminado &&
            (excluirId == null || u.id != excluirId));

    public async Task<bool> ExisteCorreoAsync(string correo, int? excluirId = null)
        => await _context.Usuarios.AnyAsync(u =>
            u.correo.ToLower() == correo.ToLower() && !u.eliminado &&
            (excluirId == null || u.id != excluirId));

    public async Task<Usuario> CrearAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<Usuario> ActualizarAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    // Soft delete: marca eliminado = true y activo = false
    public async Task<bool> EliminarAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario is null) return false;

        usuario.eliminado      = true;
        usuario.activo         = false;
        usuario.fecha_modifica = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}
