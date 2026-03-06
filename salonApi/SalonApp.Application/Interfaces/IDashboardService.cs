using SalonApp.Application.DTOs;

namespace SalonApp.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardOverviewDto> GetDashboardOverviewAsync();
    Task<IEnumerable<TodayAppointmentDto>> GetTodayAppointmentsAsync();
    Task<IEnumerable<RevenueSummaryDto>> GetRevenueSummaryAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<PopularServiceDto>> GetPopularServicesAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<StaffPerformanceDto>> GetStaffPerformanceAsync(DateTime startDate, DateTime endDate);
}
