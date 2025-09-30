using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using SMS.Models;

namespace SMS.Models.ViewModels
{
    public class AccountingVM
    {
        // For the "Create Account" form tab
        public Accounting NewAccounting { get; set; } = new Accounting();

        // For the "Account List" tab
        public IEnumerable<Accounting> AccountingList { get; set; } = new List<Accounting>();

        // For the Branch dropdown list
        public IEnumerable<SelectListItem> BranchList { get; set; }
    }
}