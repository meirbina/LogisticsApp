using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Subject Name is required.")]
        [Display(Name = "Subject Name")]
        public string SubjectName { get; set; }

        [Required(ErrorMessage = "Subject Code is required.")]
        [Display(Name = "Subject Code")]
        public string SubjectCode { get; set; }

        [Display(Name = "Subject Author")]
        public string? SubjectAuthor { get; set; } // Nullable as it's not marked as required

        [Required(ErrorMessage = "Subject Type is required.")]
        [Display(Name = "Subject Type")]
        public string SubjectType { get; set; }

        // Foreign Key for Branch
        [Required(ErrorMessage = "Please select a Branch.")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }
    }
}