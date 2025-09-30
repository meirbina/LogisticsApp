using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class Merchant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string BusinessName { get; set; }

        [Required]
        public string OwnerFirstName { get; set; }

        [Required]
        public string OwnerLastName { get; set; }

        [Required]
        [EmailAddress]
        public string BusinessEmail { get; set; }

        [Required]
        public string BusinessPhoneNumber { get; set; }

        [Required]
        public string BusinessAddress { get; set; }

        // Navigation Properties
        public virtual Wallet Wallet { get; set; }
        public virtual ICollection<WeightPrice> WeightPrices { get; set; }
    }
}