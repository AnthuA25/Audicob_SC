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
}