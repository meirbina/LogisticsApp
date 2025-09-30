using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Plate Number")]
        public string PlateNumber { get; set; }

        public bool IsActive { get; set; } = true;
    }
}