using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Models
{
    public class Branch
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100), Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        [Required]
        [StringLength(100), Display(Name = "School Name")]
        public string SchoolName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(11, ErrorMessage = "Mobile number must be exactly 11 digits.")]
        [RegularExpression(@"^0\d{10}$", ErrorMessage = "Invalid mobile number. Must be 11 digits starting with 0.")]
        [Display(Name = "Mobile No")]
        public string MobileNo { get; set; }

        [Required]
        [StringLength(100)]
        public string Currency { get; set; }

        [Required]
        [StringLength(5), Display(Name = "Currency Symbol")]
        public string CurrencySymbol { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(100)]
        public string State { get; set; }

        [Required]
        [StringLength(450)]
        public string Address { get; set; }
    }
}