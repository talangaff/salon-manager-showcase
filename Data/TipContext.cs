using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using salon_manager.Models;

namespace salon_manager.Data;

public class TipContext : IdentityDbContext<ApplicationUser>
{
    public TipContext(DbContextOptions<TipContext> options) : base(options) { }

    public DbSet<Tip> Tips { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<ClientColor> ClientColors { get; set; }
    public DbSet<ClientToner> ClientToners { get; set; }
}
