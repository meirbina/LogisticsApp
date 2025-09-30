using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class Exam
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Term")]
        public int ExamTermId { get; set; }
        public ExamTerm ExamTerm { get; set; }

        [Required]
        [Display(Name = "Exam Type")]
        public string ExamType { get; set; }
        
        public string Remarks { get; set; }

        // Many-to-many relationship through the join table
        public ICollection<ExamMarkDistribution> ExamMarkDistributions { get; set; } = new List<ExamMarkDistribution>();
    }
}