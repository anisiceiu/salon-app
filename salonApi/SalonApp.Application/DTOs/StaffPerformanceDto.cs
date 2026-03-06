namespace SalonApp.Application.DTOs;

public class StaffPerformanceDto
{
    public int StaffId { get; set; }
    public string StaffName { get; set; } = string.Empty;
    public int CompletedAppointments { get; set; }
    public decimal TotalRevenue { get; set; }
}
