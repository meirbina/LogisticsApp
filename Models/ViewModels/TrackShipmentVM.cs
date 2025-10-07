using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    // A universal class to hold display data for a searched shipment
    public class TrackedShipmentResult
    {
        public string WaybillNumber { get; set; }
        public string SenderPhoneNumber { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal AmountPaid { get; set; }
        public string Status { get; set; }
        public System.DateTime DateCreated { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class TrackShipmentVM
    {
        public string SearchTerm { get; set; }
        public List<TrackedShipmentResult> Results { get; set; }
    }
}