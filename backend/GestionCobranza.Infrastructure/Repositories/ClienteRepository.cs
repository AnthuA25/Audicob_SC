using GestionCobranza.Application.Interfaces;
using GestionCobranza.Domain.Entities;
using GestionCobranza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionCobranza.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly ApplicationDbContext _context;
    public ClienteRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<Cliente>> ObtenerTodosAsync(string? filtro)
    {
        var query = _context.Clientes.Where(c => !c.eliminado && c.activo).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            filtro = filtro.ToLower();
            query = query.Where(c => c.nombres.ToLower().Contains(filtro) ||
                                     c.apellidos.ToLower().Contains(filtro) ||
                                     c.dni.Contains(filtro));
        }
        return await query.ToListAsync();
    }

    // HU-11: Listado por Asesor
    public async Task<IEnumerable<Cliente>> ObtenerPorAsesorAsync(int idAsesor, string? filtro)
    {
        var query = _context.Clientes
            .Where(c => c.id_asesor == idAsesor && !c.eliminado && c.activo)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            filtro = filtro.ToLower();
            query = query.Where(c => c.nombres.ToLower().Contains(filtro) ||
                                     c.apellidos.ToLower().Contains(filtro) ||
                                     c.dni.Contains(filtro));
        }

        return await query.ToListAsync();
    }

    // HU-04: Verificar si ya existe un DNI registrado (excluye eliminados)
    public async Task<bool> ExisteDniAsync(string dni)
        => await _context.Clientes.AnyAsync(c => c.dni == dni && !c.eliminado);

    // HU-04: Verificar que el asesor existe y está activo
    public async Task<bool> AsesorExisteYActivoAsync(int idAsesor)
        => await _context.Usuarios.AnyAsync(u => u.id == idAsesor && u.activo && !u.eliminado);

    // HU-04: Crear nuevo cliente
    public async Task<Cliente> CrearAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }

    // HU-04: Listar asesores activos para el selector del formulario
    public async Task<IEnumerable<Usuario>> ObtenerAsesoresActivosAsync()
        => await _context.Usuarios
            .Where(u => u.activo && !u.eliminado)
            .OrderBy(u => u.nombres)
            .ToListAsync();
}