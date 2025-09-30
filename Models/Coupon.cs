using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public enum DiscountType
    {
        Percentage,
        FixedValue
    }

    public class Coupon
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public DiscountType DiscountType { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountValue { get; set; }
        public bool IsActive { get; set; } = true;
    }
}