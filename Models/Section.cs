using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Models
{
    public class Section
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        public int Capacity { get; set; }

        [Required]
        [ForeignKey("Branch")]
        public int BranchId { get; set; }

        public Branch Branch { get; set; }
    }
}