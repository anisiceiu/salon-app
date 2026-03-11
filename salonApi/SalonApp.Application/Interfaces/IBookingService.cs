using SalonApp.Application.DTOs;

namespace SalonApp.Application.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<BookingSlotDto>> GetAvailableSlotsAsync(int staffId, int serviceId, DateTime date);
    Task<IEnumerable<AutoAssignStaffResultDto>> GetAvailableStaffAsync(int serviceId, DateTime date);
    Task<AppointmentDto> CreateBookingAsync(int customerId, CreateBookingRequest request);
    Task<AppointmentDto> ConfirmBookingAsync(int appointmentId);
    Task<AppointmentDto> CancelBookingAsync(int appointmentId, string reason);
    Task<AppointmentDto> RescheduleBookingAsync(int appointmentId, DateTime newDateTime);
    Task<AppointmentDto> CompleteBookingAsync(int appointmentId);
    Task<AppointmentDto> MarkNoShowAsync(int appointmentId);
    Task<IEnumerable<AppointmentDto>> GetCustomerBookingsAsync(int customerId);
    Task<IEnumerable<AppointmentDto>> GetStaffAppointmentsAsync(int staffId);
    Task<AppointmentDto?> GetAppointmentByIdAsync(int appointmentId);
    
    // Multi-service booking methods
    Task<IEnumerable<MultiServiceSlotDto>> GetMultiServiceSlotsAsync(List<int> serviceIds, int? staffId, DateTime date);
    Task<IEnumerable<AutoAssignStaffResultDto>> GetAvailableStaffForMultipleServicesAsync(List<int> serviceIds, DateTime date);
    Task<AppointmentDto> CreateMultiServiceBookingAsync(int customerId, CreateMultiServiceBookingRequest request);
    Task<IEnumerable<ServiceDto>> GetAppointmentServicesAsync(int appointmentId);
}
