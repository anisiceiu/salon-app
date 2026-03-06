using SalonApp.Domain.Enums;

namespace SalonApp.Domain.Entities;

public class Leave
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Staff? Staff { get; set; }
}
