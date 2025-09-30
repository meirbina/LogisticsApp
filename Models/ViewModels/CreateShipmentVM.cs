using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models.ViewModels
{
    public class CreateShipmentVM
    {
        // For displaying the logged-in user's email
        public string LoggedInUserEmail { get; set; }

        // --- All Form Fields ---
        [Required] public string SenderName { get; set; }
        [Required] public string SenderPhoneNumber { get; set; }
        [Required] public string SenderAddress { get; set; }
        [Required] public string ReceiverName { get; set; }
        [Required] public string ReceiverPhoneNumber { get; set; }
        [Required] public string ReceiverAddress { get; set; }
        [Required] public int ReceiverStateId { get; set; }
        [Required] public int DestinationLocationId { get; set; }
        [Required] public string PaymentMethod { get; set; }
        public decimal DeclaredValue { get; set; }
        public int? InsuranceId { get; set; }
        public List<ShipmentItemVM> Items { get; set; } = new List<ShipmentItemVM>();
        
        // --- Data Holders ---
        public PriceDetailsVM PriceDetails { get; set; }
        public IEnumerable<SelectListItem> States { get; set; }
        public IEnumerable<SelectListItem> PackagingItems { get; set; }
        public IEnumerable<SelectListItem> InsuranceOptions { get; set; }
    }

    public class ShipmentItemVM 
    {
        [Required] public string Description { get; set; }
        [Required] public decimal Weight { get; set; }
        [Required] public string Condition { get; set; }
        public int PackagingPriceId { get; set; }
        public int NumberOfPackagingItems { get; set; }
    }

    public class PriceDetailsVM
    {
        public decimal ShipmentCost { get; set; }
        public decimal PackagingCost { get; set; }
        public decimal InsuranceCost { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Vat { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalToPay { get; set; }
    }
}