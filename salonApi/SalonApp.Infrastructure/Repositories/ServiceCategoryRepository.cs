using Microsoft.EntityFrameworkCore;
using SalonApp.Application.Interfaces;
using SalonApp.Domain.Entities;
using SalonApp.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SalonApp.Infrastructure.Repositories;

public class ServiceCategoryRepository : IServiceCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public ServiceCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ServiceCategory>> GetAllAsync()
    {
        return await _context.ServiceCategories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<ServiceCategory?> GetByIdAsync(int id)
    {
        return await _context.ServiceCategories
            .Include(c => c.Services)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<ServiceCategory> AddAsync(ServiceCategory category)
    {
        _context.ServiceCategories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<ServiceCategory> UpdateAsync(ServiceCategory category)
    {
        _context.ServiceCategories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _context.ServiceCategories.FindAsync(id);
        if (category != null)
        {
            category.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}
