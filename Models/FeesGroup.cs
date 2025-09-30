using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class FeesGroup
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Group Name is required.")]
        [Display(Name = "Group Name")]
        public string GroupName { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Please select a branch.")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        public ICollection<FeesGroupDetail> FeesGroupDetails { get; set; } = new List<FeesGroupDetail>();
    }
}