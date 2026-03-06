using SalonApp.Application.DTOs;

namespace SalonApp.Application.DTOs;

public class StaffDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? PhotoUrl { get; set; }
    public bool IsActive { get; set; }
    public List<ServiceDto> Services { get; set; } = new();
    public List<WorkingHoursDto> WorkingHours { get; set; } = new();
}
