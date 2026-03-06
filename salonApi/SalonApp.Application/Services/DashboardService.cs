using SalonApp.Application.DTOs;
using SalonApp.Application.Interfaces;
using SalonApp.Domain.Enums;

namespace SalonApp.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStaffRepository _staffRepository;

    public DashboardService(
        IAppointmentRepository appointmentRepository,
        IUserRepository userRepository,
        IStaffRepository staffRepository)
    {
        _appointmentRepository = appointmentRepository;
        _userRepository = userRepository;
        _staffRepository = staffRepository;
    }

    public async Task<DashboardOverviewDto> GetDashboardOverviewAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        // Get all users and filter by role
        // Note: In a real application, you would have a method like GetUsersByRoleAsync
        // For now, we'll count from appointments to get customers
        
        // Get all appointments for counting customers
        var allAppointments = await _appointmentRepository.GetAllAsync();
        var distinctCustomers = allAppointments.Select(a => a.CustomerId).Distinct().Count();
        
        // Get total staff count
        var allStaff = await _staffRepository.GetAllAsync();
        var totalStaff = allStaff.Count();

        // Get today's appointments
        var todayAppointments = await _appointmentRepository.GetByDateRangeAsync(today, tomorrow);
        var todayAppointmentList = todayAppointments.ToList();
        
        // Get today's completed appointments for revenue
        var completedToday = todayAppointmentList
            .Where(a => a.Status == AppointmentStatus.Completed)
            .ToList();

        var todayRevenue = completedToday.Sum(a => a.FinalPrice);
        var pendingCount = todayAppointmentList.Count(a => a.Status == AppointmentStatus.Pending);

        return new DashboardOverviewDto
        {
            TotalCustomers = distinctCustomers,
            TotalStaff = totalStaff,
            TodayAppointments = todayAppointmentList.Count,
            TodayRevenue = todayRevenue,
            PendingAppointments = pendingCount
        };
    }

    public async Task<IEnumerable<TodayAppointmentDto>> GetTodayAppointmentsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var appointments = await _appointmentRepository.GetByDateRangeAsync(today, tomorrow);
        
        return appointments.Select(a => new TodayAppointmentDto
        {
            AppointmentId = a.Id,
            CustomerName = a.Customer?.FullName ?? "Unknown",
            ServiceName = a.Service?.Name ?? "Unknown",
            StaffName = a.Staff?.User?.FullName ?? "Unknown",
            Time = a.StartTime,
            Status = a.Status.ToString(),
            Price = a.FinalPrice
        }).OrderBy(a => a.Time);
    }

    public async Task<IEnumerable<RevenueSummaryDto>> GetRevenueSummaryAsync(DateTime startDate, DateTime endDate)
    {
        // Ensure endDate includes the full day
        var endDateInclusive = endDate.Date.AddDays(1);
        
        var appointments = await _appointmentRepository.GetByDateRangeAsync(startDate.Date, endDateInclusive);
        var completedAppointments = appointments
            .Where(a => a.Status == AppointmentStatus.Completed)
            .ToList();

        // Group by date
        var revenueByDate = completedAppointments
            .GroupBy(a => a.AppointmentDate.Date)
            .Select(g => new RevenueSummaryDto
            {
                Date = g.Key,
                Revenue = g.Sum(a => a.FinalPrice),
                AppointmentCount = g.Count()
            })
            .OrderBy(r => r.Date);

        return revenueByDate;
    }

    public async Task<IEnumerable<PopularServiceDto>> GetPopularServicesAsync(DateTime startDate, DateTime endDate)
    {
        var endDateInclusive = endDate.Date.AddDays(1);
        
        var appointments = await _appointmentRepository.GetByDateRangeAsync(startDate.Date, endDateInclusive);
        
        var serviceStats = appointments
            .Where(a => a.Status == AppointmentStatus.Completed || a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Pending)
            .GroupBy(a => new { a.ServiceId, a.Service?.Name })
            .Select(g => new PopularServiceDto
            {
                ServiceId = g.Key.ServiceId,
                ServiceName = g.Key.Name ?? "Unknown",
                BookingCount = g.Count(),
                TotalRevenue = g.Where(a => a.Status == AppointmentStatus.Completed).Sum(a => a.FinalPrice)
            })
            .OrderByDescending(s => s.BookingCount);

        return serviceStats;
    }

    public async Task<IEnumerable<StaffPerformanceDto>> GetStaffPerformanceAsync(DateTime startDate, DateTime endDate)
    {
        var endDateInclusive = endDate.Date.AddDays(1);
        
        var appointments = await _appointmentRepository.GetByDateRangeAsync(startDate.Date, endDateInclusive);
        
        var staffStats = appointments
            .Where(a => a.Status == AppointmentStatus.Completed)
            .GroupBy(a => new { a.StaffId, a.Staff?.User?.FullName })
            .Select(g => new StaffPerformanceDto
            {
                StaffId = g.Key.StaffId,
                StaffName = g.Key.FullName ?? "Unknown",
                CompletedAppointments = g.Count(),
                TotalRevenue = g.Sum(a => a.FinalPrice)
            })
            .OrderByDescending(s => s.CompletedAppointments);

        return staffStats;
    }
}
