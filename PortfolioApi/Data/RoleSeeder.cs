
using Microsoft.AspNetCore.Identity;

namespace PortfolioApi.Data;

// Deze klasse maakt de standaardrollen aan in de database.
public static class RoleSeeder
{
    // Controleert bij het opstarten of de rollen bestaan.
    public static async Task SeedAsync(IServiceProvider services)
    {
        // Maak een scope aan om Identity-services te gebruiken.
        await using var scope = services.CreateAsyncScope();

        // Haal RoleManager op via Dependency Injection.
        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        // De drie rollen die onze applicatie gebruikt.
        string[] rollen = { "Gast", "Editor", "Admin" };

        // Loop door alle rollen.
        foreach (var rol in rollen)
        {
            // Controleer of de rol al in de database bestaat.
            if (!await roleManager.RoleExistsAsync(rol))
            {
                // Maak de rol aan als deze nog niet bestaat.
                var resultaat = await roleManager.CreateAsync(
                    new IdentityRole(rol));

                // Geef een foutmelding als het aanmaken mislukt.
                if (!resultaat.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Rol '{rol}' kon niet worden aangemaakt.");
                }
            }
        }
    }
}
