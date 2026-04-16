using GestionCobranza.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionCobranza.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Pago>    Pagos    { get; set; }
}