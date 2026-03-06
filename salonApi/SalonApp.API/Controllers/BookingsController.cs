using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonApp.Application.DTOs;
using SalonApp.Application.Interfaces;
using SalonApp.Domain.Enums;
using System.Security.Claims;

namespace SalonApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IStaffService _staffService;

    public BookingsController(IBookingService bookingService, IStaffService staffService)
    {
        _bookingService = bookingService;
        _staffService = staffService;
    }

    // ==================== Public Endpoints ====================

    /// <summary>
    /// Get available slots for a staff member on a specific date (Public)
    /// </summary>
    [HttpGet("slots")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<BookingSlotDto>>> GetAvailableSlots(
        [FromQuery] int staffId, 
        [FromQuery] int serviceId, 
        [FromQuery] DateTime date)
    {
        try
        {
            var slots = await _bookingService.GetAvailableSlotsAsync(staffId, serviceId, date);
            return Ok(slots);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Auto-assign staff - get all available staff for a service on a specific date (Public)
    /// </summary>
    [HttpGet("auto-assign")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AutoAssignStaffResultDto>>> GetAvailableStaff(
        [FromQuery] int serviceId, 
        [FromQuery] DateTime date)
    {
        try
        {
            var availableStaff = await _bookingService.GetAvailableStaffAsync(serviceId, date);
            return Ok(availableStaff);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ==================== Customer Endpoints ====================

    /// <summary>
    /// Create a new booking (Customer, Auth required)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Customer,Staff,Admin")]
    public async Task<ActionResult<AppointmentDto>> CreateBooking([FromBody] CreateBookingRequest request)
    {
        try
        {
            var customerId = GetCurrentUserId();
            var booking = await _bookingService.CreateBookingAsync(customerId, request);
            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get my bookings (Customer)
    /// </summary>
    [HttpGet("my")]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetMyBookings()
    {
        var customerId = GetCurrentUserId();
        var bookings = await _bookingService.GetCustomerBookingsAsync(customerId);
        return Ok(bookings);
    }

    /// <summary>
    /// Cancel own booking (Customer)
    /// </summary>
    [HttpPut("{id}/cancel")]
    [Authorize(Roles = "Customer,Staff,Admin")]
    public async Task<ActionResult<AppointmentDto>> CancelBooking(int id, [FromBody] CancelBookingRequest request)
    {
        try
        {
            var customerId = GetCurrentUserId();
            var booking = await _bookingService.GetAppointmentByIdAsync(id);
            
            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            // Check if user owns the booking or is staff/admin
            var userRole = GetCurrentUserRole();
            if (booking.CustomerId != customerId && userRole != "Staff" && userRole != "Admin")
                return Forbid();

            var updatedBooking = await _bookingService.CancelBookingAsync(id, request.Reason);
            return Ok(updatedBooking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Reschedule own booking (Customer)
    /// </summary>
    [HttpPut("{id}/reschedule")]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<AppointmentDto>> RescheduleBooking(int id, [FromBody] RescheduleBookingRequest request)
    {
        try
        {
            var customerId = GetCurrentUserId();
            var booking = await _bookingService.GetAppointmentByIdAsync(id);
            
            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            if (booking.CustomerId != customerId)
                return Forbid();

            var updatedBooking = await _bookingService.RescheduleBookingAsync(id, request.NewDateTime);
            return Ok(updatedBooking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ==================== Staff Endpoints ====================

    /// <summary>
    /// Get staff's appointments (Staff)
    /// </summary>
    [HttpGet("appointments")]
    [Authorize(Roles = "Staff,Admin")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetStaffAppointments()
    {
        var staffId = GetCurrentUserId();
        var appointments = await _bookingService.GetStaffAppointmentsAsync(staffId);
        return Ok(appointments);
    }

    /// <summary>
    /// Confirm booking (Staff/Admin)
    /// </summary>
    [HttpPut("{id}/confirm")]
    [Authorize(Roles = "Staff,Admin")]
    public async Task<ActionResult<AppointmentDto>> ConfirmBooking(int id)
    {
        try
        {
            var booking = await _bookingService.ConfirmBookingAsync(id);
            return Ok(booking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Complete booking (Staff)
    /// </summary>
    [HttpPut("{id}/complete")]
    [Authorize(Roles = "Staff,Admin")]
    public async Task<ActionResult<AppointmentDto>> CompleteBooking(int id)
    {
        try
        {
            var booking = await _bookingService.CompleteBookingAsync(id);
            return Ok(booking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Mark booking as no-show (Staff)
    /// </summary>
    [HttpPut("{id}/noshow")]
    [Authorize(Roles = "Staff,Admin")]
    public async Task<ActionResult<AppointmentDto>> MarkNoShow(int id)
    {
        try
        {
            var booking = await _bookingService.MarkNoShowAsync(id);
            return Ok(booking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ==================== Shared Endpoints ====================

    /// <summary>
    /// Get booking details (Owner/Staff/Admin)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Customer,Staff,Admin")]
    public async Task<ActionResult<AppointmentDto>> GetBooking(int id)
    {
        var booking = await _bookingService.GetAppointmentByIdAsync(id);
        
        if (booking == null)
            return NotFound(new { message = "Booking not found" });

        var customerId = GetCurrentUserId();
        var userRole = GetCurrentUserRole();

        // Only allow owner, staff assigned to booking, or admin
        if (booking.CustomerId != customerId && 
            (userRole != "Staff" && userRole != "Admin"))
        {
            return Forbid();
        }

        return Ok(booking);
    }

    // ==================== Admin Endpoints ====================

    /// <summary>
    /// Delete booking (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _bookingService.GetAppointmentByIdAsync(id);
        
        if (booking == null)
            return NotFound(new { message = "Booking not found" });

        // Use the repository directly to delete (need to add Delete method to service)
        // For now, we'll return NotImplemented - can be added later
        return StatusCode(501, new { message = "Delete functionality not yet implemented" });
    }

    // ==================== Helper Methods ====================

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new InvalidOperationException("User ID not found in token");
        }
        return int.Parse(userIdClaim);
    }

    private string GetCurrentUserRole()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        return roleClaim ?? string.Empty;
    }
}
