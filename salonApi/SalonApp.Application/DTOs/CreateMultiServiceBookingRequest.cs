namespace SalonApp.Application.DTOs;

public class CreateMultiServiceBookingRequest
{
    public List<int> ServiceIds { get; set; } = new List<int>();
    public int? PreferredStaffId { get; set; }
    public DateTime DateTime { get; set; }
    public string? Notes { get; set; }
}
