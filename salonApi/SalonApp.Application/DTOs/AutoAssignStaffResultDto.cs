namespace SalonApp.Application.DTOs;

public class AutoAssignStaffResultDto
{
    public int StaffId { get; set; }
    public string StaffName { get; set; } = string.Empty;
    public List<BookingSlotDto> AvailableSlots { get; set; } = new();
    public bool IsAvailable { get; set; }
}
