using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Gender { get; set; }

        // Foreign key to the Location table
        [Required(ErrorMessage = "Please assign a location.")]
        [Display(Name = "Assigned Location")]
        public int LocationId { get; set; }

        [ForeignKey("LocationId")]
        public Location Location { get; set; }
        
        // Foreign key to the ASP.NET Core Identity user table
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}