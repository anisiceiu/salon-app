namespace SalonApp.Application.DTOs;

public class PopularServiceDto
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public int BookingCount { get; set; }
    public decimal TotalRevenue { get; set; }
}
