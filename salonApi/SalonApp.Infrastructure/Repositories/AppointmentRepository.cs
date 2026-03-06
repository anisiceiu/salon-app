using Microsoft.EntityFrameworkCore;
using SalonApp.Application.Interfaces;
using SalonApp.Domain.Entities;
using SalonApp.Infrastructure.Data;

namespace SalonApp.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly ApplicationDbContext _context;

    public AppointmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        return await _context.Appointments
            .Include(a => a.Customer)
            .Include(a => a.Staff)
                .ThenInclude(s => s!.User)
            .Include(a => a.Service)
            .OrderByDescending(a => a.AppointmentDate)
            .ThenByDescending(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<Appointment?> GetByIdAsync(int id)
    {
        return await _context.Appointments
            .Include(a => a.Customer)
            .Include(a => a.Staff)
                .ThenInclude(s => s!.User)
            .Include(a => a.Service)
            .Include(a => a.Discount)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Appointment>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Appointments
            .Include(a => a.Customer)
            .Include(a => a.Staff)
                .ThenInclude(s => s!.User)
            .Include(a => a.Service)
            .Where(a => a.CustomerId == customerId)
            .OrderByDescending(a => a.AppointmentDate)
            .ThenByDescending(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByStaffIdAsync(int staffId)
    {
        return await _context.Appointments
            .Include(a => a.Customer)
            .Include(a => a.Staff)
                .ThenInclude(s => s!.User)
            .Include(a => a.Service)
            .Where(a => a.StaffId == staffId)
            .OrderByDescending(a => a.AppointmentDate)
            .ThenByDescending(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Appointments
            .Include(a => a.Customer)
            .Include(a => a.Staff)
                .ThenInclude(s => s!.User)
            .Include(a => a.Service)
            .Where(a => a.AppointmentDate >= startDate.Date && a.AppointmentDate <= endDate.Date)
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByStaffIdAndDateAsync(int staffId, DateTime date)
    {
        return await _context.Appointments
            .Include(a => a.Customer)
            .Include(a => a.Staff)
                .ThenInclude(s => s!.User)
            .Include(a => a.Service)
            .Where(a => a.StaffId == staffId && a.AppointmentDate.Date == date.Date)
            .OrderBy(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<Appointment> AddAsync(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }

    public async Task<Appointment> UpdateAsync(Appointment appointment)
    {
        appointment.UpdatedAt = DateTime.UtcNow;
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }

    public async Task DeleteAsync(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment != null)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }
    }
}
