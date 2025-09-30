using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    public class AssignVehicleVM
    {
        public AssignVehicle AssignVehicle { get; set; }

        public IEnumerable<AssignVehicle> AssignVehicleList { get; set; }

        public IEnumerable<SelectListItem> BranchList { get; set; }
        public IEnumerable<SelectListItem> RouteList { get; set; }
        public IEnumerable<SelectListItem> StoppageList { get; set; }
        public IEnumerable<SelectListItem> VehicleList { get; set; }
    }
}