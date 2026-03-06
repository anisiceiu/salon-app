namespace SalonApp.Application.DTOs;

public class TodayAppointmentDto
{
    public int AppointmentId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string StaffName { get; set; } = string.Empty;
    public TimeSpan Time { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
