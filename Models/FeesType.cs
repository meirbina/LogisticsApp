using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class FeesType
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
    }
}