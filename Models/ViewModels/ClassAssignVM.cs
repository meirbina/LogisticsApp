using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models.ViewModels
{
    // A helper class to represent the data collected from the form.
    // This is the equivalent of the 'EventType' class in your example.
    public class ClassAssignment
    {
        [Required(ErrorMessage = "Please select a branch.")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Please select a class.")]
        [Display(Name = "Class")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Please select a section.")]
        [Display(Name = "Section")]
        public int SectionId { get; set; }

        [Required(ErrorMessage = "Please select at least one subject.")]
        [Display(Name = "Subject")]
        public List<int> SubjectIds { get; set; } = new List<int>();
    }

    // This is the main ViewModel for the page, like 'EventTypeVM'.
    public class ClassAssignVM
    {
        // This holds the data for the create/edit form.
        public ClassAssignment Assignment { get; set; } = new();

        // This holds the data for the list view table.
        public IEnumerable<AssignListDetail> AssignList { get; set; }

        // This holds the initial data for the Branch dropdown.
        public IEnumerable<SelectListItem> BranchList { get; set; }
    }

    // A DTO to structure data cleanly for the list view
    public class AssignListDetail
    {
        public int SectionId { get; set; }
        public string BranchName { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public IEnumerable<string> AssignedSubjects { get; set; }
    }
}