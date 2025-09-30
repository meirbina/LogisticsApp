using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models.ViewModels
{
    public class StudentListVM
    {
        // --- Search Filter Properties ---
        [Required]
        [Display(Name = "Branch")]
        public int? SearchBranchId { get; set; }

        [Required]
        [Display(Name = "Class")]
        public int? SearchClassId { get; set; }

        [Display(Name = "Section")]
        public int? SearchSectionId { get; set; } // Can be null for "All Sections"
        
        // --- Lists for Dropdowns ---
        public IEnumerable<SelectListItem> BranchList { get; set; }

        // --- Result List ---
        // This will be populated after the user clicks the "Filter" button.
        public IEnumerable<Student> StudentList { get; set; } = new List<Student>();
    }
}