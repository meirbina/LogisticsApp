using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models;

public class PackagingPrice
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Packaging name is required.")]
    [Display(Name = "Packaging Description")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Amount is required.")]
    [Column(TypeName = "decimal(18,2)")] // Best practice for currency in SQL Server
    [Display(Name = "Price")]
    public decimal Amount { get; set; }

    [Display(Name = "Is Active")]
    [DefaultValue(true)]
    public bool IsActive { get; set; } = true;
}