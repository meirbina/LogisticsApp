using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models.ViewModels
{
    // A helper class to hold the standardized data for a cancelled shipment
    public class CancelledShipmentRecord
    {
        public string WaybillNumber { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string CancelledBy { get; set; }
        public string CancellationReason { get; set; }
    }

    // The main ViewModel for the Cancelled Shipments Log page
    public class CancelledLogVM
    {
        public List<CancelledShipmentRecord> CancelledShipments { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}