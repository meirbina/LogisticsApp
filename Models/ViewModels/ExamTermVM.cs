using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    public class ExamTermVM
    {
        // For the "Add Exam Term" form on the left
        public ExamTerm NewExamTerm { get; set; } = new ExamTerm();

        // For the "Exam Term List" on the right
        public IEnumerable<ExamTerm> ExamTermList { get; set; } = new List<ExamTerm>();

        // For the Branch dropdown list
        public IEnumerable<SelectListItem> BranchList { get; set; }
    }
}