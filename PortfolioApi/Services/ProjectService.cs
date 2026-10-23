using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.DTOs;

namespace PortfolioApi.Services;

public class ProjectService : IProjectService
{
    private readonly PortfolioDbContext _db;

    public ProjectService(PortfolioDbContext db)
    {
        _db = db;
    }
}

public async Task<List<ProjectResponse>> GetAllAsync()
{
    return await _db.Projecten
        .AsNoTracking()
        .Select(p => new ProjectResponse
        {
            Id = p.Id,
            Titel = p.Titel,
            Beschrijving = p.Beschrijving,
            Categorie = p.Categorie,
            GitHubUrl = p.GitHubUrl,
            Datum = p.Datum
        })
        .ToListAsync();
}