using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    public class DepositVM
    {
        // For the "Add Deposit" form
        public Deposit NewDeposit { get; set; } = new Deposit();
        public IFormFile Attachment { get; set; } // To capture the uploaded file

        // For the "Deposit List" table
        public IEnumerable<Deposit> DepositList { get; set; } = new List<Deposit>();

        // For the dropdowns
        public IEnumerable<SelectListItem> BranchList { get; set; }
        public IEnumerable<SelectListItem> PayViaList { get; set; }
    }
}