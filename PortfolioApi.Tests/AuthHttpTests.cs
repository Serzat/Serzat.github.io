using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortfolioApi.DTOs;
using PortfolioApi.Models;
using Xunit;

namespace PortfolioApi.Tests;

public class AuthHttpTests
{
    // Test 1: Registratie via HTTP maakt een gebruiker met uitsluitend de rol Gast.
    [Fact]
    public async Task Register_NieuweGebruiker_KrijgtGastRol()
    {
        // Arrange
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var email = $"registratie-{Guid.NewGuid():N}@example.test";
        var request = new RegisterRequest
        {
            Email = email,
            Password = LeesTestWachtwoord()
        };

        try
        {
            // Act: gebruik de echte registratieroute en Identity-configuratie.
            var response = await client.PostAsJsonAsync("/api/auth/register", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var gebruiker = await userManager.FindByEmailAsync(email);
            Assert.NotNull(gebruiker);
            Assert.True(await userManager.IsInRoleAsync(gebruiker, "Gast"));
            Assert.False(await userManager.IsInRoleAsync(gebruiker, "Editor"));
            Assert.False(await userManager.IsInRoleAsync(gebruiker, "Admin"));
        }
        finally
        {
            var gebruiker = await userManager.FindByEmailAsync(email);
            if (gebruiker is not null)
                await userManager.DeleteAsync(gebruiker);
        }
    }

    // Test 2: Een ongeldig e-mailadres wordt bij registratie geweigerd.
    [Fact]
    public async Task Register_OngeldigEmail_Geeft400()
    {
        // Arrange
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();
        var request = new RegisterRequest
        {
            Email = "geen-geldig-emailadres",
            Password = LeesTestWachtwoord()
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", request);

        // Assert: [EmailAddress] op RegisterRequest leidt tot 400.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Test 3: Inloggen via HTTP geeft een cookie; /me herkent de gebruiker.
    [Fact]
    public async Task Login_JuisteGegevens_GeeftCookieEnMe200()
    {
        // Arrange
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var wachtwoord = LeesTestWachtwoord();
        var email = $"login-{Guid.NewGuid():N}@example.test";
        var gebruiker = new User { UserName = email, Email = email };

        try
        {
            var gemaakt = await userManager.CreateAsync(gebruiker, wachtwoord);
            Assert.True(gemaakt.Succeeded);
            var rol = await userManager.AddToRoleAsync(gebruiker, "Gast");
            Assert.True(rol.Succeeded);

            // Act
            var response = await client.PostAsJsonAsync(
                "/api/auth/login", new { Email = email, Password = wachtwoord });

            // Assert: login werkt en levert een authentication cookie op.
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.TryGetValues("Set-Cookie", out var cookies));
            Assert.NotEmpty(cookies!);

            // Dezelfde HTTP-client stuurt de cookie automatisch mee.
            var meResponse = await client.GetAsync("/api/auth/me");
            Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
            using var json = JsonDocument.Parse(await meResponse.Content.ReadAsStringAsync());
            Assert.Equal(email, json.RootElement.GetProperty("email").GetString());
        }
        finally
        {
            var bestaandeGebruiker = await userManager.FindByEmailAsync(email);
            if (bestaandeGebruiker is not null)
                await userManager.DeleteAsync(bestaandeGebruiker);
        }
    }

    // Test 4: Een verkeerd wachtwoord wordt geweigerd en geeft geen sessie.
    [Fact]
    public async Task Login_VerkeerdWachtwoord_Geeft401()
    {
        // Arrange
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var wachtwoord = LeesTestWachtwoord();
        var email = $"foutlogin-{Guid.NewGuid():N}@example.test";
        var gebruiker = new User { UserName = email, Email = email };

        try
        {
            var gemaakt = await userManager.CreateAsync(gebruiker, wachtwoord);
            Assert.True(gemaakt.Succeeded);

            // Act
            var response = await client.PostAsJsonAsync(
                "/api/auth/login",
                new { Email = email, Password = "VerkeerdWachtwoord123!" });

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            var meResponse = await client.GetAsync("/api/auth/me");
            Assert.Equal(HttpStatusCode.Unauthorized, meResponse.StatusCode);
        }
        finally
        {
            var bestaandeGebruiker = await userManager.FindByEmailAsync(email);
            if (bestaandeGebruiker is not null)
                await userManager.DeleteAsync(bestaandeGebruiker);
        }
    }

    private static string LeesTestWachtwoord()
    {
        var configuratie = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        return configuratie["TestData:Password"]
            ?? throw new InvalidOperationException("TestData:Password ontbreekt in User Secrets.");
    }
}
