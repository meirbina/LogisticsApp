using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class GenericPackage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } // e.g., "Small Envelope", "Medium Box"

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal RegularPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Merchant Price (Discounted)")]
        public decimal MerchantPrice { get; set; }

        public bool IsActive { get; set; } = true;
    }
}