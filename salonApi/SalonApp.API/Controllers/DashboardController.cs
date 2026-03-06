using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonApp.Application.DTOs;
using SalonApp.Application.Interfaces;

namespace SalonApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    // GET /api/dashboard/overview - Dashboard overview (Admin)
    [HttpGet("overview")]
    public async Task<ActionResult<DashboardOverviewDto>> GetDashboardOverview()
    {
        var overview = await _dashboardService.GetDashboardOverviewAsync();
        return Ok(overview);
    }

    // GET /api/dashboard/today - Today's appointments (Admin)
    [HttpGet("today")]
    public async Task<ActionResult<IEnumerable<TodayAppointmentDto>>> GetTodayAppointments()
    {
        var appointments = await _dashboardService.GetTodayAppointmentsAsync();
        return Ok(appointments);
    }

    // GET /api/dashboard/revenue?startDate=&endDate= - Revenue report (Admin)
    [HttpGet("revenue")]
    public async Task<ActionResult<IEnumerable<RevenueSummaryDto>>> GetRevenueSummary(
        [FromQuery] string? startDate, 
        [FromQuery] string? endDate)
    {
        DateTime start;
        DateTime end;

        // Default to last 30 days if not provided
        if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
        {
            end = DateTime.UtcNow;
            start = end.AddDays(-30);
        }
        else
        {
            if (!DateTime.TryParse(startDate, out start))
            {
                return BadRequest(new { message = "Invalid startDate format. Use YYYY-MM-DD" });
            }
            if (!DateTime.TryParse(endDate, out end))
            {
                return BadRequest(new { message = "Invalid endDate format. Use YYYY-MM-DD" });
            }
        }

        var revenue = await _dashboardService.GetRevenueSummaryAsync(start, end);
        return Ok(revenue);
    }

    // GET /api/dashboard/services?startDate=&endDate= - Popular services (Admin)
    [HttpGet("services")]
    public async Task<ActionResult<IEnumerable<PopularServiceDto>>> GetPopularServices(
        [FromQuery] string? startDate, 
        [FromQuery] string? endDate)
    {
        DateTime start;
        DateTime end;

        // Default to last 30 days if not provided
        if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
        {
            end = DateTime.UtcNow;
            start = end.AddDays(-30);
        }
        else
        {
            if (!DateTime.TryParse(startDate, out start))
            {
                return BadRequest(new { message = "Invalid startDate format. Use YYYY-MM-DD" });
            }
            if (!DateTime.TryParse(endDate, out end))
            {
                return BadRequest(new { message = "Invalid endDate format. Use YYYY-MM-DD" });
            }
        }

        var services = await _dashboardService.GetPopularServicesAsync(start, end);
        return Ok(services);
    }

    // GET /api/dashboard/staff?startDate=&endDate= - Staff performance (Admin)
    [HttpGet("staff")]
    public async Task<ActionResult<IEnumerable<StaffPerformanceDto>>> GetStaffPerformance(
        [FromQuery] string? startDate, 
        [FromQuery] string? endDate)
    {
        DateTime start;
        DateTime end;

        // Default to last 30 days if not provided
        if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
        {
            end = DateTime.UtcNow;
            start = end.AddDays(-30);
        }
        else
        {
            if (!DateTime.TryParse(startDate, out start))
            {
                return BadRequest(new { message = "Invalid startDate format. Use YYYY-MM-DD" });
            }
            if (!DateTime.TryParse(endDate, out end))
            {
                return BadRequest(new { message = "Invalid endDate format. Use YYYY-MM-DD" });
            }
        }

        var staffPerformance = await _dashboardService.GetStaffPerformanceAsync(start, end);
        return Ok(staffPerformance);
    }
}
