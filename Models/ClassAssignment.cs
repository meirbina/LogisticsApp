using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class ClassAssignment
    {
        public int Id { get; set; }

        // Academic Details
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        
        public string Role { get; set; } // Will store the role name like "Admin", "Teacher"
        
        public int ControlClassId { get; set; }
        public ControlClass ControlClass { get; set; }
        public int SectionId { get; set; }
        public Section Section { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}