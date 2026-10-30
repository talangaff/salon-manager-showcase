using System.ComponentModel.DataAnnotations;

namespace salon_manager.Models;

public class ClientToner
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public Client? Client { get; set; }

    [Required]
    [MaxLength(200)]
    public string Toner { get; set; } = string.Empty;
}
