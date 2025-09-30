using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    public class SubjectVM
    {
        public Subject Subject { get; set; }
        public IEnumerable<Subject> SubjectList { get; set; }
        public IEnumerable<SelectListItem> BranchList { get; set; }
    }
}