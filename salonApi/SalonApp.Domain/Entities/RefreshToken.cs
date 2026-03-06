using System;

namespace SalonApp.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public int UserId { get; set; }
    public bool IsRevoked { get; set; } = false;
    public string CreatedByIp { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? RevokedByIp { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}
