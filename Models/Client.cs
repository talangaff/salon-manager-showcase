using System.ComponentModel.DataAnnotations;

namespace salon_manager.Models;

public class Client
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public ICollection<ClientColor> Colors { get; set; } = new List<ClientColor>();
    public ICollection<ClientToner> Toners { get; set; } = new List<ClientToner>();
}
