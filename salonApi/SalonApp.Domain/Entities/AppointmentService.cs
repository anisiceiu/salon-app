namespace SalonApp.Domain.Entities;

public class AppointmentService
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public int ServiceId { get; set; }
    public int OrderIndex { get; set; } // To maintain order of services in the booking

    // Navigation properties
    public Appointment? Appointment { get; set; }
    public Service? Service { get; set; }
}
