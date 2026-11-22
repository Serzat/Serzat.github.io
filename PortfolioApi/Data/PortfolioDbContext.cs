
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Models;

namespace PortfolioApi.Data;

// Databasecontext voor projecten, blogposts en gebruikers.
public class PortfolioDbContext : IdentityDbContext<User>
{
    // Ontvangt de databaseconfiguratie via Dependency Injection.
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options)
        : base(options)
    {
    }

    // Tabel voor projecten.
    public DbSet<Project> Projecten => Set<Project>();

    // Tabel voor blogposts.
    public DbSet<Blogpost> Blogposts => Set<Blogpost>();

    // Configureert de Identity-tabellen en bestaande modellen.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
