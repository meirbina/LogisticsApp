using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models.ViewModels
{
    // This class remains the same
    public class ShipmentDisplayViewModel
    {
        public int Id { get; set; }
        public string ShipmentType { get; set; }
        public string WaybillNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public string Departure { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string Destination { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; }
    }

    // The main ViewModel is updated to include the date filters
    public class AllShipmentsVM
    {
        public List<ShipmentDisplayViewModel> Shipments { get; set; }

        // --- NEW PROPERTIES FOR DATE FILTERING ---
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}