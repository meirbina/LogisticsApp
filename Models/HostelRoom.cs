using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class HostelRoom
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a branch.")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        [Required(ErrorMessage = "Please select a room category.")]
        [Display(Name = "Category")]
        public int HostelCategoryId { get; set; }

        [ForeignKey("HostelCategoryId")]
        public virtual HostelCategory HostelCategory { get; set; }

        [Required(ErrorMessage = "Room Name is required.")]
        [Display(Name = "Room Name")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Number of beds is required.")]
        [Display(Name = "Number of Beds")]
        public int NumberOfBeds { get; set; }

        [Required(ErrorMessage = "Cost per bed is required.")]
        [Display(Name = "Cost Per Bed")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CostPerBed { get; set; }

        [StringLength(500)]
        public string Remarks { get; set; }
    }
}