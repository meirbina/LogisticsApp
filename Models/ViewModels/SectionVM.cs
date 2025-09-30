using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SMS.Models.ViewModels
{
    public class SectionVM
    {
        public IEnumerable<SelectListItem> BranchList { get; set; }
        public Section Section { get; set; }
    }
}