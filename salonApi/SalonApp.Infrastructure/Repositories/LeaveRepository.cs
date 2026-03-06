using Microsoft.EntityFrameworkCore;
using SalonApp.Application.Interfaces;
using SalonApp.Domain.Entities;
using SalonApp.Domain.Enums;
using SalonApp.Infrastructure.Data;

namespace SalonApp.Infrastructure.Repositories;

public class LeaveRepository : ILeaveRepository
{
    private readonly ApplicationDbContext _context;

    public LeaveRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Leave>> GetAllAsync()
    {
        return await _context.Leaves
            .Include(l => l.Staff)
                .ThenInclude(s => s!.User)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<Leave?> GetByIdAsync(int id)
    {
        return await _context.Leaves
            .Include(l => l.Staff)
                .ThenInclude(s => s!.User)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<Leave>> GetByStaffIdAsync(int staffId)
    {
        return await _context.Leaves
            .Where(l => l.StaffId == staffId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Leave>> GetByStaffIdAndDateRangeAsync(int staffId, DateTime startDate, DateTime endDate)
    {
        return await _context.Leaves
            .Where(l => l.StaffId == staffId 
                && l.StartDate <= endDate.Date 
                && l.EndDate >= startDate.Date
                && l.Status == LeaveStatus.Approved)
            .OrderBy(l => l.StartDate)
            .ToListAsync();
    }

    public async Task<Leave> AddAsync(Leave leave)
    {
        _context.Leaves.Add(leave);
        await _context.SaveChangesAsync();
        return leave;
    }

    public async Task<Leave> UpdateAsync(Leave leave)
    {
        _context.Leaves.Update(leave);
        await _context.SaveChangesAsync();
        return leave;
    }

    public async Task DeleteAsync(int id)
    {
        var leave = await _context.Leaves.FindAsync(id);
        if (leave != null)
        {
            _context.Leaves.Remove(leave);
            await _context.SaveChangesAsync();
        }
    }
}
