
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PortfolioApi.DTOs;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/blogposts")]
public class BlogpostsController : ControllerBase
{
    private readonly IBlogpostService _blogpostService;

    // Ontvangt de blogpostservice via Dependency Injection.
    public BlogpostsController(IBlogpostService blogpostService)
    {
        _blogpostService = blogpostService;
    }

    // Iedereen mag alle blogposts bekijken.
    [HttpGet]
    public async Task<ActionResult<List<BlogpostResponse>>> GetBlogposts()
    {
        var blogposts = await _blogpostService.GetAllAsync();

        return Ok(blogposts);
    }

    // Iedereen mag een blogpost ophalen via het ID.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BlogpostResponse>> GetBlogpost(int id)
    {
        var blogpost = await _blogpostService.GetByIdAsync(id);

        if (blogpost == null)
        {
            return NotFound();
        }

        return Ok(blogpost);
    }

    // Alleen Editor en Admin mogen blogposts aanmaken.
    [Authorize(Policy = "CanManageContent")]
    [HttpPost]
    public async Task<ActionResult<BlogpostResponse>> CreateBlogpost(
        [FromBody] CreateBlogpostRequest request)
    {
        var nieuwBlogpost = await _blogpostService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetBlogpost),
            new { id = nieuwBlogpost.Id },
            nieuwBlogpost);
    }

    // Alleen Editor en Admin mogen blogposts wijzigen.
    [Authorize(Policy = "CanManageContent")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateBlogpost(
        int id, [FromBody] UpdateBlogpostRequest request)
    {
        var updated = await _blogpostService.UpdateAsync(id, request);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // Alleen Editor en Admin mogen blogposts verwijderen.
    [Authorize(Policy = "CanManageContent")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBlogpost(int id)
    {
        var deleted = await _blogpostService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
