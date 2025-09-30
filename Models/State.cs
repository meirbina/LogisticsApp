using System.ComponentModel.DataAnnotations;

namespace SMS.Models;

public class State
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "State Name")]
    public string Name { get; set; }

    [Display(Name = "Is Operational?")]
    public bool InState { get; set; } // Represents the 'In State?' column
}