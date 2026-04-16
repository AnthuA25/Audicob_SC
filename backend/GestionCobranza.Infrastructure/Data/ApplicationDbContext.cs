using GestionCobranza.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionCobranza.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Deuda> Deudas { get; set; }
    public DbSet<Gestion> Gestiones { get; set; }
}