using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    public class VehicleMasterVM
    {
        public VehicleMaster VehicleMaster { get; set; }
        public IEnumerable<VehicleMaster> VehicleMasterList { get; set; }
        public IEnumerable<SelectListItem> BranchList { get; set; }
    }
}