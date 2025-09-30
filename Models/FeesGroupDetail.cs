using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class FeesGroupDetail
    {
        public int Id { get; set; }
        public int FeesGroupId { get; set; }
        public FeesGroup FeesGroup { get; set; }
        public int FeesTypeId { get; set; }
        public FeesType FeesType { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
    }
}