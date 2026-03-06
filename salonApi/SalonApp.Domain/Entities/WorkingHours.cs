namespace SalonApp.Domain.Entities;

public class WorkingHours
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public int DayOfWeek { get; set; } // 0-6 (Sunday-Saturday)
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsWorking { get; set; } = true;

    // Navigation properties
    public Staff? Staff { get; set; }
}
