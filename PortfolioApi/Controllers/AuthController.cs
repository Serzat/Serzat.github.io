
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortfolioApi.DTOs;
using PortfolioApi.Models;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    // Ontvangt de Identity-services via Dependency Injection.
    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // Registreert een nieuwe gebruiker met de rol Gast.
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        // Controleer of het e-mailadres al bestaat.
        var bestaandeGebruiker =
            await _userManager.FindByEmailAsync(request.Email);

        if (bestaandeGebruiker != null)
        {
            return BadRequest(
                "Dit e-mailadres is al geregistreerd.");
        }

        // Maak een nieuwe gebruiker aan.
        var gebruiker = new User
        {
            UserName = request.Email,
            Email = request.Email
        };

        // Sla de gebruiker op met een gehasht wachtwoord.
        var resultaat = await _userManager.CreateAsync(
            gebruiker, request.Password);

        // Geef validatiefouten terug als registratie mislukt.
        if (!resultaat.Succeeded)
        {
            return BadRequest(resultaat.Errors);
        }

        // Geef de nieuwe gebruiker automatisch de rol Gast.
        var rolResultaat =
            await _userManager.AddToRoleAsync(gebruiker, "Gast");

        if (!rolResultaat.Succeeded)
        {
            // Verwijder de gebruiker als de roltoekenning mislukt.
            await _userManager.DeleteAsync(gebruiker);

            return StatusCode(500,
                "Registratie mislukt bij het toekennen van de rol.");
        }

        // Geef een bevestiging na succesvolle registratie.
        return Ok(new
        {
            Message = "Gebruiker succesvol geregistreerd.",
            Email = gebruiker.Email,
            Rol = "Gast"
        });
    }

    // Logt een bestaande gebruiker in met een authenticatiecookie.
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        // Zoek de gebruiker op via het e-mailadres.
        var gebruiker =
            await _userManager.FindByEmailAsync(request.Email);

        // Geef een foutmelding als de gebruiker niet bestaat.
        if (gebruiker == null)
        {
            return Unauthorized("Ongeldige inloggegevens.");
        }

        // Controleer het wachtwoord en maak een logincookie aan.
        var resultaat = await _signInManager.PasswordSignInAsync(
            gebruiker,
            request.Password,
            isPersistent: false,
            lockoutOnFailure: false);

        // Geef een foutmelding als het inloggen mislukt.
        if (!resultaat.Succeeded)
        {
            return Unauthorized("Ongeldige inloggegevens.");
        }

        // Geef een bevestiging na succesvol inloggen.
        return Ok(new
        {
            Message = "Succesvol ingelogd."
        });
    }

    // Geeft informatie terug over de ingelogde gebruiker.
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        // Haal de gebruikersnaam uit de authenticatiegegevens.
        return Ok(new
        {
            Email = User.Identity?.Name
        });
    }
}
