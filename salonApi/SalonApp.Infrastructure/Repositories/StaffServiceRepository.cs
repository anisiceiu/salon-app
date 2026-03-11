using Microsoft.EntityFrameworkCore;
using SalonApp.Application.Interfaces;
using SalonApp.Domain.Entities;
using SalonApp.Infrastructure.Data;

namespace SalonApp.Infrastructure.Repositories;

public class StaffServiceRepository : IStaffServiceRepository
{
    private readonly ApplicationDbContext _context;

    public StaffServiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StaffService>> GetAllAsync()
    {
        return await _context.StaffServices
            .Include(ss => ss.Staff)
                .ThenInclude(s => s!.User)
            .Include(ss => ss.Service)
                .ThenInclude(s => s!.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<StaffService>> GetByStaffIdAsync(int staffId)
    {
        return await _context.StaffServices
            .Include(ss => ss.Service)
                .ThenInclude(s => s!.Category)
            .Where(ss => ss.StaffId == staffId)
            .ToListAsync();
    }

    public async Task<IEnumerable<StaffService>> GetByServiceIdAsync(int serviceId)
    {
        return await _context.StaffServices
            .Include(ss => ss.Staff)
                .ThenInclude(s => s!.User)
            .Where(ss => ss.ServiceId == serviceId)
            .ToListAsync();
    }

    public async Task AddRangeAsync(IEnumerable<StaffService> staffServices)
    {
        _context.StaffServices.AddRange(staffServices);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByStaffIdAsync(int staffId)
    {
        var staffServices = await _context.StaffServices
            .Where(ss => ss.StaffId == staffId)
            .ToListAsync();
        
        _context.StaffServices.RemoveRange(staffServices);
        await _context.SaveChangesAsync();
    }
}
