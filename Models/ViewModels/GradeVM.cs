using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    public class GradeVM
    {
        // For the "Create Grade" form tab
        public Grade NewGrade { get; set; } = new Grade();

        // For the "Grade List" tab
        public IEnumerable<Grade> GradeList { get; set; } = new List<Grade>();

        // For the Branch dropdown list
        public IEnumerable<SelectListItem> BranchList { get; set; }
    }
}