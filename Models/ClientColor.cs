using System.ComponentModel.DataAnnotations;

namespace salon_manager.Models;

public class ClientColor
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public Client? Client { get; set; }

    [Required]
    [MaxLength(200)]
    public string Color { get; set; } = string.Empty;
}
