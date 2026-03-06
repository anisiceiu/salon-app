using SalonApp.Application.DTOs;
using SalonApp.Application.Interfaces;
using SalonApp.Domain.Entities;

namespace SalonApp.Application.Services;

public class DiscountService : IDiscountService
{
    private readonly IDiscountRepository _discountRepository;

    public DiscountService(IDiscountRepository discountRepository)
    {
        _discountRepository = discountRepository;
    }

    public async Task<IEnumerable<DiscountDto>> GetAllDiscountsAsync()
    {
        var discounts = await _discountRepository.GetAllAsync();
        return discounts.Select(MapToDto);
    }

    public async Task<DiscountDto?> GetDiscountByIdAsync(int id)
    {
        var discount = await _discountRepository.GetByIdAsync(id);
        return discount == null ? null : MapToDto(discount);
    }

    public async Task<DiscountDto?> GetDiscountByCodeAsync(string code)
    {
        var discount = await _discountRepository.GetByCodeAsync(code);
        return discount == null ? null : MapToDto(discount);
    }

    public async Task<IEnumerable<DiscountDto>> GetActiveDiscountsAsync()
    {
        var discounts = await _discountRepository.GetActiveAsync();
        return discounts.Select(MapToDto);
    }

    public async Task<DiscountDto> CreateDiscountAsync(CreateDiscountRequest request)
    {
        // Check if discount code already exists
        var existing = await _discountRepository.GetByCodeAsync(request.Code);
        if (existing != null)
        {
            throw new InvalidOperationException("Discount code already exists");
        }

        // Validate discount value
        if (request.DiscountValue <= 0)
        {
            throw new InvalidOperationException("Discount value must be greater than 0");
        }

        // Validate date range
        if (request.ValidFrom >= request.ValidUntil)
        {
            throw new InvalidOperationException("ValidFrom must be before ValidUntil");
        }

        // Validate discount type specific rules
        if (request.DiscountType == Domain.Enums.DiscountType.Percentage && request.DiscountValue > 100)
        {
            throw new InvalidOperationException("Percentage discount cannot exceed 100%");
        }

        var discount = new Discount
        {
            Code = request.Code.ToUpper(),
            Name = request.Name,
            DiscountType = request.DiscountType,
            DiscountValue = request.DiscountValue,
            MinOrderValue = request.MinOrderValue,
            ValidFrom = request.ValidFrom,
            ValidUntil = request.ValidUntil,
            IsActive = request.IsActive,
            MaxUses = request.MaxUses,
            UsedCount = 0
        };

        var created = await _discountRepository.AddAsync(discount);
        return MapToDto(created);
    }

    public async Task<DiscountDto> UpdateDiscountAsync(int id, UpdateDiscountRequest request)
    {
        var discount = await _discountRepository.GetByIdAsync(id);
        if (discount == null)
        {
            throw new InvalidOperationException("Discount not found");
        }

        // Check if code is being changed and if new code already exists
        if (discount.Code != request.Code.ToUpper())
        {
            var existing = await _discountRepository.GetByCodeAsync(request.Code);
            if (existing != null)
            {
                throw new InvalidOperationException("Discount code already exists");
            }
        }

        // Validate discount value
        if (request.DiscountValue <= 0)
        {
            throw new InvalidOperationException("Discount value must be greater than 0");
        }

        // Validate date range
        if (request.ValidFrom >= request.ValidUntil)
        {
            throw new InvalidOperationException("ValidFrom must be before ValidUntil");
        }

        // Validate discount type specific rules
        if (request.DiscountType == Domain.Enums.DiscountType.Percentage && request.DiscountValue > 100)
        {
            throw new InvalidOperationException("Percentage discount cannot exceed 100%");
        }

        discount.Code = request.Code.ToUpper();
        discount.Name = request.Name;
        discount.DiscountType = request.DiscountType;
        discount.DiscountValue = request.DiscountValue;
        discount.MinOrderValue = request.MinOrderValue;
        discount.ValidFrom = request.ValidFrom;
        discount.ValidUntil = request.ValidUntil;
        discount.IsActive = request.IsActive;
        discount.MaxUses = request.MaxUses;

        var updated = await _discountRepository.UpdateAsync(discount);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteDiscountAsync(int id)
    {
        var discount = await _discountRepository.GetByIdAsync(id);
        if (discount == null)
        {
            return false;
        }

        await _discountRepository.DeleteAsync(id);
        return true;
    }

    private static DiscountDto MapToDto(Discount discount)
    {
        return new DiscountDto
        {
            Id = discount.Id,
            Code = discount.Code,
            Name = discount.Name,
            DiscountType = discount.DiscountType,
            DiscountValue = discount.DiscountValue,
            MinOrderValue = discount.MinOrderValue,
            ValidFrom = discount.ValidFrom,
            ValidUntil = discount.ValidUntil,
            IsActive = discount.IsActive,
            MaxUses = discount.MaxUses,
            UsedCount = discount.UsedCount
        };
    }
}
