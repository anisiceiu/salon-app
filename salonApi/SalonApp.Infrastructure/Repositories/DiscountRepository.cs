using Microsoft.EntityFrameworkCore;
using SalonApp.Application.Interfaces;
using SalonApp.Domain.Entities;
using SalonApp.Infrastructure.Data;

namespace SalonApp.Infrastructure.Repositories;

public class DiscountRepository : IDiscountRepository
{
    private readonly ApplicationDbContext _context;

    public DiscountRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Discount>> GetAllAsync()
    {
        return await _context.Discounts
            .OrderBy(d => d.Code)
            .ToListAsync();
    }

    public async Task<Discount?> GetByIdAsync(int id)
    {
        return await _context.Discounts.FindAsync(id);
    }

    public async Task<Discount?> GetByCodeAsync(string code)
    {
        return await _context.Discounts
            .FirstOrDefaultAsync(d => d.Code.ToLower() == code.ToLower());
    }

    public async Task<IEnumerable<Discount>> GetActiveAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Discounts
            .Where(d => d.IsActive && d.ValidFrom <= now && d.ValidUntil >= now)
            .OrderBy(d => d.Code)
            .ToListAsync();
    }

    public async Task<Discount> AddAsync(Discount discount)
    {
        _context.Discounts.Add(discount);
        await _context.SaveChangesAsync();
        return discount;
    }

    public async Task<Discount> UpdateAsync(Discount discount)
    {
        _context.Discounts.Update(discount);
        await _context.SaveChangesAsync();
        return discount;
    }

    public async Task DeleteAsync(int id)
    {
        var discount = await _context.Discounts.FindAsync(id);
        if (discount != null)
        {
            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
        }
    }
}
