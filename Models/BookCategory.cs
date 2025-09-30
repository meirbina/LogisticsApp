using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class BookCategory
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category Name is required."), DisplayName("Book Category Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select a Branch.")]
        [Display(Name = "Branch")]
        [ForeignKey("Branch")]
        public int BranchId { get; set; }

        public Branch? Branch { get; set; }
    }
}