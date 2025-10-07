using System.ComponentModel.DataAnnotations;

namespace SMS.Models.ViewModels
{
    public class TopUpWalletVM
    {
        public decimal CurrentBalance { get; set; }

        [Required]
        [Range(100, 1000000, ErrorMessage = "Amount must be between 100 and 1,000,000.")]
        [Display(Name = "Amount to Top Up")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please select a payment gateway.")]
        public string PaymentGateway { get; set; } // "Paystack", "Sterling", "Korapay"
    }
}