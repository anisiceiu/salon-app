namespace SalonApp.Application.DTOs;

public class CreateLeaveRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }
}
