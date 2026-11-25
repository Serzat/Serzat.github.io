
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PortfolioApi.Data;

namespace PortfolioApi.Tests;

// Start de API met een aparte testdatabase.
public class PortfolioApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Lees de testverbinding uit de User Secrets van de API.
        var configuratie = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var verbinding = configuratie
            .GetConnectionString("PortfolioDb_Test");

        // Stop als er geen testverbinding is ingesteld.
        if (string.IsNullOrWhiteSpace(verbinding))
        {
            throw new InvalidOperationException(
                "De testdatabaseverbinding ontbreekt.");
        }

        // Controleer of de juiste database wordt gebruikt.
        var databaseNaam =
            new SqlConnectionStringBuilder(verbinding).InitialCatalog;

        if (databaseNaam != "PortfolioDb_Test")
        {
            throw new InvalidOperationException(
                "Tests mogen alleen PortfolioDb_Test gebruiken.");
        }

        // Vervang de normale databaseconfiguratie.
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<
                IDbContextOptionsConfiguration<PortfolioDbContext>>();

            services.RemoveAll<
                DbContextOptions<PortfolioDbContext>>();

            // Gebruik SQL Server met de testdatabase.
            services.AddDbContext<PortfolioDbContext>(options =>
                options.UseSqlServer(verbinding));
        });
    }
}
