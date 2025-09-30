using Microsoft.AspNetCore.Mvc.Rendering;

namespace SMS.Models.ViewModels;

public class CategoryVM
{
    public IEnumerable<SelectListItem> BranchList { get; set; }
    public Category Category { get; set; }
}