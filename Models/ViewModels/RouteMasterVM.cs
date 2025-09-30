using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    public class RouteMasterVM
    {
        public RouteMaster RouteMaster { get; set; }
        public IEnumerable<RouteMaster> RouteMasterList { get; set; }
        public IEnumerable<SelectListItem> BranchList { get; set; }
    }
}
