using SalonApp.Domain.Entities;

namespace SalonApp.Application.Interfaces;

public interface IServiceRepository
{
    Task<IEnumerable<Service>> GetAllAsync();
    Task<Service?> GetByIdAsync(int id);
    Task<IEnumerable<Service>> GetByCategoryAsync(int categoryId);
    Task<Service> AddAsync(Service service);
    Task<Service> UpdateAsync(Service service);
    Task DeleteAsync(int id);
}
