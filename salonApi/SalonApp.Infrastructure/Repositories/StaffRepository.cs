using Microsoft.EntityFrameworkCore;
using SalonApp.Application.Interfaces;
using SalonApp.Domain.Entities;
using SalonApp.Infrastructure.Data;

namespace SalonApp.Infrastructure.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly ApplicationDbContext _context;

    public StaffRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Staff>> GetAllAsync()
    {
        return await _context.Staff
            .Include(s => s.User)
            .Include(s => s.StaffServices)
                .ThenInclude(ss => ss.Service)
            .Include(s => s.WorkingHours)
            .OrderBy(s => s.User!.FullName)
            .ToListAsync();
    }

    public async Task<Staff?> GetByIdAsync(int id)
    {
        return await _context.Staff
            .Include(s => s.User)
            .Include(s => s.StaffServices)
                .ThenInclude(ss => ss.Service)
                    .ThenInclude(s => s!.Category)
            .Include(s => s.WorkingHours)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Staff?> GetByUserIdAsync(int userId)
    {
        return await _context.Staff
            .Include(s => s.User)
            .Include(s => s.StaffServices)
                .ThenInclude(ss => ss.Service)
            .Include(s => s.WorkingHours)
            .FirstOrDefaultAsync(s => s.Id == userId);
    }

    public async Task<Staff> AddAsync(Staff staff)
    {
        _context.Staff.Add(staff);
        await _context.SaveChangesAsync();
        return staff;
    }

    public async Task<Staff> AddWithUserAsync(User user, Staff staff, IEnumerable<int>? serviceIds = null)
    {
        // Add user first
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Set staff ID to match user ID
        staff.Id = user.Id;
        
        // Add staff
        _context.Staff.Add(staff);
        await _context.SaveChangesAsync();

        // Add services if provided
        if (serviceIds != null && serviceIds.Any())
        {
            var staffServices = serviceIds.Select(serviceId => new StaffService
            {
                StaffId = staff.Id,
                ServiceId = serviceId
            });
            _context.StaffServices.AddRange(staffServices);
            await _context.SaveChangesAsync();
        }

        return await GetByIdAsync(staff.Id);
    }

    public async Task<Staff> UpdateAsync(Staff staff)
    {
        _context.Staff.Update(staff);
        await _context.SaveChangesAsync();
        return staff;
    }

    public async Task DeleteAsync(int id)
    {
        var staff = await _context.Staff.FindAsync(id);
        if (staff != null)
        {
            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteWithUserAsync(int id)
    {
        // Delete related entities first
        var staffServices = await _context.StaffServices.Where(ss => ss.StaffId == id).ToListAsync();
        _context.StaffServices.RemoveRange(staffServices);

        var workingHours = await _context.WorkingHours.Where(wh => wh.StaffId == id).ToListAsync();
        _context.WorkingHours.RemoveRange(workingHours);

        var staff = await _context.Staff.FindAsync(id);
        if (staff != null)
        {
            _context.Staff.Remove(staff);
        }

        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
        }

        await _context.SaveChangesAsync();
    }
}
