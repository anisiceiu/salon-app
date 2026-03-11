namespace SalonApp.Application.DTOs;

public class MultiServiceSlotDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int TotalDurationMinutes { get; set; }
    public decimal TotalPrice { get; set; }
    public List<ServiceDurationInfo> Services { get; set; } = new List<ServiceDurationInfo>();
    public bool IsAvailable { get; set; }
}

public class ServiceDurationInfo
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
}
