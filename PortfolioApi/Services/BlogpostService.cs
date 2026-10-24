
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.DTOs;
using PortfolioApi.Models;

namespace PortfolioApi.Services;

public class BlogpostService : IBlogpostService
{
    private readonly PortfolioDbContext _db;

    public BlogpostService(PortfolioDbContext db)
    {
        _db = db;
    }

    // Alle blogposts ophalen
    public async Task<List<BlogpostResponse>> GetAllAsync()
    {
        return await _db.Blogposts
            .AsNoTracking()
            .Select(b => new BlogpostResponse
            {
                Id = b.Id,
                Titel = b.Titel,
                Inhoud = b.Inhoud,
                Publicatiedatum = b.Publicatiedatum
            })
            .ToListAsync();
    }

    // Een blogpost ophalen op basis van ID
    public async Task<BlogpostResponse?> GetByIdAsync(int id)
    {
        return await _db.Blogposts
            .AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new BlogpostResponse
            {
                Id = b.Id,
                Titel = b.Titel,
                Inhoud = b.Inhoud,
                Publicatiedatum = b.Publicatiedatum
            })
            .FirstOrDefaultAsync();
    }

    // Een nieuwe blogpost aanmaken
    public async Task<BlogpostResponse> CreateAsync(CreateBlogpostRequest request)
    {
        var blogpost = new Blogpost
        {
            Titel = request.Titel,
            Inhoud = request.Inhoud,
            Publicatiedatum = request.Publicatiedatum
        };

        _db.Blogposts.Add(blogpost);
        await _db.SaveChangesAsync();

        return new BlogpostResponse
        {
            Id = blogpost.Id,
            Titel = blogpost.Titel,
            Inhoud = blogpost.Inhoud,
            Publicatiedatum = blogpost.Publicatiedatum
        };
    }

    // Een bestaande blogpost wijzigen
    public async Task<bool> UpdateAsync(int id, UpdateBlogpostRequest request)
    {
        var blogpost = await _db.Blogposts
            .FirstOrDefaultAsync(b => b.Id == id);

        if (blogpost == null)
        {
            return false;
        }

        blogpost.Titel = request.Titel;
        blogpost.Inhoud = request.Inhoud;
        blogpost.Publicatiedatum = request.Publicatiedatum;

        await _db.SaveChangesAsync();

        return true;
    }

    // Een blogpost verwijderen
    public async Task<bool> DeleteAsync(int id)
    {
        var blogpost = await _db.Blogposts
            .FirstOrDefaultAsync(b => b.Id == id);

        if (blogpost == null)
        {
            return false;
        }

        _db.Blogposts.Remove(blogpost);
        await _db.SaveChangesAsync();

        return true;
    }
}
