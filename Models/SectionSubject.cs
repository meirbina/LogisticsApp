using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models;

public class SectionSubject
{
    public int SectionId { get; set; }
    public int SubjectId { get; set; }
    [ForeignKey("SectionId")]
    public virtual Section Section { get; set; }

    [ForeignKey("SubjectId")]
    public virtual Subject Subject { get; set; }
}