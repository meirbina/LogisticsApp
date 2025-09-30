using SMS.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models.ViewModels
{
    public class MerchantVM
    {
        // For displaying the list on the Index page
        public IEnumerable<Merchant> MerchantList { get; set; }

        // --- A dedicated object for the single Create/Edit form ---
        public MerchantFormModel Form { get; set; } = new MerchantFormModel();
    }

    // This class will hold all the properties for the form
    public class MerchantFormModel
    {
        public int Id { get; set; } // 0 for Create, > 0 for Edit
        [Required] public string BusinessName { get; set; }
        [Required] public string OwnerFirstName { get; set; }
        [Required] public string OwnerLastName { get; set; }
        [Required, EmailAddress] public string BusinessEmail { get; set; }
        [Required] public string BusinessPhoneNumber { get; set; }
        [Required] public string BusinessAddress { get; set; }
        public List<WeightPrice> WeightPrices { get; set; } = new List<WeightPrice>();
    }
}