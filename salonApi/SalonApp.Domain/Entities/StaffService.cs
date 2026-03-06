namespace SalonApp.Domain.Entities;

public class StaffService
{
    public int StaffId { get; set; }
    public int ServiceId { get; set; }

    // Navigation properties
    public Staff? Staff { get; set; }
    public Service? Service { get; set; }
}
