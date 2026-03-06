using Microsoft.EntityFrameworkCore;
using SalonApp.Application.Interfaces;
using SalonApp.Domain.Entities;
using SalonApp.Infrastructure.Data;

namespace SalonApp.Infrastructure.Repositories;

public class WorkingHoursRepository : IWorkingHoursRepository
{
    private readonly ApplicationDbContext _context;

    public WorkingHoursRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WorkingHours>> GetByStaffIdAsync(int staffId)
    {
        return await _context.WorkingHours
            .Where(w => w.StaffId == staffId)
            .OrderBy(w => w.DayOfWeek)
            .ToListAsync();
    }

    public async Task<WorkingHours?> GetByIdAsync(int id)
    {
        return await _context.WorkingHours.FindAsync(id);
    }

    public async Task<WorkingHours?> GetByStaffIdAndDayOfWeekAsync(int staffId, int dayOfWeek)
    {
        return await _context.WorkingHours
            .FirstOrDefaultAsync(w => w.StaffId == staffId && w.DayOfWeek == dayOfWeek);
    }

    public async Task<WorkingHours> AddAsync(WorkingHours workingHours)
    {
        _context.WorkingHours.Add(workingHours);
        await _context.SaveChangesAsync();
        return workingHours;
    }

    public async Task<WorkingHours> UpdateAsync(WorkingHours workingHours)
    {
        _context.WorkingHours.Update(workingHours);
        await _context.SaveChangesAsync();
        return workingHours;
    }

    public async Task DeleteAsync(int id)
    {
        var workingHours = await _context.WorkingHours.FindAsync(id);
        if (workingHours != null)
        {
            _context.WorkingHours.Remove(workingHours);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteByStaffIdAsync(int staffId)
    {
        var workingHours = await _context.WorkingHours
            .Where(w => w.StaffId == staffId)
            .ToListAsync();
        
        _context.WorkingHours.RemoveRange(workingHours);
        await _context.SaveChangesAsync();
    }
}
