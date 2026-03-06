namespace SalonApp.Application.DTOs;

public class DashboardOverviewDto
{
    public int TotalCustomers { get; set; }
    public int TotalStaff { get; set; }
    public int TodayAppointments { get; set; }
    public decimal TodayRevenue { get; set; }
    public int PendingAppointments { get; set; }
}
