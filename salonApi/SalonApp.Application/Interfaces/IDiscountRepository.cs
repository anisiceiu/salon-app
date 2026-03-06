using SalonApp.Domain.Entities;

namespace SalonApp.Application.Interfaces;

public interface IDiscountRepository
{
    Task<IEnumerable<Discount>> GetAllAsync();
    Task<Discount?> GetByIdAsync(int id);
    Task<Discount?> GetByCodeAsync(string code);
    Task<IEnumerable<Discount>> GetActiveAsync();
    Task<Discount> AddAsync(Discount discount);
    Task<Discount> UpdateAsync(Discount discount);
    Task DeleteAsync(int id);
}
