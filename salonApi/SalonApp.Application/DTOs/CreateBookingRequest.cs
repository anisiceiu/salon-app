namespace SalonApp.Application.DTOs;

public class CreateBookingRequest
{
    public int StaffId { get; set; }
    public int ServiceId { get; set; }
    public DateTime DateTime { get; set; }
    public string? Notes { get; set; }
}
