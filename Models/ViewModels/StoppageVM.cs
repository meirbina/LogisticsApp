using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    public class StoppageVM
    {
        public Stoppage Stoppage { get; set; }
        public IEnumerable<Stoppage> StoppageList { get; set; }
        public IEnumerable<SelectListItem> BranchList { get; set; }
    }
}