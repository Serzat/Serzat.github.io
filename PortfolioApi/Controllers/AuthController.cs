
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

    // Geef de validatiefouten terug als registratie mislukt.
    if (!resultaat.Succeeded)
    {
        return BadRequest(resultaat.Errors);
    }

    // Geef elke nieuwe gebruiker automatisch de rol Gast.
    var rolResultaat =
        await _userManager.AddToRoleAsync(gebruiker, "Gast");

    if (!rolResultaat.Succeeded)
    {
        // Verwijder de gebruiker als de roltoekenning mislukt.
        await _userManager.DeleteAsync(gebruiker);

        return StatusCode(500,
            "Registratie mislukt bij het toekennen van de rol.");
    }

    // Registratie is geslaagd.
    return Ok(new
    {
        Message = "Gebruiker succesvol geregistreerd.",
        Email = gebruiker.Email,
        Rol = "Gast"
    });
}
