using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    public class ExpenseVM
    {
        // For the "Add Expense" form
        public Expense NewExpense { get; set; } = new Expense();
        public IFormFile Attachment { get; set; } // To capture the uploaded file

        // For the "Expense List" table
        public IEnumerable<Expense> ExpenseList { get; set; } = new List<Expense>();

        // For the dropdowns
        public IEnumerable<SelectListItem> BranchList { get; set; }
        public IEnumerable<SelectListItem> PayViaList { get; set; }
    }
}