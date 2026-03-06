using SalonApp.Application.DTOs;
using SalonApp.Application.Interfaces;
using SalonApp.Domain.Entities;
using SalonApp.Domain.Enums;

namespace SalonApp.Application.Services;

public class BookingService : IBookingService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IWorkingHoursRepository _workingHoursRepository;
    private readonly ILeaveRepository _leaveRepository;
    private readonly IStaffServiceRepository _staffServiceRepository;

    // Configuration
    private const int SlotDurationMinutes = 30;
    private const int BufferTimeMinutes = 15;
    private const int CancellationWindowHours = 24;

    public BookingService(
        IAppointmentRepository appointmentRepository,
        IServiceRepository serviceRepository,
        IStaffRepository staffRepository,
        IWorkingHoursRepository workingHoursRepository,
        ILeaveRepository leaveRepository,
        IStaffServiceRepository staffServiceRepository)
    {
        _appointmentRepository = appointmentRepository;
        _serviceRepository = serviceRepository;
        _staffRepository = staffRepository;
        _workingHoursRepository = workingHoursRepository;
        _leaveRepository = leaveRepository;
        _staffServiceRepository = staffServiceRepository;
    }

    public async Task<IEnumerable<BookingSlotDto>> GetAvailableSlotsAsync(int staffId, int serviceId, DateTime date)
    {
        // Get service to determine duration
        var service = await _serviceRepository.GetByIdAsync(serviceId);
        if (service == null)
            throw new ArgumentException("Service not found");

        // Get staff working hours for the requested day
        var workingHours = await _workingHoursRepository.GetByStaffIdAsync(staffId);
        var dayWorkingHours = workingHours.FirstOrDefault(wh => wh.DayOfWeek == (int)date.DayOfWeek);

        // If staff doesn't work on this day or is not working, return empty
        if (dayWorkingHours == null || !dayWorkingHours.IsWorking)
            return Enumerable.Empty<BookingSlotDto>();

        // Check if staff has approved leave on this date
        var leaves = await _leaveRepository.GetByStaffIdAndDateRangeAsync(staffId, date, date);
        var hasApprovedLeave = leaves.Any();

        if (hasApprovedLeave)
            return Enumerable.Empty<BookingSlotDto>();

        // Get existing appointments for this staff on this date
        var existingAppointments = await _appointmentRepository.GetByStaffIdAndDateAsync(staffId, date);
        var activeAppointments = existingAppointments
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .ToList();

        // Generate available slots
        var slots = new List<BookingSlotDto>();
        var currentTime = dayWorkingHours.StartTime;
        var endTime = dayWorkingHours.EndTime;
        var serviceDuration = TimeSpan.FromMinutes(service.Duration);
        
        // Ensure service fits within working hours
        var lastPossibleStart = endTime - serviceDuration;

        while (currentTime <= lastPossibleStart)
        {
            var slotStart = currentTime;
            var slotEnd = currentTime + serviceDuration;
            var slotEndWithBuffer = slotEnd + TimeSpan.FromMinutes(BufferTimeMinutes);

            // Check if slot extends beyond working hours
            if (slotEnd > endTime)
                break;

            // Check if slot is in the past (for today)
            var isPastSlot = date.Date == DateTime.Today && slotStart < DateTime.Now.TimeOfDay;

            // Check for conflicts with existing appointments
            var isAvailable = !isPastSlot && !IsSlotConflicting(slotStart, slotEnd, activeAppointments);

            slots.Add(new BookingSlotDto
            {
                StartTime = date.Date.Add(slotStart),
                EndTime = date.Date.Add(slotEnd),
                IsAvailable = isAvailable
            });

            // Move to next slot (30-minute intervals)
            currentTime = currentTime.Add(TimeSpan.FromMinutes(SlotDurationMinutes));
        }

        return slots;
    }

    public async Task<IEnumerable<AutoAssignStaffResultDto>> GetAvailableStaffAsync(int serviceId, DateTime date)
    {
        // Get service to determine duration
        var service = await _serviceRepository.GetByIdAsync(serviceId);
        if (service == null)
            throw new ArgumentException("Service not found");

        // Get all staff who provide this service
        var staffServices = await _staffServiceRepository.GetByServiceIdAsync(serviceId);
        var staffIds = staffServices.Select(ss => ss.StaffId).ToList();

        var availableStaff = new List<AutoAssignStaffResultDto>();

        foreach (var staffId in staffIds)
        {
            // Get staff details
            var staff = await _staffRepository.GetByIdAsync(staffId);
            if (staff == null) continue;

            // Get staff working hours for the requested day
            var workingHours = await _workingHoursRepository.GetByStaffIdAsync(staffId);
            var dayWorkingHours = workingHours.FirstOrDefault(wh => wh.DayOfWeek == (int)date.DayOfWeek);

            // Skip if staff doesn't work on this day
            if (dayWorkingHours == null || !dayWorkingHours.IsWorking)
                continue;

            // Check if staff has approved leave on this date
            var leaves = await _leaveRepository.GetByStaffIdAndDateRangeAsync(staffId, date, date);
            var hasApprovedLeave = leaves.Any();

            // Skip if staff has approved leave
            if (hasApprovedLeave)
                continue;

            // Get existing appointments for this staff on this date
            var existingAppointments = await _appointmentRepository.GetByStaffIdAndDateAsync(staffId, date);
            var activeAppointments = existingAppointments
                .Where(a => a.Status != AppointmentStatus.Cancelled)
                .ToList();

            // Generate available slots
            var slots = new List<BookingSlotDto>();
            var currentTime = dayWorkingHours.StartTime;
            var endTime = dayWorkingHours.EndTime;
            var serviceDuration = TimeSpan.FromMinutes(service.Duration);

            // Ensure service fits within working hours
            var lastPossibleStart = endTime - serviceDuration;

            while (currentTime <= lastPossibleStart)
            {
                var slotStart = currentTime;
                var slotEnd = currentTime + serviceDuration;

                // Check if slot is in the past (for today)
                var isPastSlot = date.Date == DateTime.Today && slotStart < DateTime.Now.TimeOfDay;

                // Check for conflicts with existing appointments
                var isAvailable = !isPastSlot && !IsSlotConflicting(slotStart, slotEnd, activeAppointments);

                slots.Add(new BookingSlotDto
                {
                    StartTime = date.Date.Add(slotStart),
                    EndTime = date.Date.Add(slotEnd),
                    IsAvailable = isAvailable
                });

                // Move to next slot (30-minute intervals)
                currentTime = currentTime.Add(TimeSpan.FromMinutes(SlotDurationMinutes));
            }

            // Only add staff who has at least one available slot
            var availableSlots = slots.Where(s => s.IsAvailable).ToList();
            if (availableSlots.Any())
            {
                availableStaff.Add(new AutoAssignStaffResultDto
                {
                    StaffId = staffId,
                    StaffName = staff.User?.FullName ?? "Unknown",
                    AvailableSlots = availableSlots,
                    IsAvailable = true
                });
            }
        }

        // Sort by number of available slots (most available first)
        return availableStaff.OrderByDescending(s => s.AvailableSlots.Count);
    }

    private bool IsSlotConflicting(TimeSpan slotStart, TimeSpan slotEnd, List<Appointment> activeAppointments)
    {
        foreach (var appointment in activeAppointments)
        {
            // Add buffer time to existing appointment end time
            var appointmentEndWithBuffer = appointment.EndTime + TimeSpan.FromMinutes(BufferTimeMinutes);

            // Check for overlap: slotStart < appointmentEndWithBuffer && slotEnd > appointment.StartTime
            if (slotStart < appointmentEndWithBuffer && slotEnd > appointment.StartTime)
                return true;
        }
        return false;
    }

    public async Task<AppointmentDto> CreateBookingAsync(int customerId, CreateBookingRequest request)
    {
        // Validate booking is in future
        if (request.DateTime < DateTime.UtcNow)
            throw new ArgumentException("Cannot book appointments in the past");

        // Get service
        var service = await _serviceRepository.GetByIdAsync(request.ServiceId);
        if (service == null)
            throw new ArgumentException("Service not found");

        // Get staff
        var staff = await _staffRepository.GetByIdAsync(request.StaffId);
        if (staff == null)
            throw new ArgumentException("Staff not found");

        // Check if staff provides this service
        var staffProvidesService = staff.StaffServices.Any(ss => ss.ServiceId == request.ServiceId);
        if (!staffProvidesService)
            throw new ArgumentException("Staff does not provide this service");

        // Check working hours
        var workingHours = await _workingHoursRepository.GetByStaffIdAsync(request.StaffId);
        var dayWorkingHours = workingHours.FirstOrDefault(wh => 
            wh.DayOfWeek == (int)request.DateTime.DayOfWeek);

        if (dayWorkingHours == null || !dayWorkingHours.IsWorking)
            throw new ArgumentException("Staff is not working on this day");

        // Check if within working hours
        var appointmentTime = request.DateTime.TimeOfDay;
        var serviceDuration = TimeSpan.FromMinutes(service.Duration);
        var appointmentEndTime = appointmentTime + serviceDuration;

        if (appointmentTime < dayWorkingHours.StartTime || appointmentEndTime > dayWorkingHours.EndTime)
            throw new ArgumentException("Appointment time is outside working hours");

        // Check for approved leave
        var leaves = await _leaveRepository.GetByStaffIdAndDateRangeAsync(
            request.StaffId, request.DateTime, request.DateTime);
        var hasApprovedLeave = leaves.Any();

        if (hasApprovedLeave)
            throw new ArgumentException("Staff is on leave on this date");

        // Check for double booking
        var existingAppointments = await _appointmentRepository.GetByStaffIdAndDateAsync(
            request.StaffId, request.DateTime);

        var hasConflict = existingAppointments
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .Any(a => IsSlotConflicting(appointmentTime, appointmentEndTime, 
                new List<Appointment> { a }));

        if (hasConflict)
            throw new ArgumentException("Time slot is not available");

        // Create appointment
        var appointment = new Appointment
        {
            CustomerId = customerId,
            StaffId = request.StaffId,
            ServiceId = request.ServiceId,
            AppointmentDate = request.DateTime.Date,
            StartTime = appointmentTime,
            EndTime = appointmentEndTime,
            Status = AppointmentStatus.Pending,
            TotalPrice = service.Price,
            FinalPrice = service.Price,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        var createdAppointment = await _appointmentRepository.AddAsync(appointment);
        
        // Load related data for response
        var fullAppointment = await _appointmentRepository.GetByIdAsync(createdAppointment.Id);
        
        return MapToDto(fullAppointment!);
    }

    public async Task<AppointmentDto> ConfirmBookingAsync(int appointmentId)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment == null)
            throw new ArgumentException("Appointment not found");

        if (appointment.Status != AppointmentStatus.Pending)
            throw new ArgumentException("Only pending appointments can be confirmed");

        appointment.Status = AppointmentStatus.Confirmed;
        var updated = await _appointmentRepository.UpdateAsync(appointment);
        
        return MapToDto(updated);
    }

    public async Task<AppointmentDto> CancelBookingAsync(int appointmentId, string reason)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment == null)
            throw new ArgumentException("Appointment not found");

        if (appointment.Status == AppointmentStatus.Cancelled)
            throw new ArgumentException("Appointment is already cancelled");

        if (appointment.Status == AppointmentStatus.Completed)
            throw new ArgumentException("Cannot cancel completed appointments");

        // Check cancellation window (24 hours before appointment)
        var appointmentDateTime = appointment.AppointmentDate.Add(appointment.StartTime);
        var hoursUntilAppointment = (appointmentDateTime - DateTime.UtcNow).TotalHours;

        if (hoursUntilAppointment < CancellationWindowHours)
            throw new ArgumentException($"Cannot cancel within {CancellationWindowHours} hours of appointment");

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.Notes = string.IsNullOrEmpty(appointment.Notes) 
            ? $"Cancelled: {reason}" 
            : $"{appointment.Notes}\nCancelled: {reason}";

        var updated = await _appointmentRepository.UpdateAsync(appointment);
        
        return MapToDto(updated);
    }

    public async Task<AppointmentDto> RescheduleBookingAsync(int appointmentId, DateTime newDateTime)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment == null)
            throw new ArgumentException("Appointment not found");

        if (appointment.Status == AppointmentStatus.Cancelled)
            throw new ArgumentException("Cannot reschedule cancelled appointment");

        if (appointment.Status == AppointmentStatus.Completed)
            throw new ArgumentException("Cannot reschedule completed appointment");

        // Check cancellation window
        var appointmentDateTime = appointment.AppointmentDate.Add(appointment.StartTime);
        var hoursUntilAppointment = (appointmentDateTime - DateTime.UtcNow).TotalHours;

        if (hoursUntilAppointment < CancellationWindowHours)
            throw new ArgumentException($"Cannot reschedule within {CancellationWindowHours} hours of appointment");

        // Validate new time is in future
        if (newDateTime < DateTime.UtcNow)
            throw new ArgumentException("Cannot reschedule to past time");

        // Get service for duration
        var service = await _serviceRepository.GetByIdAsync(appointment.ServiceId);
        if (service == null)
            throw new ArgumentException("Service not found");

        // Check new time is within working hours
        var workingHours = await _workingHoursRepository.GetByStaffIdAsync(appointment.StaffId);
        var dayWorkingHours = workingHours.FirstOrDefault(wh => 
            wh.DayOfWeek == (int)newDateTime.DayOfWeek);

        if (dayWorkingHours == null || !dayWorkingHours.IsWorking)
            throw new ArgumentException("Staff is not working on this day");

        var newStartTime = newDateTime.TimeOfDay;
        var newEndTime = newStartTime + TimeSpan.FromMinutes(service.Duration);

        if (newStartTime < dayWorkingHours.StartTime || newEndTime > dayWorkingHours.EndTime)
            throw new ArgumentException("New appointment time is outside working hours");

        // Check for conflicts
        var existingAppointments = await _appointmentRepository.GetByStaffIdAndDateAsync(
            appointment.StaffId, newDateTime);

        var hasConflict = existingAppointments
            .Where(a => a.Status != AppointmentStatus.Cancelled && a.Id != appointmentId)
            .Any(a => IsSlotConflicting(newStartTime, newEndTime, 
                new List<Appointment> { a }));

        if (hasConflict)
            throw new ArgumentException("New time slot is not available");

        // Update appointment
        appointment.AppointmentDate = newDateTime.Date;
        appointment.StartTime = newStartTime;
        appointment.EndTime = newEndTime;

        var updated = await _appointmentRepository.UpdateAsync(appointment);
        
        return MapToDto(updated);
    }

    public async Task<AppointmentDto> CompleteBookingAsync(int appointmentId)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment == null)
            throw new ArgumentException("Appointment not found");

        if (appointment.Status != AppointmentStatus.Confirmed)
            throw new ArgumentException("Only confirmed appointments can be marked as completed");

        appointment.Status = AppointmentStatus.Completed;
        var updated = await _appointmentRepository.UpdateAsync(appointment);
        
        return MapToDto(updated);
    }

    public async Task<AppointmentDto> MarkNoShowAsync(int appointmentId)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment == null)
            throw new ArgumentException("Appointment not found");

        if (appointment.Status != AppointmentStatus.Pending && appointment.Status != AppointmentStatus.Confirmed)
            throw new ArgumentException("Only pending or confirmed appointments can be marked as no-show");

        appointment.Status = AppointmentStatus.NoShow;
        var updated = await _appointmentRepository.UpdateAsync(appointment);
        
        return MapToDto(updated);
    }

    public async Task<IEnumerable<AppointmentDto>> GetCustomerBookingsAsync(int customerId)
    {
        var appointments = await _appointmentRepository.GetByCustomerIdAsync(customerId);
        return appointments.Select(MapToDto);
    }

    public async Task<IEnumerable<AppointmentDto>> GetStaffAppointmentsAsync(int staffId)
    {
        var appointments = await _appointmentRepository.GetByStaffIdAsync(staffId);
        return appointments.Select(MapToDto);
    }

    public async Task<AppointmentDto?> GetAppointmentByIdAsync(int appointmentId)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        return appointment == null ? null : MapToDto(appointment);
    }

    private static AppointmentDto MapToDto(Appointment appointment)
    {
        return new AppointmentDto
        {
            Id = appointment.Id,
            CustomerId = appointment.CustomerId,
            CustomerName = appointment.Customer?.FullName ?? "Unknown",
            CustomerPhone = appointment.Customer?.Phone ?? "",
            StaffId = appointment.StaffId,
            StaffName = appointment.Staff?.User?.FullName ?? "Unknown",
            ServiceId = appointment.ServiceId,
            ServiceName = appointment.Service?.Name ?? "Unknown",
            AppointmentDate = appointment.AppointmentDate,
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            Status = appointment.Status,
            TotalPrice = appointment.TotalPrice,
            FinalPrice = appointment.FinalPrice,
            Notes = appointment.Notes,
            CreatedAt = appointment.CreatedAt,
            UpdatedAt = appointment.UpdatedAt
        };
    }
}
