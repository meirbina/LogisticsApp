using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class Stoppage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Stop Time")]
        public TimeSpan StopTime { get; set; }

        [Required]
        [Display(Name = "Route Fare")]
        [Range(0, 100000)]
        public decimal Fare { get; set; }
    }
}
