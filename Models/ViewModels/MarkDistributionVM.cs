using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    public class MarkDistributionVM
    {
        // For the "Add Mark Distribution" form
        public MarkDistribution NewMarkDistribution { get; set; } = new MarkDistribution();

        // For the "Mark Distribution List" table
        public IEnumerable<MarkDistribution> MarkDistributionList { get; set; } = new List<MarkDistribution>();

        // For the Branch dropdown
        public IEnumerable<SelectListItem> BranchList { get; set; }
    }
}