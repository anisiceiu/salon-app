using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalonApp.Application.DTOs;
using SalonApp.Application.Interfaces;

namespace SalonApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscountsController : ControllerBase
{
    private readonly IDiscountService _discountService;

    public DiscountsController(IDiscountService discountService)
    {
        _discountService = discountService;
    }

    // GET /api/discounts - Get all discounts (Admin)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<DiscountDto>>> GetAllDiscounts()
    {
        var discounts = await _discountService.GetAllDiscountsAsync();
        return Ok(discounts);
    }

    // GET /api/discounts/active - Get active discounts (Public)
    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<DiscountDto>>> GetActiveDiscounts()
    {
        var discounts = await _discountService.GetActiveDiscountsAsync();
        return Ok(discounts);
    }

    // GET /api/discounts/{id} - Get discount by ID (Admin)
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DiscountDto>> GetDiscount(int id)
    {
        var discount = await _discountService.GetDiscountByIdAsync(id);
        if (discount == null)
        {
            return NotFound(new { message = "Discount not found" });
        }
        return Ok(discount);
    }

    // GET /api/discounts/code/{code} - Get discount by code (Public)
    [HttpGet("code/{code}")]
    [AllowAnonymous]
    public async Task<ActionResult<DiscountDto>> GetDiscountByCode(string code)
    {
        var discount = await _discountService.GetDiscountByCodeAsync(code);
        if (discount == null)
        {
            return NotFound(new { message = "Discount not found" });
        }
        return Ok(discount);
    }

    // POST /api/discounts - Create new discount (Admin)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DiscountDto>> CreateDiscount([FromBody] CreateDiscountRequest request)
    {
        try
        {
            var discount = await _discountService.CreateDiscountAsync(request);
            return CreatedAtAction(nameof(GetDiscount), new { id = discount.Id }, discount);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT /api/discounts/{id} - Update discount (Admin)
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DiscountDto>> UpdateDiscount(int id, [FromBody] UpdateDiscountRequest request)
    {
        try
        {
            var discount = await _discountService.UpdateDiscountAsync(id, request);
            return Ok(discount);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE /api/discounts/{id} - Delete discount (Admin)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDiscount(int id)
    {
        var result = await _discountService.DeleteDiscountAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Discount not found" });
        }
        return NoContent();
    }
}
