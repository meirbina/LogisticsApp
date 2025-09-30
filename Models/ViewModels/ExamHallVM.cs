using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    public class ExamHallVM
    {
        // For the "Add Exam Hall" form
        public ExamHall NewExamHall { get; set; } = new ExamHall();

        // For the "Exam Hall List" table
        public IEnumerable<ExamHall> ExamHallList { get; set; } = new List<ExamHall>();

        // For the Branch dropdown
        public IEnumerable<SelectListItem> BranchList { get; set; }
    }
}