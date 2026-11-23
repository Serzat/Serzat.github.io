
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

    // Iedereen mag alle projecten bekijken.
    [HttpGet]
    public async Task<ActionResult<List<ProjectResponse>>> GetProjects()
    {
        var projecten = await _projectService.GetAllAsync();

        return Ok(projecten);
    }

    // Alleen Editor en Admin mogen projecten aanmaken.
    [Authorize(Policy = "CanManageContent")]
    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> CreateProject(
        [FromBody] CreateProjectRequest request)
    {
        var nieuwProject = await _projectService.CreateAsync(request);

        return Created($"/api/projects/{nieuwProject.Id}", nieuwProject);
    }

    // Iedereen mag een project opvragen via het ID.
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

    // Alleen Editor en Admin mogen projecten wijzigen.
    [Authorize(Policy = "CanManageContent")]
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

    // Alleen Editor en Admin mogen projecten verwijderen.
    [Authorize(Policy = "CanManageContent")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var deleted = await _projectService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
