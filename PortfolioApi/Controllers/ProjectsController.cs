using Microsoft.AspNetCore.Mvc;
using PortfolioApi.DTOs;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProjectResponse>>> GetProjects()
    {
        var projecten = await _projectService.GetAllAsync();

        return Ok(projecten);
    }

    [HttpPost]
public async Task<ActionResult<ProjectResponse>> CreateProject(
    [FromBody] CreateProjectRequest request)
{
    var nieuwProject = await _projectService.CreateAsync(request);

    return Created($"/api/projects/{nieuwProject.Id}", nieuwProject);
}

[HttpGet("{id:int}")]
public async Task<ActionResult<ProjectResponse>> GetProject(int id)
{
    var project = await _projectService.GetByIdAsync(id);

    if (project == null)
    {
        return NotFound();
    }

    return Ok(project);
}


[HttpPut("{id:int}")]
public async Task<IActionResult> UpdateProject(
    int id, [FromBody] UpdateProjectRequest request)
{
    var updated = await _projectService.UpdateAsync(id, request);

    if (!updated)
    {
        return NotFound();
    }

    return NoContent();
}

}