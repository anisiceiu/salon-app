namespace SalonApp.Application.DTOs;

public class SetWorkingHoursRequest
{
    public List<WorkingHoursItemDto> WorkingHours { get; set; } = new();
}

public class WorkingHoursItemDto
{
    public int DayOfWeek { get; set; } // 0-6 (Sunday-Saturday)
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsWorking { get; set; } = true;
}
