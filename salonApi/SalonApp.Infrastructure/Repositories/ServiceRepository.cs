using Microsoft.EntityFrameworkCore;
using SalonApp.Application.Interfaces;
using SalonApp.Domain.Entities;
using SalonApp.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SalonApp.Infrastructure.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly ApplicationDbContext _context;

    public ServiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Service>> GetAllAsync()
    {
        return await _context.Services
            .Include(s => s.Category)
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Service?> GetByIdAsync(int id)
    {
        return await _context.Services
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Service>> GetByCategoryAsync(int categoryId)
    {
        return await _context.Services
            .Include(s => s.Category)
            .Where(s => s.CategoryId == categoryId && s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Service> AddAsync(Service service)
    {
        _context.Services.Add(service);
        await _context.SaveChangesAsync();
        return service;
    }

    public async Task<Service> UpdateAsync(Service service)
    {
        _context.Services.Update(service);
        await _context.SaveChangesAsync();
        return service;
    }

    public async Task DeleteAsync(int id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service != null)
        {
            service.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}
