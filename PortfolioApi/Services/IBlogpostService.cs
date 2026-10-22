using PortfolioApi.DTOs;

namespace PortfolioApi.Services;

public interface IBlogpostService
{
    Task<List<BlogpostResponse>> GetAllAsync();

    Task<BlogpostResponse?> GetByIdAsync(int id);

    Task<BlogpostResponse> CreateAsync(CreateBlogpostRequest request);

    Task<bool> UpdateAsync(int id, UpdateBlogpostRequest request);

    Task<bool> DeleteAsync(int id);
}