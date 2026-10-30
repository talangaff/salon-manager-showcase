using System.ComponentModel.DataAnnotations;

namespace salon_manager.Models;

public enum PaymentMethod
{
    Card,
    Cash
}

public class Tip
{
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; } = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);

    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    // Final tip amount (entered directly for Card, computed for Cash)
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Tip amount must be non-negative.")]
    public decimal TipAmount { get; set; }

    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    // Cash payment fields only
    [Range(0, double.MaxValue)]
    public decimal? TotalCash { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? ServiceCost { get; set; }
}
