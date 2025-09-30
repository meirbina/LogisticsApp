using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class HostelMaster
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a branch.")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        [Required(ErrorMessage = "Hostel Name is required.")]
        [Display(Name = "Hostel Name")]
        [StringLength(150)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int HostelCategoryId { get; set; }

        [ForeignKey("HostelCategoryId")]
        public virtual HostelCategory HostelCategory { get; set; }

        [Required(ErrorMessage = "Watchman Name is required.")]
        [Display(Name = "Watchman Name")]
        [StringLength(100)]
        public string WatchmanName { get; set; }

        [Display(Name = "Hostel Address")]
        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(500)]
        public string Remarks { get; set; }
    }
}