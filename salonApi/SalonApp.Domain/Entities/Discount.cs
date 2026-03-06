using SalonApp.Domain.Enums;

namespace SalonApp.Domain.Entities;

public class Discount
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinOrderValue { get; set; } = 0;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
    public bool IsActive { get; set; } = true;
    public int? MaxUses { get; set; }
    public int UsedCount { get; set; } = 0;
}
