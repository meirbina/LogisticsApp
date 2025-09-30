using Microsoft.AspNetCore.Identity;

namespace SMS.Models;

public class ApplicationUser : IdentityUser
{
    // Custom properties can be added here later if needed
    public int? ParentId { get; set; }
    public Parent? Parent { get; set; }
}