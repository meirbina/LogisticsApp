// using Microsoft.AspNetCore.Mvc.Rendering;
// using System.ComponentModel.DataAnnotations;
//
// namespace SMS.Models.ViewModels
// {
//     public class EmployeeVM
//     {
//         // --- For the Main Table ---
//         public IEnumerable<Employee> EmployeeList { get; set; }
//
//         // --- For the "Add Employee" Modal ---
//         public Employee NewEmployee { get; set; }
//
//         [Required]
//         [EmailAddress]
//         public string Email { get; set; }
//
//         [Required]
//         [Display(Name = "Phone Number")]
//         public string PhoneNumber { get; set; }
//
//         [Required(ErrorMessage = "Please select at least one role.")]
//         public List<string> SelectedRoles { get; set; }
//
//         // --- For Populating Dropdowns ---
//         public IEnumerable<SelectListItem> LocationList { get; set; }
//         public IEnumerable<SelectListItem> RoleList { get; set; }
//     }
// }





using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models.ViewModels
{
    // A new class to hold the formatted data for the main table list.
    public class EmployeeDetails
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string LocationName { get; set; }
        // This will hold the roles to be displayed as badges.
        public List<string> Roles { get; set; }
        // --- NEW PROPERTY ---
        // This will track if the user's account is locked.
        public bool IsLocked { get; set; }
    }

    // A new class to represent the data in the create/edit form tab.
    public class EmployeeAssignment
    {
        // This will be 0 for a new employee, or the ID when editing.
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public int LocationId { get; set; }
        [Required(ErrorMessage = "Please select at least one role.")]
        public List<string> SelectedRoles { get; set; }
    }

    // The main ViewModel for the entire page.
    public class EmployeeVM
    {
        // For the "Employee List" tab's table.
        public List<EmployeeDetails> EmployeeList { get; set; }

        // For the "Assign Employee" form tab.
        public EmployeeAssignment Assignment { get; set; }

        // For populating dropdown lists in the form.
        public IEnumerable<SelectListItem> LocationList { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}