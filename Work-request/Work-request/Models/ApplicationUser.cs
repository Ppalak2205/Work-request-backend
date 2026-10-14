using Microsoft.AspNetCore.Identity;

namespace Work_request.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}
