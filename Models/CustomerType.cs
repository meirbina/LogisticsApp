using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace SMS.Models;

public class CustomerType
{
    [Key]
    public int Id { get; set; }   
    [Required(ErrorMessage = "Customer type name is required.")]
        [Display(Name = "Customer Type")]
        public string Name { get; set; }
    
        [Display(Name = "Is Active")]
        [DefaultValue(true)] // New customer types will be active by default
        public bool IsActive { get; set; } = true;
}