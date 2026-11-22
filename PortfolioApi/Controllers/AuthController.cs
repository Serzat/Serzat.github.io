
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

    // Ontvangt UserManager via Dependency Injection.
    public AuthController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    // Registreert een nieuwe gebruiker met de rol Gast.
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        // Controleer of het e-mailadres al bestaat.
        var bestaandeGebruiker =
            await _userManager.FindByEmailAsync(request.Email);

        if (bestaandeGebruiker != null)
        {
            return BadRequest("Dit e-mailadres is al geregistreerd.");
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

        // Geef de nieuwe gebruiker de rol Gast.
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
}
