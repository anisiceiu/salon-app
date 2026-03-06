using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonApp.Application.DTOs;
using SalonApp.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalonApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _serviceService;

    public ServicesController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    // ==================== Services ====================

    /// <summary>
    /// Get all services (public)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetAllServices()
    {
        var services = await _serviceService.GetAllServicesAsync();
        return Ok(services);
    }

    /// <summary>
    /// Get service by ID (public)
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceDto>> GetService(int id)
    {
        var service = await _serviceService.GetServiceByIdAsync(id);
        if (service == null)
        {
            return NotFound(new { message = "Service not found" });
        }
        return Ok(service);
    }

    /// <summary>
    /// Get services by category (public)
    /// </summary>
    [HttpGet("category/{categoryId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServicesByCategory(int categoryId)
    {
        var services = await _serviceService.GetServicesByCategoryAsync(categoryId);
        return Ok(services);
    }

    /// <summary>
    /// Create a new service (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceDto>> CreateService([FromBody] CreateServiceRequest request)
    {
        try
        {
            var service = await _serviceService.CreateServiceAsync(request);
            return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update a service (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceDto>> UpdateService(int id, [FromBody] UpdateServiceRequest request)
    {
        try
        {
            var service = await _serviceService.UpdateServiceAsync(id, request);
            return Ok(service);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a service (Admin only) - Soft delete
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var result = await _serviceService.DeleteServiceAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Service not found" });
        }
        return NoContent();
    }

    // ==================== Categories ====================

    /// <summary>
    /// Get all categories (public)
    /// </summary>
    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ServiceCategoryDto>>> GetAllCategories()
    {
        var categories = await _serviceService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Get category by ID (public)
    /// </summary>
    [HttpGet("categories/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceCategoryDto>> GetCategory(int id)
    {
        var category = await _serviceService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound(new { message = "Category not found" });
        }
        return Ok(category);
    }

    /// <summary>
    /// Create a new category (Admin only)
    /// </summary>
    [HttpPost("categories")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceCategoryDto>> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        try
        {
            var category = await _serviceService.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update a category (Admin only)
    /// </summary>
    [HttpPut("categories/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceCategoryDto>> UpdateCategory(int id, [FromBody] UpdateCategoryRequest request)
    {
        try
        {
            var category = await _serviceService.UpdateCategoryAsync(id, request);
            return Ok(category);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a category (Admin only) - Soft delete
    /// </summary>
    [HttpDelete("categories/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var result = await _serviceService.DeleteCategoryAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Category not found" });
        }
        return NoContent();
    }
}
