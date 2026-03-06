using SalonApp.Domain.Entities;

namespace SalonApp.Application.Interfaces;

public interface IServiceCategoryRepository
{
    Task<IEnumerable<ServiceCategory>> GetAllAsync();
    Task<ServiceCategory?> GetByIdAsync(int id);
    Task<ServiceCategory> AddAsync(ServiceCategory category);
    Task<ServiceCategory> UpdateAsync(ServiceCategory category);
    Task DeleteAsync(int id);
}
