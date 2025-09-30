using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models.ViewModels
{
    // This is the logical data container for your form.
    public class TeacherAssignment
    {
        // This Id is used for editing an existing assignment.
        // It will be 0 for a new assignment.
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a branch.")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Please select a teacher.")]
        [Display(Name = "Teacher")]
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Please select a class.")]
        [Display(Name = "Class")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Please select a section.")]
        [Display(Name = "Section")]
        public int SectionId { get; set; }

        [Required(ErrorMessage = "Please select a subject.")]
        [Display(Name = "Subject")]
        public int SubjectId { get; set; }
    }

    // This is the main ViewModel for the page.
    public class TeacherAssignVM
    {
        public TeacherAssignment Assignment { get; set; } = new();
        public IEnumerable<TeacherAssignListDetail> AssignList { get; set; }
        public IEnumerable<SelectListItem> BranchList { get; set; }
    }

    // A DTO to structure data cleanly for the list view table.
    public class TeacherAssignListDetail
    {
        public int AssignmentId { get; set; }
        public string TeacherName { get; set; }
        public string BranchName { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string SubjectName { get; set; }
    }
}