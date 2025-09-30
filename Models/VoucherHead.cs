using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class VoucherHead
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a branch.")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        public string Type { get; set; } // "Income" or "Expense"
    }
}