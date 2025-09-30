using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    public class FeesTypeVM
    {
        public FeesType NewFeesType { get; set; } = new FeesType();
        public IEnumerable<FeesType> FeesTypeList { get; set; } = new List<FeesType>();
        public IEnumerable<SelectListItem> BranchList { get; set; }
    }
}