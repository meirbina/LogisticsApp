using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SMS.Models.ViewModels
{
    public class EventVM
    {
        public IEnumerable<SelectListItem> BranchList { get; set; }
        public Event Event { get; set; }
    }
}