using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    public class FeesGroupVM
    {
        // For the "Add" form
        public FeesGroup NewFeesGroup { get; set; } = new FeesGroup();
        public List<FeesGroupDetail> NewDetails { get; set; } = new List<FeesGroupDetail>();

        // For the "Edit" modal
        public FeesGroup EditFeesGroup { get; set; } = new FeesGroup();
        public List<FeesGroupDetail> EditDetails { get; set; } = new List<FeesGroupDetail>();

        // For the list view
        public IEnumerable<FeesGroup> FeesGroupList { get; set; } = new List<FeesGroup>();
        
        // For dropdowns
        public IEnumerable<SelectListItem> BranchList { get; set; }
    }
}