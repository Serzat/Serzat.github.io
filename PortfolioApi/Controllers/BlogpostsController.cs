
using Microsoft.AspNetCore.Mvc;
using PortfolioApi.DTOs;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/blogposts")]
public class BlogpostsController : ControllerBase
{
    private readonly IBlogpostService _blogpostService;

    public BlogpostsController(IBlogpostService blogpostService)
    {
        _blogpostService = blogpostService;
    }

    // Alle blogposts ophalen
    [HttpGet]
    public async Task<ActionResult<List<BlogpostResponse>>> GetBlogposts()
    {
        var blogposts = await _blogpostService.GetAllAsync();

        return Ok(blogposts);
    }

    // Een blogpost ophalen op basis van ID
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

    // Een nieuwe blogpost aanmaken
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

    // Een bestaande blogpost wijzigen
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

    // Een blogpost verwijderen
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
