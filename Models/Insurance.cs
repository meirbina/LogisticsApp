using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models;

public class Insurance
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Insurance Name")]
    public string Name { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Rate (as a percentage, e.g., 5 for 5%)")]
    public decimal Amount { get; set; }
}