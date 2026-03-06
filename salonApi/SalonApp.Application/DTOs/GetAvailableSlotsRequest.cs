namespace SalonApp.Application.DTOs;

public class GetAvailableSlotsRequest
{
    public int StaffId { get; set; }
    public int ServiceId { get; set; }
    public DateTime Date { get; set; }
}
