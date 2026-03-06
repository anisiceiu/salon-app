namespace SalonApp.Application.DTOs;

public class RevenueSummaryDto
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int AppointmentCount { get; set; }
}
