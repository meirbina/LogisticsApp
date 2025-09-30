using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    public class HostelMasterVM
    {
        // For the "Create Hostel" form tab
        public HostelMaster HostelMaster { get; set; }

        // For the "Hostel List" tab
        public IEnumerable<HostelMaster> HostelMasterList { get; set; }

        // For the Branch dropdown
        public IEnumerable<SelectListItem> BranchList { get; set; }

        // For the dynamic Category dropdown
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}