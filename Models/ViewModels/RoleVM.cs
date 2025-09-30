using Microsoft.AspNetCore.Identity;

namespace SMS.Models.ViewModels
{
    public class RoleVM
    {
        public IdentityRole NewRole { get; set; } = new();
        public IEnumerable<IdentityRole> Roles { get; set; } = new List<IdentityRole>();
    }
}