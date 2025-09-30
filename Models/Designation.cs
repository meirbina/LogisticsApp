using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models;

public class Designation
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [ForeignKey("Branch")]
    public int BranchId { get; set; }

    public Branch Branch { get; set; }
}