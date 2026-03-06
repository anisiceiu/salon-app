namespace SalonApp.Application.DTOs;

public class UpdateServiceRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Duration { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public bool IsActive { get; set; }
}
