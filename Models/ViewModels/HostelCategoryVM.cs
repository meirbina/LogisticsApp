using Microsoft.AspNetCore.Mvc.Rendering;

namespace SMS.Models.ViewModels;

public class HostelCategoryVM
{
    
        // For the "Add Category" form
        public HostelCategory HostelCategory { get; set; }

        // For the "Category List" table
        public IEnumerable<HostelCategory> HostelCategoryList { get; set; }

        // For the Branch dropdown
        public IEnumerable<SelectListItem> BranchList { get; set; }

        // For the "Category For" dropdown
        public IEnumerable<SelectListItem> TypeList { get; set; }
    
}