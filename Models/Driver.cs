using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models;

public class Driver
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }
        
    public string PhoneNumber { get; set; }

    [Required]
    [ForeignKey("Company")]
    public int CompanyId { get; set; }

    public Company Company { get; set; }
}