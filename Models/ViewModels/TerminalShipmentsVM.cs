using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models.ViewModels
{
    // A dedicated helper class ONLY for the Terminal Shipments page.
    public class TerminalShipmentRecord
    {
        public string WaybillNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string DepartureLocation { get; set; }
        public string DestinationLocation { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; }

        // --- THE FIX: This property was missing and is required by the controller ---
        public string CreatedBy { get; set; } // The email of the user who created the shipment
    }

    // The main ViewModel for the Incoming/Outgoing Shipments page.
    public class TerminalShipmentsVM
    {
        public List<TerminalShipmentRecord> IncomingShipments { get; set; }
        public List<TerminalShipmentRecord> OutgoingShipments { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}