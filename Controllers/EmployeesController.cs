// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Rendering;
// using Microsoft.EntityFrameworkCore;
// using SMS.DataContext;
// using SMS.Models;
// using SMS.Models.ViewModels;
//
// namespace SMS.Controllers
// {
//     [Authorize(Roles = "Admin")]
//     public class EmployeesController : Controller
//     {
//         private readonly AppDbContext _context;
//         private readonly UserManager<ApplicationUser> _userManager;
//         private readonly RoleManager<IdentityRole> _roleManager;
//
//         public EmployeesController(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
//         {
//             _context = context;
//             _userManager = userManager;
//             _roleManager = roleManager;
//         }
//
//         // GET: Main page for the list and assignment form
//         public async Task<IActionResult> Index()
//         {
//             var vm = new EmployeeVM
//             {
//                 EmployeeList = await GetEmployeeDetailsList(),
//                 Assignment = new EmployeeAssignment(), // For the create form
//                 LocationList = await GetLocationSelectList(),
//                 RoleList = await GetRoleSelectList()
//             };
//             return View(vm);
//         }
//         
//         // --- NEW ACTION TO HANDLE LOCK/UNLOCK ---
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> ToggleLockout(int id)
//         {
//             var employee = await _context.Employees.FindAsync(id);
//             if (employee == null) return NotFound();
//
//             var user = await _userManager.FindByIdAsync(employee.ApplicationUserId);
//             if (user == null) return NotFound();
//
//             if (await _userManager.IsLockedOutAsync(user))
//             {
//                 // If user is locked, unlock them by setting the date to null.
//                 await _userManager.SetLockoutEndDateAsync(user, null);
//                 TempData["success"] = "User unlocked successfully.";
//             }
//             else
//             {
//                 // If user is not locked, lock them by setting a date far in the future.
//                 await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
//                 TempData["success"] = "User locked successfully.";
//             }
//
//             return RedirectToAction(nameof(Index));
//         }
//
//
//         // POST: Handles both Creating and Updating an employee from the form tab
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> AssignEmployee(EmployeeVM vm)
//         {
//             if (!ModelState.IsValid)
//             {
//                 // If validation fails, repopulate the entire viewmodel and return to show errors
//                 TempData["error"] = "Please correct the validation errors.";
//                 vm.EmployeeList = await GetEmployeeDetailsList();
//                 vm.LocationList = await GetLocationSelectList();
//                 vm.RoleList = await GetRoleSelectList();
//                 return View("Index", vm);
//             }
//
//             // --- UPDATE LOGIC ---
//             if (vm.Assignment.Id > 0)
//             {
//                 var employeeInDb = await _context.Employees.FindAsync(vm.Assignment.Id);
//                 var user = await _userManager.FindByIdAsync(employeeInDb.ApplicationUserId);
//
//                 user.Email = vm.Assignment.Email;
//                 user.UserName = vm.Assignment.Email;
//                 user.PhoneNumber = vm.Assignment.PhoneNumber;
//                 await _userManager.UpdateAsync(user);
//
//                 var currentRoles = await _userManager.GetRolesAsync(user);
//                 await _userManager.RemoveFromRolesAsync(user, currentRoles);
//                 await _userManager.AddToRolesAsync(user, vm.Assignment.SelectedRoles);
//
//                 employeeInDb.FirstName = vm.Assignment.FirstName;
//                 employeeInDb.LastName = vm.Assignment.LastName;
//                 employeeInDb.Gender = vm.Assignment.Gender;
//                 employeeInDb.LocationId = vm.Assignment.LocationId;
//                 
//                 TempData["success"] = "Employee updated successfully!";
//             }
//             // --- CREATE LOGIC ---
//             else
//             {
//                 var user = new ApplicationUser { UserName = vm.Assignment.Email, Email = vm.Assignment.Email, PhoneNumber = vm.Assignment.PhoneNumber };
//                 var result = await _userManager.CreateAsync(user, "Password123!"); // Set a default password
//
//                 if (!result.Succeeded)
//                 {
//                      TempData["error"] = result.Errors.FirstOrDefault()?.Description ?? "Failed to create user login.";
//                      // To show the error on the form, we must repopulate and return the view
//                      vm.EmployeeList = await GetEmployeeDetailsList();
//                      vm.LocationList = await GetLocationSelectList();
//                      vm.RoleList = await GetRoleSelectList();
//                      ModelState.AddModelError("Assignment.Email", result.Errors.First().Description);
//                      return View("Index", vm);
//                 }
//
//                 await _userManager.AddToRolesAsync(user, vm.Assignment.SelectedRoles);
//
//                 var newEmployee = new Employee
//                 {
//                     FirstName = vm.Assignment.FirstName,
//                     LastName = vm.Assignment.LastName,
//                     Gender = vm.Assignment.Gender,
//                     LocationId = vm.Assignment.LocationId,
//                     ApplicationUserId = user.Id
//                 };
//                 _context.Employees.Add(newEmployee);
//                 TempData["success"] = "Employee created successfully!";
//             }
//
//             await _context.SaveChangesAsync();
//             return RedirectToAction(nameof(Index));
//         }
//
//         // POST: Delete
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Delete(int id)
//         {
//             var employee = await _context.Employees.FindAsync(id);
//             if (employee == null) return NotFound();
//
//             var user = await _userManager.FindByIdAsync(employee.ApplicationUserId);
//             if (user != null)
//             {
//                 await _userManager.DeleteAsync(user);
//             }
//             
//             _context.Employees.Remove(employee);
//             await _context.SaveChangesAsync();
//
//             TempData["success"] = "Employee deleted successfully.";
//             return RedirectToAction(nameof(Index));
//         }
//
//         #region API and Helper Methods
//         
//         // GET: API endpoint for the Edit button
//         [HttpGet]
//         public async Task<IActionResult> GetEmployeeForEdit(int id)
//         {
//             var employee = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
//             if (employee == null) return NotFound();
//
//             var user = await _userManager.FindByIdAsync(employee.ApplicationUserId);
//             if (user == null) return NotFound();
//
//             var roles = await _userManager.GetRolesAsync(user);
//
//             var data = new EmployeeAssignment
//             {
//                 Id = employee.Id,
//                 FirstName = employee.FirstName,
//                 LastName = employee.LastName,
//                 Gender = employee.Gender,
//                 Email = user.Email,
//                 PhoneNumber = user.PhoneNumber,
//                 LocationId = employee.LocationId,
//                 SelectedRoles = roles.ToList()
//             };
//             return Json(data);
//         }
//
//         // Helper to build the detailed list for the main table
//         // private async Task<List<EmployeeDetails>> GetEmployeeDetailsList()
//         // {
//         //     var employees = await _context.Employees.Include(e => e.Location.State).ToListAsync();
//         //     var employeeDetailsList = new List<EmployeeDetails>();
//         //
//         //     foreach (var emp in employees)
//         //     {
//         //         var user = await _userManager.FindByIdAsync(emp.ApplicationUserId);
//         //         if (user == null) continue;
//         //
//         //         employeeDetailsList.Add(new EmployeeDetails
//         //         {
//         //             Id = emp.Id,
//         //             FullName = $"{emp.FirstName} {emp.LastName}",
//         //             Gender = emp.Gender,
//         //             LocationName = $"{emp.Location?.State?.Name} ==> {emp.Location?.Name}",
//         //             Roles = (await _userManager.GetRolesAsync(user)).ToList()
//         //         });
//         //     }
//         //     return employeeDetailsList;
//         // }
//         //
//         
//         
//         private async Task<List<EmployeeDetails>> GetEmployeeDetailsList()
//         {
//             var employees = await _context.Employees.Include(e => e.Location.State).ToListAsync();
//             var employeeDetailsList = new List<EmployeeDetails>();
//
//             foreach (var emp in employees)
//             {
//                 var user = await _userManager.FindByIdAsync(emp.ApplicationUserId);
//                 if (user == null) continue;
//
//                 employeeDetailsList.Add(new EmployeeDetails
//                 {
//                     Id = emp.Id,
//                     FullName = $"{emp.FirstName} {emp.LastName}",
//                     Gender = emp.Gender,
//                     Email = user.Email,
//                     LocationName = $"{emp.Location?.State?.Name} ==> {emp.Location?.Name}",
//                     Roles = (await _userManager.GetRolesAsync(user)).ToList(),
//                     
//                     // --- MODIFICATION: Check and set the IsLocked status ---
//                     IsLocked = await _userManager.IsLockedOutAsync(user)
//                 });
//             }
//             return employeeDetailsList;
//         }
//
//         private async Task<IEnumerable<SelectListItem>> GetLocationSelectList()
//         {
//              var locations = await _context.Locations.Include(l => l.State).ToListAsync();
//              return locations.Select(l => new SelectListItem { Text = $"{l.State.Name} ==> {l.Name}", Value = l.Id.ToString() });
//         }
//
//         private async Task<IEnumerable<SelectListItem>> GetRoleSelectList()
//         {
//             return await _roleManager.Roles.Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToListAsync();
//         }
//
//         #endregion
//     }
// }


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.DataContext;
using SMS.Models;
using SMS.Models.ViewModels;
using SMS.Services;


namespace SMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ISmsService _smsService; // <-- INJECT THE SERVICE

        public EmployeesController(
            AppDbContext context, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            ISmsService smsService) // <-- ADD TO CONSTRUCTOR
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _smsService = smsService; // <-- INITIALIZE THE SERVICE
        }

        public async Task<IActionResult> Index()
        {
            var vm = new EmployeeVM
            {
                EmployeeList = await GetEmployeeDetailsList(),
                Assignment = new EmployeeAssignment(),
                LocationList = await GetLocationSelectList(),
                RoleList = await GetRoleSelectList()
            };
            return View(vm);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLockout(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();
            var user = await _userManager.FindByIdAsync(employee.ApplicationUserId);
            if (user == null) return NotFound();

            if (await _userManager.IsLockedOutAsync(user))
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["success"] = "User unlocked successfully.";
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                TempData["success"] = "User locked successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignEmployee(EmployeeVM vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Please correct the validation errors.";
                vm.EmployeeList = await GetEmployeeDetailsList();
                vm.LocationList = await GetLocationSelectList();
                vm.RoleList = await GetRoleSelectList();
                return View("Index", vm);
            }

            if (vm.Assignment.Id > 0) // --- UPDATE LOGIC ---
            {
                var employeeInDb = await _context.Employees.FindAsync(vm.Assignment.Id);
                var user = await _userManager.FindByIdAsync(employeeInDb.ApplicationUserId);

                user.Email = vm.Assignment.Email;
                user.UserName = vm.Assignment.Email;
                user.PhoneNumber = vm.Assignment.PhoneNumber;
                await _userManager.UpdateAsync(user);

                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRolesAsync(user, vm.Assignment.SelectedRoles);

                employeeInDb.FirstName = vm.Assignment.FirstName;
                employeeInDb.LastName = vm.Assignment.LastName;
                employeeInDb.Gender = vm.Assignment.Gender;
                employeeInDb.LocationId = vm.Assignment.LocationId;
                
                TempData["success"] = "Employee updated successfully!";
            }
            else // --- CREATE LOGIC ---
            {
                // --- THE FIX: Generate a random password ---
                var autoPassword = $"{Guid.NewGuid().ToString().Substring(0, 8)}A1!";

                var user = new ApplicationUser { UserName = vm.Assignment.Email, Email = vm.Assignment.Email, PhoneNumber = vm.Assignment.PhoneNumber };
                var result = await _userManager.CreateAsync(user, autoPassword);

                if (!result.Succeeded)
                {
                     TempData["error"] = result.Errors.FirstOrDefault()?.Description ?? "Failed to create user login.";
                     vm.EmployeeList = await GetEmployeeDetailsList();
                     vm.LocationList = await GetLocationSelectList();
                     vm.RoleList = await GetRoleSelectList();
                     ModelState.AddModelError("Assignment.Email", result.Errors.First().Description);
                     return View("Index", vm);
                }

                await _userManager.AddToRolesAsync(user, vm.Assignment.SelectedRoles);

                var newEmployee = new Employee
                {
                    FirstName = vm.Assignment.FirstName,
                    LastName = vm.Assignment.LastName,
                    Gender = vm.Assignment.Gender,
                    LocationId = vm.Assignment.LocationId,
                    ApplicationUserId = user.Id
                };
                _context.Employees.Add(newEmployee);
                
                // --- THE FIX: Send the welcome SMS ---
                if (!string.IsNullOrEmpty(vm.Assignment.PhoneNumber))
                {
                    // "Fire and forget" the task
                    _ = _smsService.SendEmployeeWelcomeSmsAsync(vm.Assignment.PhoneNumber, vm.Assignment.Email, autoPassword);
                }

                TempData["success"] = "Employee created successfully! Login details sent via SMS.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            var user = await _userManager.FindByIdAsync(employee.ApplicationUserId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            TempData["success"] = "Employee deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        #region API and Helper Methods
        [HttpGet]
        public async Task<IActionResult> GetEmployeeForEdit(int id)
        {
            var employee = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null) return NotFound();
            var user = await _userManager.FindByIdAsync(employee.ApplicationUserId);
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            var data = new EmployeeAssignment {
                Id = employee.Id, FirstName = employee.FirstName, LastName = employee.LastName,
                Gender = employee.Gender, Email = user.Email, PhoneNumber = user.PhoneNumber,
                LocationId = employee.LocationId, SelectedRoles = roles.ToList()
            };
            return Json(data);
        }
        
        private async Task<List<EmployeeDetails>> GetEmployeeDetailsList()
        {
            var employees = await _context.Employees.Include(e => e.Location.State).ToListAsync();
            var employeeDetailsList = new List<EmployeeDetails>();
            foreach (var emp in employees)
            {
                var user = await _userManager.FindByIdAsync(emp.ApplicationUserId);
                if (user == null) continue;
                employeeDetailsList.Add(new EmployeeDetails {
                    Id = emp.Id, FullName = $"{emp.FirstName} {emp.LastName}", Gender = emp.Gender,
                    Email = user.Email, LocationName = $"{emp.Location?.State?.Name} ==> {emp.Location?.Name}",
                    Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                    IsLocked = await _userManager.IsLockedOutAsync(user)
                });
            }
            return employeeDetailsList;
        }

        private async Task<IEnumerable<SelectListItem>> GetLocationSelectList()
        {
             var locations = await _context.Locations.Include(l => l.State).ToListAsync();
             return locations.Select(l => new SelectListItem { Text = $"{l.State.Name} ==> {l.Name}", Value = l.Id.ToString() });
        }

        private async Task<IEnumerable<SelectListItem>> GetRoleSelectList()
        {
            return await _roleManager.Roles.Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToListAsync();
        }
        #endregion
    }
}