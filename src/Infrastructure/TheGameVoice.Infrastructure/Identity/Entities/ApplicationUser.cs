using Microsoft.AspNetCore.Identity;

namespace TheGameVoice.Infrastructure.Identity.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = default!;

    public bool IsActive { get; set; } = true;
}