using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models.ViewModels
{
    public class GenericShipmentItemVM
    {
        [Required]
        public int GenericPackageId { get; set; }
        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }
    }

    public class CreateGenericShipmentVM
    {
        public string LoggedInUserEmail { get; set; }
        
        [Required]
        public string ShipmentType { get; set; }
        
        public int? MerchantId { get; set; }
        public string SenderName { get; set; }
        public string SenderPhoneNumber { get; set; }
        public string SenderAddress { get; set; }

        [Required] public string ReceiverName { get; set; }
        [Required] public string ReceiverPhoneNumber { get; set; }
        [Required] public string ReceiverAddress { get; set; }
        [Required] public int DestinationLocationId { get; set; }
        
        [Required(ErrorMessage = "Please select a payment method.")]
        public string PaymentMethod { get; set; }
        
        public List<GenericShipmentItemVM> Items { get; set; } = new List<GenericShipmentItemVM>();
        
        public PriceDetailsVM PriceDetails { get; set; }
        
        public IEnumerable<SelectListItem> MerchantList { get; set; }
        public IEnumerable<SelectListItem> LocationList { get; set; }
        public IEnumerable<SelectListItem> GenericPackageList { get; set; }
    }
}