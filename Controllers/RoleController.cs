using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS.Models.ViewModels;

namespace SMS.Controllers
{
    [Authorize(Roles = "Admin")] // Only Admins can access
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new RoleVM
            {
                Roles = await _roleManager.Roles.ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleVM vm)
        {
            if (!string.IsNullOrEmpty(vm.NewRole.Name))
            {
                if (!await _roleManager.RoleExistsAsync(vm.NewRole.Name))
                {
                    await _roleManager.CreateAsync(new IdentityRole(vm.NewRole.Name));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}