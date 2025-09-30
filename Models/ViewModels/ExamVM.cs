using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using SMS.Models;

namespace SMS.Models.ViewModels
{
    public class ExamVM
    {
        public IEnumerable<Exam> ExamList { get; set; } = new List<Exam>();
        public Exam NewExam { get; set; } = new Exam();
        
        // Captures IDs from the "Add" form's multi-select
        public int[] SelectedDistributionIds { get; set; }

        // For static dropdowns
        public IEnumerable<SelectListItem> BranchList { get; set; }
        public IEnumerable<SelectListItem> ExamTypeList { get; set; }
    }
}