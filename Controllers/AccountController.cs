// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using SMS.Models;
// using SMS.Models.ViewModels;
// using System.Threading.Tasks;
//
// namespace SMS.Controllers
// {
//     [Authorize] // Ensures only logged-in users can access this controller's actions
//     public class AccountController : Controller
//     {
//         private readonly UserManager<ApplicationUser> _userManager;
//         private readonly SignInManager<ApplicationUser> _signInManager;
//
//         public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
//         {
//             _userManager = userManager;
//             _signInManager = signInManager;
//         }
//
//         // GET: /Account/ChangePassword
//         [HttpGet]
//         public IActionResult ChangePassword()
//         {
//             // This action simply returns the view for changing the password.
//             return View();
//         }
//
//         // POST: /Account/ChangePassword
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
//         {
//             if (!ModelState.IsValid)
//             {
//                 // If the form data is not valid, return the view with the validation errors.
//                 return View(model);
//             }
//
//             // Get the currently logged-in user.
//             var user = await _userManager.GetUserAsync(User);
//             if (user == null)
//             {
//                 // This should not happen if the user is authorized, but it's a good safety check.
//                 return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
//             }
//
//             // Attempt to change the user's password.
//             var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
//
//             if (!changePasswordResult.Succeeded)
//             {
//                 // If the password change fails (e.g., wrong old password), add the errors to the model state.
//                 foreach (var error in changePasswordResult.Errors)
//                 {
//                     ModelState.AddModelError(string.Empty, error.Description);
//                 }
//                 return View(model);
//             }
//
//             // After a successful password change, sign the user in again to update the security stamp.
//             await _signInManager.RefreshSignInAsync(user);
//             
//             // Redirect to a confirmation page.
//             return RedirectToAction(nameof(ChangePasswordConfirmation));
//         }
//
//         // GET: /Account/ChangePasswordConfirmation
//         [HttpGet]
//         public IActionResult ChangePasswordConfirmation()
//         {
//             // This action shows a success message to the user.
//             return View();
//         }
//     }
// }







using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS.DataContext;
using SMS.Models;
using SMS.Models.ViewModels;


namespace SMS.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context; // Inject the DbContext

        // Updated constructor to accept AppDbContext
        public AccountController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            AppDbContext context) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context; // Initialize DbContext
        }

        // --- NEW ACTION FOR USER PROFILE ---
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var employee = await _context.Employees
                .Include(e => e.Location.State)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);

            var roles = await _userManager.GetRolesAsync(user);

            var vm = new UserProfileVM
            {
                FirstName = employee?.FirstName,
                LastName = employee?.LastName,
                Gender = employee?.Gender,
                Email = user.Email,
                MobileNumber = user.PhoneNumber,
                Location = (employee?.Location != null) 
                           ? $"{employee.Location.State.Name} ==> {employee.Location.Name}" 
                           : "N/A",
                Roles = string.Join(", ", roles)
            };

            return View(vm);
        }

        // GET: /Account/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["success"] = "Your password has been changed successfully.";
            return RedirectToAction(nameof(Profile));
        }
        
        // --- NEW ACTION FOR LOGOUT ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            // Redirect to your login page after logout
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
    }
}