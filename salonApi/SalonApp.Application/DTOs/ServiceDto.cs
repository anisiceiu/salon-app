namespace SalonApp.Application.DTOs;

public class ServiceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Duration { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
