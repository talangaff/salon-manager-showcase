using Microsoft.AspNetCore.Identity;

namespace salon_manager.Models;

public class ApplicationUser : IdentityUser
{
    public bool MustChangePassword { get; set; } = true;
}
