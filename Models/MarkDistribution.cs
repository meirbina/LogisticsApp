using System.Collections.Generic;
// ... other usings

namespace SMS.Models
{
    public class MarkDistribution
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        // Navigation property for the many-to-many relationship
        public ICollection<ExamMarkDistribution> ExamMarkDistributions { get; set; } = new List<ExamMarkDistribution>();
    }
}