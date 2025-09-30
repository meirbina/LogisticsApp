using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models.ViewModels
{
    public class StudentAdmissionVM
    {
        // For the List View
        public IEnumerable<Student> StudentList { get; set; } = new List<Student>();

        // For the "Add Admission" Form
        public Student NewStudent { get; set; } = new Student();
        public Parent NewParent { get; set; } = new Parent();
        
        // CORRECTED: Separate IFormFile properties for student and parent
        public IFormFile StudentProfilePicture { get; set; }
        public IFormFile ParentProfilePicture { get; set; }

        [Required, EmailAddress]
        public string StudentEmail { get; set; }
        [Required, EmailAddress]
        public string ParentEmail { get; set; }
        
        // For All Dropdowns
        public IEnumerable<SelectListItem> AcademicYearList { get; set; }
        public IEnumerable<SelectListItem> BranchList { get; set; }
        public IEnumerable<SelectListItem> GenderList { get; set; }
        public IEnumerable<SelectListItem> BloodGroupList { get; set; }
    }
}