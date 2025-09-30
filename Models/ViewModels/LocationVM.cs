using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Models;
using System.Collections.Generic;

namespace SMS.Models.ViewModels // Or your appropriate namespace
{
    public class LocationVM
    {
        // For the "Create" form tab
        public Location Location { get; set; }

        // For the "List" tab table
        public IEnumerable<Location> LocationList { get; set; }

        // For the State dropdown
        public IEnumerable<SelectListItem> StateList { get; set; }

        // For the Location Type (Station/Hub) dropdown
        public IEnumerable<SelectListItem> LocationTypeList { get; set; }
    }
}