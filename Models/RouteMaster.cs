using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class RouteMaster
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a branch.")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        [Required(ErrorMessage = "Route Name is required.")]
        [Display(Name = "Route Name")]
        [StringLength(150)]
        public string RouteName { get; set; }

        [Required(ErrorMessage = "Start Place is required.")]
        [Display(Name = "Start Place")]
        [StringLength(150)]
        public string StartPlace { get; set; }

        [Required(ErrorMessage = "Stop Place is required.")]
        [Display(Name = "Stop Place")]
        [StringLength(150)]
        public string StopPlace { get; set; }

        [StringLength(500)]
        public string Remarks { get; set; }
    }
}