using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models;

public class EventType
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Please select a branch.")]
    [Display(Name = "Branch")]
    public int BranchId { get; set; }
    [ForeignKey("BranchId")]
    public virtual Branch Branch { get; set; }

    [Required(ErrorMessage = "Please select an event.")]
    [Display(Name = "Type")]
    public int EventId { get; set; }
    [ForeignKey("EventId")]
    public virtual Event Event { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    // [Required]
    // [Display(Name = "Start Date")]
    // public DateTime StartDate { get; set; }
    //
    // [Required]
    // [Display(Name = "End Date")]
    // public DateTime EndDate { get; set; }
    
    [Required]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; }

    [Required]
    [Display(Name = "End Date")]
    public DateTime EndDate { get; set; }


    public string Description { get; set; }

    [Required(ErrorMessage = "Please select an audience.")]
    public string Audience { get; set; }

    [Display(Name = "Class")]
    public int? ControlClassId { get; set; }
    [ForeignKey("ControlClassId")]
    public virtual ControlClass ControlClass { get; set; }

    [Display(Name = "Section")]
    public int? SectionId { get; set; }
    [ForeignKey("SectionId")]
    public virtual Section Section { get; set; }
}