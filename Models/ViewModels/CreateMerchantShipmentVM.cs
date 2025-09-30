using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models.ViewModels
{
    public class CreateMerchantShipmentVM
    {
        public string LoggedInUserEmail { get; set; }
        [Required(ErrorMessage = "Please select a merchant.")]
        [Display(Name = "Select Merchant")]
        public int MerchantId { get; set; }

        [Required] public string ReceiverName { get; set; }
        [Required] public string ReceiverPhoneNumber { get; set; }
        [Required] public string ReceiverAddress { get; set; }
        [Required] public int ReceiverStateId { get; set; }
        [Required] public int DestinationLocationId { get; set; }
        [Required] public string PaymentMethod { get; set; }
        public decimal DeclaredValue { get; set; }
        public int? InsuranceId { get; set; }
        public List<ShipmentItemVM> Items { get; set; } = new List<ShipmentItemVM>();
        
        public PriceDetailsVM PriceDetails { get; set; }
        public IEnumerable<SelectListItem> MerchantList { get; set; }
        public IEnumerable<SelectListItem> States { get; set; }
        public IEnumerable<SelectListItem> PackagingItems { get; set; }
        public IEnumerable<SelectListItem> InsuranceOptions { get; set; }
    }
}