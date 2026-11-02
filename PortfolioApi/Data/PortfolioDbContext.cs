
using Microsoft.EntityFrameworkCore; 
using PortfolioApi.Models; 

// Defines the namespace for the data layer.
namespace PortfolioApi.Data;

// Creates our database context by inheriting from DbContext.
public class PortfolioDbContext : DbContext
{
    // Constructor that receives database configuration through Dependency Injection.
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options)
        // Passes the configuration options to the base DbContext class.
        : base(options)
    {
    }
        

    // Provides access to Project records in the database.
    public DbSet<Project> Projecten => Set<Project>();

    // Provides access to Blogpost records in the database.
    public DbSet<Blogpost> Blogposts => Set<Blogpost>();
}
