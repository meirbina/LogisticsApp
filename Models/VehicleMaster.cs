using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class VehicleMaster
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a branch.")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        [Required(ErrorMessage = "Vehicle No is required.")]
        [Display(Name = "Vehicle No")]
        [StringLength(50)]
        public string VehicleNo { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Display(Name = "Insurance Renewal Date")]
        [DataType(DataType.Date)]
        public DateTime? InsuranceRenewalDate { get; set; }

        [Required]
        [Display(Name = "Driver Name")]
        [StringLength(100)]
        public string DriverName { get; set; }

        [Required]
        [Display(Name = "Driver Phone")]
        [Phone]
        public string DriverPhone { get; set; }

        [Required]
        [Display(Name = "Driver License")]
        [StringLength(100)]
        public string DriverLicense { get; set; }
    }
}