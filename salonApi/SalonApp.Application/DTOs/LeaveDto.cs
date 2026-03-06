using SalonApp.Domain.Enums;

namespace SalonApp.Application.DTOs;

public class LeaveDto
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public string? StaffName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }
    public LeaveStatus Status { get; set; }
    public int? ApprovedBy { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime CreatedAt { get; set; }
}
