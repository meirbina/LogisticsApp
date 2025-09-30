using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class TeacherSubjectAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Teacher")]
        public int EmployeeId { get; set; } // Foreign Key to the Employee table

        [Required]
        [Display(Name = "Section")]
        public int SectionId { get; set; } // Foreign Key to the Section table

        [Required]
        [Display(Name = "Subject")]
        public int SubjectId { get; set; } // Foreign Key to the Subject table

        [ForeignKey("EmployeeId")]
        public virtual Employee Teacher { get; set; }

        [ForeignKey("SectionId")]
        public virtual Section Section { get; set; }

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }
    }
}