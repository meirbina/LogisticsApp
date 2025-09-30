using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100), DisplayName("Category Name")]
    public string Name { get; set; }

    [Required]
    [ForeignKey("Branch")]
    public int BranchId { get; set; }

    public Branch Branch { get; set; }
}