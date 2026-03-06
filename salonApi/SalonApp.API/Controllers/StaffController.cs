using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonApp.Application.DTOs;
using SalonApp.Application.Interfaces;
using SalonApp.Domain.Enums;
using System.Security.Claims;

namespace SalonApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;

    public StaffController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    // GET /api/staff - Get all staff (Public)
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<StaffDto>>> GetAllStaff()
    {
        var staff = await _staffService.GetAllStaffAsync();
        return Ok(staff);
    }

    // GET /api/staff/{id} - Get staff by ID (Public)
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<StaffDto>> GetStaff(int id)
    {
        var staff = await _staffService.GetStaffByIdAsync(id);
        if (staff == null)
        {
            return NotFound(new { message = "Staff not found" });
        }
        return Ok(staff);
    }

    // GET /api/staff/{id}/services - Get staff's assigned services (Public)
    [HttpGet("{id}/services")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetStaffServices(int id)
    {
        var services = await _staffService.GetStaffServicesAsync(id);
        return Ok(services);
    }

    // GET /api/staff/{id}/availability - Get staff working hours (Public)
    [HttpGet("{id}/availability")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<WorkingHoursDto>>> GetStaffAvailability(int id)
    {
        var workingHours = await _staffService.GetWorkingHoursAsync(id);
        return Ok(workingHours);
    }

    // POST /api/staff - Create new staff (Admin only)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StaffDto>> CreateStaff([FromBody] CreateStaffRequest request)
    {
        try
        {
            var staff = await _staffService.CreateStaffAsync(request);
            return CreatedAtAction(nameof(GetStaff), new { id = staff.Id }, staff);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT /api/staff/{id} - Update staff (Admin only)
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StaffDto>> UpdateStaff(int id, [FromBody] UpdateStaffRequest request)
    {
        try
        {
            var staff = await _staffService.UpdateStaffAsync(id, request);
            return Ok(staff);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // DELETE /api/staff/{id} - Delete staff (Admin only)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteStaff(int id)
    {
        var result = await _staffService.DeleteStaffAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Staff not found" });
        }
        return NoContent();
    }

    // PUT /api/staff/{id}/services - Assign services to staff (Admin only)
    [HttpPut("{id}/services")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignServices(int id, [FromBody] AssignServicesRequest request)
    {
        try
        {
            await _staffService.AssignServicesAsync(id, request);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT /api/staff/{id}/working-hours - Set working hours (Admin only)
    [HttpPut("{id}/working-hours")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetWorkingHours(int id, [FromBody] SetWorkingHoursRequest request)
    {
        try
        {
            await _staffService.SetWorkingHoursAsync(id, request);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST /api/staff/leave - Request leave (Staff only)
    [HttpPost("leave")]
    [Authorize(Roles = "Staff,Admin")]
    public async Task<ActionResult<LeaveDto>> RequestLeave([FromBody] CreateLeaveRequest request)
    {
        try
        {
            var staffId = GetCurrentUserId();
            var leave = await _staffService.RequestLeaveAsync(staffId, request);
            return Ok(leave);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT /api/staff/leave/{leaveId}/approve - Approve/reject leave (Admin only)
    [HttpPut("leave/{leaveId}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<LeaveDto>> ApproveLeave(int leaveId, [FromBody] ApproveLeaveRequest request)
    {
        var leave = await _staffService.ApproveLeaveAsync(leaveId, request);
        if (leave == null)
        {
            return NotFound(new { message = "Leave request not found" });
        }
        return Ok(leave);
    }

    // GET /api/staff/leave - Get all leave requests (Admin only)
    [HttpGet("leave")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<LeaveDto>>> GetAllLeaves()
    {
        var leaves = await _staffService.GetAllLeavesAsync();
        return Ok(leaves);
    }

    // GET /api/staff/leave/my - Get my leave requests (Staff only)
    [HttpGet("leave/my")]
    [Authorize(Roles = "Staff,Admin")]
    public async Task<ActionResult<IEnumerable<LeaveDto>>> GetMyLeaves()
    {
        var staffId = GetCurrentUserId();
        var leaves = await _staffService.GetMyLeavesAsync(staffId);
        return Ok(leaves);
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new InvalidOperationException("User ID not found in token");
        }
        return int.Parse(userIdClaim);
    }
}
