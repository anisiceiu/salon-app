using SalonApp.Application.DTOs;

namespace SalonApp.Application.Interfaces;

public interface IDiscountService
{
    Task<IEnumerable<DiscountDto>> GetAllDiscountsAsync();
    Task<DiscountDto?> GetDiscountByIdAsync(int id);
    Task<DiscountDto?> GetDiscountByCodeAsync(string code);
    Task<IEnumerable<DiscountDto>> GetActiveDiscountsAsync();
    Task<DiscountDto> CreateDiscountAsync(CreateDiscountRequest request);
    Task<DiscountDto> UpdateDiscountAsync(int id, UpdateDiscountRequest request);
    Task<bool> DeleteDiscountAsync(int id);
}
