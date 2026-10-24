
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.DTOs;
using PortfolioApi.Models;

namespace PortfolioApi.Services;

public class ProjectService : IProjectService
{
    private readonly PortfolioDbContext _db;

    public ProjectService(PortfolioDbContext db)
    {
        _db = db;
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

    public Task<ProjectResponse?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request)
{
    var project = new Project
    {
        Titel = request.Titel,
        Beschrijving = request.Beschrijving,
        Categorie = request.Categorie,
        GitHubUrl = request.GitHubUrl,
        Datum = request.Datum
    };

    _db.Projecten.Add(project);
    await _db.SaveChangesAsync();

    return new ProjectResponse
    {
        Id = project.Id,
        Titel = project.Titel,
        Beschrijving = project.Beschrijving,
        Categorie = project.Categorie,
        GitHubUrl = project.GitHubUrl,
        Datum = project.Datum
    };
}

    public Task<bool> UpdateAsync(int id, UpdateProjectRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}
