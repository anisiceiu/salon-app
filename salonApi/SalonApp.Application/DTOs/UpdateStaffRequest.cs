namespace SalonApp.Application.DTOs;

public class UpdateStaffRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? PhotoUrl { get; set; }
    public bool IsActive { get; set; }
    public List<int>? ServiceIds { get; set; }
}
