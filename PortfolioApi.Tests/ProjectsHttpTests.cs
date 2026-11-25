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

public class ProjectsHttpTests
{
    // Test 1: Iedereen mag projecten bekijken.
    [Fact]
    public async Task GetProjects_ZonderLogin_Geeft200()
    {
        // Arrange
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/projects");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // Test 2: Een onbekend project geeft 404 terug.
    [Fact]
    public async Task GetProject_OnbekendId_Geeft404()
    {
        // Arrange
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/projects/-1");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // Test 3: Een bestaand project geeft de juiste gegevens terug.
    [Fact]
    public async Task GetProject_BestaandId_GeeftJuisteProject()
    {
        // Arrange: start de API met de testdatabase.
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<PortfolioDbContext>();

        // Maak een testproject aan.
        var project = new Project
        {
            Titel = "Testproject xUnit",
            Beschrijving = "Een project voor automatische tests.",
            Categorie = "Test",
            GitHubUrl = "https://github.com/",
            Datum = new DateTime(2026, 11, 25)
        };

        db.Projecten.Add(project);
        await db.SaveChangesAsync();

        try
        {
            // Act: vraag het aangemaakte project op via HTTP.
            var response = await client.GetAsync(
                $"/api/projects/{project.Id}");

            // Assert: controleer de statuscode.
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Controleer of de juiste projectgegevens terugkomen.
            var resultaat = await response.Content
                .ReadFromJsonAsync<ProjectResponse>();

            Assert.NotNull(resultaat);
            Assert.Equal(project.Id, resultaat.Id);
            Assert.Equal(project.Titel, resultaat.Titel);
        }
        finally
        {
            // Verwijder het testproject, ook als de test mislukt.
            db.Projecten.Remove(project);
            await db.SaveChangesAsync();
        }
    }

    // Test 4: Een Editor mag een project aanmaken.
    [Fact]
    public async Task CreateProject_AlsEditor_Geeft201_EnSlaatProjectOp()
    {
        // Arrange: start de API met de testdatabase.
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<PortfolioDbContext>();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<User>>();

        // Haal het testwachtwoord op uit User Secrets.
        var configuratie = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var wachtwoord = configuratie["TestData:Password"]
            ?? throw new InvalidOperationException(
                "Het testwachtwoord ontbreekt.");

        // Gebruik een uniek e-mailadres voor iedere test.
        var email = $"editor-{Guid.NewGuid():N}@example.test";

        var gebruiker = new User
        {
            UserName = email,
            Email = email
        };

        int? projectId = null;

        try
        {
            // Maak een testgebruiker aan.
            var resultaat = await userManager.CreateAsync(
                gebruiker, wachtwoord);

            Assert.True(resultaat.Succeeded);

            // Geef de testgebruiker de rol Editor.
            var rolResultaat = await userManager.AddToRoleAsync(
                gebruiker, "Editor");

            Assert.True(rolResultaat.Succeeded);

            // Act: log in via de API.
            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login",
                new { Email = email, Password = wachtwoord });

            Assert.Equal(
                HttpStatusCode.OK, loginResponse.StatusCode);

            // Maak een nieuw project via HTTP.
            var request = new CreateProjectRequest
            {
                Titel = "Testproject POST",
                Beschrijving = "Een project voor de POST-test.",
                Categorie = "Test",
                GitHubUrl = "https://github.com/",
                Datum = new DateTime(2026, 11, 25)
            };

            var response = await client.PostAsJsonAsync(
                "/api/projects", request);

            // Assert: controleer of de API 201 teruggeeft.
            Assert.Equal(
                HttpStatusCode.Created, response.StatusCode);

            // Controleer de gegevens in de response.
            var project = await response.Content
                .ReadFromJsonAsync<ProjectResponse>();

            Assert.NotNull(project);

            projectId = project.Id;

            Assert.Equal(request.Titel, project.Titel);

            // Controleer of het project in de database staat.
            var opgeslagen = await db.Projecten.FindAsync(projectId.Value);

            Assert.NotNull(opgeslagen);
            Assert.Equal(request.Titel, opgeslagen.Titel);
        }
        finally
        {
            // Verwijder het testproject als het is aangemaakt.
            if (projectId.HasValue)
            {
                var project = await db.Projecten.FindAsync(projectId.Value);

                if (project != null)
                {
                    db.Projecten.Remove(project);
                    await db.SaveChangesAsync();
                }
            }

            // Verwijder de tijdelijke testgebruiker.
            var bestaandeGebruiker =
                await userManager.FindByEmailAsync(email);

            if (bestaandeGebruiker != null)
            {
                await userManager.DeleteAsync(bestaandeGebruiker);
            }
        }
    }

    // Test 5: Zonder login mag een gebruiker geen project aanmaken.
    [Fact]
    public async Task CreateProject_ZonderLogin_Geeft401()
    {
        // Arrange: start de API met de testdatabase.
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<PortfolioDbContext>();

        // Tel hoeveel projecten er vóór het verzoek zijn.
        var aantalVoor = await db.Projecten.CountAsync();

        // Maak een geldig projectverzoek aan.
        var request = new CreateProjectRequest
        {
            Titel = "Project zonder login",
            Beschrijving = "Dit project mag niet worden opgeslagen.",
            Categorie = "Test",
            GitHubUrl = "https://github.com/",
            Datum = new DateTime(2026, 11, 25)
        };

        // Act: probeer zonder login een project aan te maken.
        var response = await client.PostAsJsonAsync(
            "/api/projects", request);

        // Assert: controleer of de API 401 teruggeeft.
        Assert.Equal(
            HttpStatusCode.Unauthorized, response.StatusCode);

        // Controleer of er geen project is toegevoegd.
        var aantalNa = await db.Projecten.CountAsync();

        Assert.Equal(aantalVoor, aantalNa);
    }

    // Test 6: Een Gast mag geen project aanmaken.
    [Fact]
    public async Task CreateProject_AlsGast_Geeft403_EnSlaatNietsOp()
    {
        // Arrange: start de API met de testdatabase.
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<PortfolioDbContext>();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<User>>();

        // Haal het testwachtwoord op uit User Secrets.
        var configuratie = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var wachtwoord = configuratie["TestData:Password"]
            ?? throw new InvalidOperationException(
                "Het testwachtwoord ontbreekt.");

        // Gebruik een uniek e-mailadres voor de test.
        var email = $"gast-{Guid.NewGuid():N}@example.test";

        var gebruiker = new User
        {
            UserName = email,
            Email = email
        };

        try
        {
            // Maak een tijdelijke testgebruiker aan.
            var resultaat = await userManager.CreateAsync(
                gebruiker, wachtwoord);

            Assert.True(resultaat.Succeeded);

            // Geef de gebruiker de rol Gast.
            var rolResultaat = await userManager.AddToRoleAsync(
                gebruiker, "Gast");

            Assert.True(rolResultaat.Succeeded);

            // Log in via de API.
            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login",
                new { Email = email, Password = wachtwoord });

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            // Maak een projectverzoek met een unieke titel.
            var titel = $"Gast project {Guid.NewGuid():N}";

            var request = new CreateProjectRequest
            {
                Titel = titel,
                Beschrijving = "Dit project mag niet worden opgeslagen.",
                Categorie = "Test",
                GitHubUrl = "https://github.com/",
                Datum = new DateTime(2026, 11, 25)
            };

            // Act: probeer als Gast een project aan te maken.
            var response = await client.PostAsJsonAsync(
                "/api/projects", request);

            // Assert: controleer of de API 403 teruggeeft.
            Assert.Equal(
                HttpStatusCode.Forbidden, response.StatusCode);

            // Controleer of het project niet is opgeslagen.
            var bestaat = await db.Projecten
                .AnyAsync(p => p.Titel == titel);

            Assert.False(bestaat);
        }
        finally
        {
            // Verwijder de tijdelijke testgebruiker.
            var bestaandeGebruiker =
                await userManager.FindByEmailAsync(email);

            if (bestaandeGebruiker != null)
            {
                await userManager.DeleteAsync(bestaandeGebruiker);
            }
        }
    }


    // Test 7: Een Editor mag een bestaand project wijzigen.
    [Fact]
    public async Task UpdateProject_AlsEditor_Geeft204_EnWijzigtProject()
    {
        // Arrange: start de API met de testdatabase.
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<PortfolioDbContext>();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<User>>();

        // Haal het testwachtwoord op uit User Secrets.
        var configuratie = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var wachtwoord = configuratie["TestData:Password"]
            ?? throw new InvalidOperationException(
                "Het testwachtwoord ontbreekt.");

        // Maak een tijdelijke Editor-gebruiker aan.
        var email = $"editor-put-{Guid.NewGuid():N}@example.test";

        var gebruiker = new User
        {
            UserName = email,
            Email = email
        };

        int? projectId = null;

        try
        {
            var resultaat = await userManager.CreateAsync(
                gebruiker, wachtwoord);

            Assert.True(resultaat.Succeeded);

            // Geef de gebruiker de rol Editor.
            var rolResultaat = await userManager.AddToRoleAsync(
                gebruiker, "Editor");

            Assert.True(rolResultaat.Succeeded);

            // Maak een project aan in de testdatabase.
            var project = new Project
            {
                Titel = "Oorspronkelijke titel",
                Beschrijving = "Oorspronkelijke beschrijving",
                Categorie = "Test",
                GitHubUrl = "https://github.com/",
                Datum = new DateTime(2026, 11, 25)
            };

            db.Projecten.Add(project);
            await db.SaveChangesAsync();

            projectId = project.Id;

            // Log in als Editor via HTTP.
            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login",
                new { Email = email, Password = wachtwoord });

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            // Maak de nieuwe projectgegevens aan.
            var request = new UpdateProjectRequest
            {
                Titel = "Bijgewerkte titel",
                Beschrijving = "Bijgewerkte beschrijving",
                Categorie = "Software Engineering",
                GitHubUrl = "https://github.com/",
                Datum = new DateTime(2026, 11, 25)
            };

            // Act: wijzig het project via de API.
            var response = await client.PutAsJsonAsync(
                $"/api/projects/{projectId.Value}", request);

            // Assert: controleer of de API 204 teruggeeft.
            Assert.Equal(
                HttpStatusCode.NoContent, response.StatusCode);

            // Controleer de opgeslagen gegevens in de database.
            var bijgewerkt = await db.Projecten
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == projectId.Value);

            Assert.NotNull(bijgewerkt);
            Assert.Equal(request.Titel, bijgewerkt.Titel);
            Assert.Equal(request.Beschrijving, bijgewerkt.Beschrijving);
        }
        finally
        {
            // Verwijder het tijdelijke testproject.
            if (projectId.HasValue)
            {
                var project = await db.Projecten.FindAsync(
                    projectId.Value);

                if (project != null)
                {
                    db.Projecten.Remove(project);
                    await db.SaveChangesAsync();
                }
            }

            // Verwijder de tijdelijke Editor-gebruiker.
            var bestaandeGebruiker =
                await userManager.FindByEmailAsync(email);

            if (bestaandeGebruiker != null)
            {
                await userManager.DeleteAsync(bestaandeGebruiker);
            }
        }
    }

    // Test 8: Een Admin mag een bestaand project verwijderen.
    [Fact]
    public async Task DeleteProject_AlsAdmin_Geeft204_EnVerwijdertProject()
    {
        // Arrange: start de API met de testdatabase.
        using var factory = new PortfolioApiFactory();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<PortfolioDbContext>();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<User>>();

        // Haal het testwachtwoord op uit User Secrets.
        var configuratie = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var wachtwoord = configuratie["TestData:Password"]
            ?? throw new InvalidOperationException(
                "Het testwachtwoord ontbreekt.");

        // Maak een tijdelijke Admin-gebruiker aan.
        var email = $"admin-delete-{Guid.NewGuid():N}@example.test";

        var gebruiker = new User
        {
            UserName = email,
            Email = email
        };

        int? projectId = null;

        try
        {
            // Maak de testgebruiker aan.
            var resultaat = await userManager.CreateAsync(
                gebruiker, wachtwoord);

            Assert.True(resultaat.Succeeded);

            // Geef de gebruiker de rol Admin.
            var rolResultaat = await userManager.AddToRoleAsync(
                gebruiker, "Admin");

            Assert.True(rolResultaat.Succeeded);

            // Maak een testproject aan.
            var project = new Project
            {
                Titel = "Project voor DELETE-test",
                Beschrijving = "Dit project wordt verwijderd.",
                Categorie = "Test",
                GitHubUrl = "https://github.com/",
                Datum = new DateTime(2026, 11, 25)
            };

            db.Projecten.Add(project);
            await db.SaveChangesAsync();

            projectId = project.Id;

            // Log in als Admin via HTTP.
            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login",
                new { Email = email, Password = wachtwoord });

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            // Act: verwijder het project via de API.
            var response = await client.DeleteAsync(
                $"/api/projects/{projectId.Value}");

            // Assert: controleer of de API 204 teruggeeft.
            Assert.Equal(
                HttpStatusCode.NoContent, response.StatusCode);

            // Controleer of het project uit de database is verwijderd.
            var bestaat = await db.Projecten
                .AsNoTracking()
                .AnyAsync(p => p.Id == projectId.Value);

            Assert.False(bestaat);

            // Controleer of GET nu 404 teruggeeft.
            var getResponse = await client.GetAsync(
                $"/api/projects/{projectId.Value}");

            Assert.Equal(
                HttpStatusCode.NotFound, getResponse.StatusCode);
        }
        finally
        {
            // Verwijder eventueel achtergebleven testgegevens.
            db.ChangeTracker.Clear();

            if (projectId.HasValue)
            {
                var project = await db.Projecten.FindAsync(
                    projectId.Value);

                if (project != null)
                {
                    db.Projecten.Remove(project);
                    await db.SaveChangesAsync();
                }
            }

            // Verwijder de tijdelijke Admin-gebruiker.
            var bestaandeGebruiker =
                await userManager.FindByEmailAsync(email);

            if (bestaandeGebruiker != null)
            {
                await userManager.DeleteAsync(bestaandeGebruiker);
            }
        }
    }



}
