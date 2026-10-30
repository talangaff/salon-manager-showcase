using System.ComponentModel.DataAnnotations;

namespace salon_manager.Models;

public class ClientFormViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    public List<string> Colors { get; set; } = new();
    public List<string> Toners { get; set; } = new();
}
