using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    public class HostelRoomVM
    {
        public HostelRoom HostelRoom { get; set; }
        public IEnumerable<HostelRoom> HostelRoomList { get; set; }
        public IEnumerable<SelectListItem> BranchList { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}