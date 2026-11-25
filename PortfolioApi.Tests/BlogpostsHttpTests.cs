using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortfolioApi.Data;
using PortfolioApi.DTOs;
using PortfolioApi.Models;
using Xunit;

namespace PortfolioApi.Tests;

public class BlogpostsHttpTests
{
    // Test 1: Iedereen mag de lijst met blogposts bekijken.
    [Fact]
    public async Task GetBlogposts_ZonderLogin_Geeft200()
    {
        // Arrange
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/blogposts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // Test 2: Een bestaand ID geeft de juiste blogpost terug.
    [Fact]
    public async Task GetBlogpost_BestaandId_GeeftJuisteBlogpost()
    {
        // Arrange
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();

        var blogpost = new Blogpost
        {
            Titel = "Blogpost GET-test",
            Inhoud = "Inhoud van een tijdelijke testblogpost.",
            Publicatiedatum = new DateTime(2026, 11, 25)
        };

        db.Blogposts.Add(blogpost);
        await db.SaveChangesAsync();

        try
        {
            // Act
            var response = await client.GetAsync($"/api/blogposts/{blogpost.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var resultaat = await response.Content.ReadFromJsonAsync<BlogpostResponse>();
            Assert.NotNull(resultaat);
            Assert.Equal(blogpost.Id, resultaat.Id);
            Assert.Equal(blogpost.Titel, resultaat.Titel);
            Assert.Equal(blogpost.Inhoud, resultaat.Inhoud);
        }
        finally
        {
            await VerwijderBlogpostAsync(db, blogpost.Id);
        }
    }

    // Test 3: Een onbekend ID geeft 404 terug.
    [Fact]
    public async Task GetBlogpost_OnbekendId_Geeft404()
    {
        // Arrange
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/blogposts/-1");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // Test 4: Een Editor mag een blogpost aanmaken; de data wordt opgeslagen.
    [Fact]
    public async Task CreateBlogpost_AlsEditor_Geeft201_EnSlaatOp()
    {
        // Arrange
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var wachtwoord = LeesTestWachtwoord();
        var gebruiker = await MaakGebruikerMetRolAsync(userManager, "Editor", wachtwoord);
        int? blogpostId = null;

        try
        {
            await LoginAsync(client, gebruiker, wachtwoord);

            var request = new CreateBlogpostRequest
            {
                Titel = "Blogpost POST-test",
                Inhoud = "De Editor maakt deze blogpost via HTTP aan.",
                Publicatiedatum = new DateTime(2026, 11, 25)
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/blogposts", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var resultaat = await response.Content.ReadFromJsonAsync<BlogpostResponse>();
            Assert.NotNull(resultaat);
            blogpostId = resultaat.Id;
            Assert.Equal(request.Titel, resultaat.Titel);

            // Controleer de database, niet alleen de HTTP-status.
            var opgeslagen = await db.Blogposts.AsNoTracking()
                .SingleOrDefaultAsync(b => b.Id == blogpostId.Value);
            Assert.NotNull(opgeslagen);
            Assert.Equal(request.Inhoud, opgeslagen.Inhoud);
        }
        finally
        {
            if (blogpostId.HasValue)
                await VerwijderBlogpostAsync(db, blogpostId.Value);
            await userManager.DeleteAsync(gebruiker);
        }
    }

    // Test 5: Zonder login mag niemand een blogpost aanmaken.
    [Fact]
    public async Task CreateBlogpost_ZonderLogin_Geeft401_EnSlaatNietsOp()
    {
        // Arrange
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        var uniekeTitel = $"Niet opslaan {Guid.NewGuid():N}";
        var request = new CreateBlogpostRequest
        {
            Titel = uniekeTitel,
            Inhoud = "Deze blogpost mag niet worden opgeslagen.",
            Publicatiedatum = new DateTime(2026, 11, 25)
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.False(await db.Blogposts.AnyAsync(b => b.Titel == uniekeTitel));
    }

    // Test 6: Een ingelogde Gast heeft geen schrijfrecht.
    [Fact]
    public async Task CreateBlogpost_AlsGast_Geeft403_EnSlaatNietsOp()
    {
        // Arrange
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var wachtwoord = LeesTestWachtwoord();
        var gebruiker = await MaakGebruikerMetRolAsync(userManager, "Gast", wachtwoord);

        try
        {
            await LoginAsync(client, gebruiker, wachtwoord);
            var uniekeTitel = $"Gast mag niet schrijven {Guid.NewGuid():N}";
            var request = new CreateBlogpostRequest
            {
                Titel = uniekeTitel,
                Inhoud = "Dit verzoek moet worden geweigerd.",
                Publicatiedatum = new DateTime(2026, 11, 25)
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/blogposts", request);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.False(await db.Blogposts.AnyAsync(b => b.Titel == uniekeTitel));
        }
        finally
        {
            await userManager.DeleteAsync(gebruiker);
        }
    }

    // Test 7: Een Editor kan een blogpost via HTTP bijwerken.
    [Fact]
    public async Task UpdateBlogpost_AlsEditor_Geeft204_EnWijzigtData()
    {
        // Arrange
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var wachtwoord = LeesTestWachtwoord();
        var gebruiker = await MaakGebruikerMetRolAsync(userManager, "Editor", wachtwoord);
        int? blogpostId = null;

        try
        {
            var blogpost = new Blogpost
            {
                Titel = "Oorspronkelijke titel",
                Inhoud = "Oorspronkelijke inhoud",
                Publicatiedatum = new DateTime(2026, 11, 25)
            };
            db.Blogposts.Add(blogpost);
            await db.SaveChangesAsync();
            blogpostId = blogpost.Id;
            await LoginAsync(client, gebruiker, wachtwoord);

            var request = new UpdateBlogpostRequest
            {
                Titel = "Bijgewerkte blogpost",
                Inhoud = "Nieuwe inhoud na de PUT-aanvraag.",
                Publicatiedatum = new DateTime(2026, 11, 26)
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/blogposts/{blogpostId.Value}", request);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            var bijgewerkt = await db.Blogposts.AsNoTracking()
                .SingleOrDefaultAsync(b => b.Id == blogpostId.Value);
            Assert.NotNull(bijgewerkt);
            Assert.Equal(request.Titel, bijgewerkt.Titel);
            Assert.Equal(request.Inhoud, bijgewerkt.Inhoud);
            Assert.Equal(request.Publicatiedatum, bijgewerkt.Publicatiedatum);
        }
        finally
        {
            if (blogpostId.HasValue)
                await VerwijderBlogpostAsync(db, blogpostId.Value);
            await userManager.DeleteAsync(gebruiker);
        }
    }

    // Test 8: Een Admin kan een blogpost verwijderen; GET geeft daarna 404.
    [Fact]
    public async Task DeleteBlogpost_AlsAdmin_Geeft204_EnVerwijdertData()
    {
        // Arrange
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var wachtwoord = LeesTestWachtwoord();
        var gebruiker = await MaakGebruikerMetRolAsync(userManager, "Admin", wachtwoord);
        int? blogpostId = null;

        try
        {
            var blogpost = new Blogpost
            {
                Titel = "Blogpost DELETE-test",
                Inhoud = "Deze blogpost wordt verwijderd.",
                Publicatiedatum = new DateTime(2026, 11, 25)
            };
            db.Blogposts.Add(blogpost);
            await db.SaveChangesAsync();
            blogpostId = blogpost.Id;
            await LoginAsync(client, gebruiker, wachtwoord);

            // Act
            var response = await client.DeleteAsync($"/api/blogposts/{blogpostId.Value}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.False(await db.Blogposts.AsNoTracking()
                .AnyAsync(b => b.Id == blogpostId.Value));

            var getResponse = await client.GetAsync($"/api/blogposts/{blogpostId.Value}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
        finally
        {
            if (blogpostId.HasValue)
                await VerwijderBlogpostAsync(db, blogpostId.Value);
            await userManager.DeleteAsync(gebruiker);
        }
    }

    // Hergebruik dezelfde testvoorbereiding, zodat de tests overzichtelijk blijven.
    private static string LeesTestWachtwoord()
    {
        var configuratie = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        return configuratie["TestData:Password"]
            ?? throw new InvalidOperationException("TestData:Password ontbreekt in User Secrets.");
    }

    private static async Task<User> MaakGebruikerMetRolAsync(
        UserManager<User> userManager, string rol, string wachtwoord)
    {
        var email = $"test-{Guid.NewGuid():N}@example.test";
        var gebruiker = new User { UserName = email, Email = email };
        var gemaakt = await userManager.CreateAsync(gebruiker, wachtwoord);
        Assert.True(gemaakt.Succeeded, string.Join(", ", gemaakt.Errors.Select(e => e.Description)));

        var toegekend = await userManager.AddToRoleAsync(gebruiker, rol);
        Assert.True(toegekend.Succeeded, string.Join(", ", toegekend.Errors.Select(e => e.Description)));
        return gebruiker;
    }

    private static async Task LoginAsync(HttpClient client, User gebruiker, string wachtwoord)
    {
        var response = await client.PostAsJsonAsync(
            "/api/auth/login", new { Email = gebruiker.Email, Password = wachtwoord });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private static async Task VerwijderBlogpostAsync(PortfolioDbContext db, int id)
    {
        // Voorkom problemen met eerder gevolgde EF Core-objecten.
        db.ChangeTracker.Clear();
        var blogpost = await db.Blogposts.FindAsync(id);
        if (blogpost is not null)
        {
            db.Blogposts.Remove(blogpost);
            await db.SaveChangesAsync();
        }
    }
}
