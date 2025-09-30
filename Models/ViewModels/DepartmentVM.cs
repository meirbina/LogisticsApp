using Microsoft.AspNetCore.Mvc.Rendering;

namespace SMS.Models.ViewModels;

public class DepartmentVM
{
    public Department Department { get; set; }
    public List<Department> DepartmentList { get; set; }

    public List<SelectListItem> BranchList { get; set; }
}