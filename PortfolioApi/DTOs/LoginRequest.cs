
using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.DTOs;

public class LoginRequest
{
    // Het e-mailadres van de gebruiker.
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    // Het wachtwoord van de gebruiker.
    [Required]
    public string Password { get; set; } = string.Empty;
}
