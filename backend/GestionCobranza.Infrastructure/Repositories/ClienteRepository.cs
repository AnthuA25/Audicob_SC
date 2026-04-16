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
        return await _context.Clientes
            .Where(c => !c.eliminado) // Solo mostramos los que no han sido dados de baja
            .ToListAsync();
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteLogicoAsync(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente != null)
        {
            cliente.eliminado = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.id_cliente == id && !c.eliminado);
    }

    // HU11 - Listado por Asesor
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
}