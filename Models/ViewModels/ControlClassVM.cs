using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SMS.Models.ViewModels
{
  public class ControlClassVM
  {
    public ControlClass ControlClass { get; set; }
    public IEnumerable<SelectListItem> BranchList { get; set; }
    public List<SelectListItem> SectionList { get; set; }
    public List<int> SelectedSectionIds { get; set; } = new List<int>(); // To store selected section IDs
  }
  
}