
using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.DTOs;

public class RegisterRequest
{
    // Het e-mailadres van de nieuwe gebruiker.
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    // Het wachtwoord van de nieuwe gebruiker.
    [Required]
    public string Password { get; set; } = string.Empty;
}
