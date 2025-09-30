using Microsoft.AspNetCore.Mvc.Rendering;

namespace SMS.Models.ViewModels;

public class DesignationVM
{
    public Designation Designation { get; set; }
    public List<Designation> DesignationList { get; set; }

    public List<SelectListItem> BranchList { get; set; }
}