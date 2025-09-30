using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class AssignVehicle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        [Required]
        [Display(Name = "Transport Route")]
        public int RouteMasterId { get; set; }

        [ForeignKey("RouteMasterId")]
        public virtual RouteMaster RouteMaster { get; set; }

        [Required]
        [Display(Name = "Stoppage")]
        public int StoppageId { get; set; }

        [ForeignKey("StoppageId")]
        public virtual Stoppage Stoppage { get; set; }

        [Required]
        [Display(Name = "Vehicle")]
        public string VehicleIds { get; set; } // Comma-separated list of vehicle IDs
    }
}