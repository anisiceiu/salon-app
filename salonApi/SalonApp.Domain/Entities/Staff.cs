namespace SalonApp.Domain.Entities;

public class Staff
{
    public int Id { get; set; }
    public string? Bio { get; set; }
    public string? ProfileImage { get; set; }
    public bool IsAvailable { get; set; } = true;

    // Navigation properties
    public User? User { get; set; }
    public ICollection<StaffService> StaffServices { get; set; } = new List<StaffService>();
    public ICollection<WorkingHours> WorkingHours { get; set; } = new List<WorkingHours>();
    public ICollection<Leave> Leaves { get; set; } = new List<Leave>();
}
