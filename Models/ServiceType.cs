using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models;

public class ServiceType
{
     [Key]
            public int Id { get; set; }
    
            [Required(ErrorMessage = "Platform Type name is required.")]
            [Display(Name = "Platform Type")]
            public string Name { get; set; }
    
            [Display(Name = "Date Created")]
            public DateTime CreatedDate { get; set; }
    
            [Display(Name = "Is Deleted")]
            [DefaultValue(false)]
            public bool IsDeleted { get; set; } = false;
}