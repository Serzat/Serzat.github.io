using Microsoft.AspNetCore.Mvc;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetProjects()
    {
        return Ok("Hier komen mijn projecten");
    }
}