using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models;

public class HostelCategory
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Please select a branch.")]
    [Display(Name = "Branch")]
    public int BranchId { get; set; }

    [ForeignKey("BranchId")]
    public virtual Branch Branch { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100)]
    public string Name { get; set; }

    [Required(ErrorMessage = "Please select a category type.")]
    [Display(Name = "Category For")]
    public string Type { get; set; } // This will store "Hostel" or "Room"

    [StringLength(500)]
    public string Remarks { get; set; }
}
