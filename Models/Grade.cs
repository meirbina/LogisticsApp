using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class Grade
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a branch.")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Grade Point is required.")]
        [Column(TypeName = "decimal(5, 2)")] // e.g., Allows for values like 4.00, 3.50
        public decimal GradePoint { get; set; }

        [Required(ErrorMessage = "Min Percentage is required.")]
        [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100.")]
        public int MinPercentage { get; set; }

        [Required(ErrorMessage = "Max Percentage is required.")]
        [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100.")]
        public int MaxPercentage { get; set; }

        public string Remarks { get; set; }
    }
}