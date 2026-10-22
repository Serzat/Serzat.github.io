using PortfolioApi.DTOs;

namespace PortfolioApi.Services;

public interface IProjectService
{
    Task<List<ProjectResponse>> GetAllAsync();

    Task<ProjectResponse?> GetByIdAsync(int id);

    Task<ProjectResponse> CreateAsync(CreateProjectRequest request);

    Task<bool> UpdateAsync(int id, UpdateProjectRequest request);

    Task<bool> DeleteAsync(int id);
}