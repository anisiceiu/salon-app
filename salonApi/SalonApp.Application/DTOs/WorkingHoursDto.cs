namespace SalonApp.Application.DTOs;

public class WorkingHoursDto
{
    public int Id { get; set; }
    public int DayOfWeek { get; set; } // 0-6 (Sunday-Saturday)
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsWorking { get; set; }
    public string? DayName { get; set; }
}
