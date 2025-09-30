using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models.ViewModels
{
    // A helper class to standardize data from both Shipment and MerchantShipment tables.
    // This is crucial for all report calculations.
    public class UniversalShipmentRecord
    {
        public DateTime Date { get; set; }
        public string CreatedBy { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Vat { get; set; }
        public decimal PackagingCost { get; set; }
        public string PaymentMethod { get; set; }
        public string WaybillNumber { get; set; }
        public decimal TotalWeight { get; set; }
        public string SenderPhoneNumber { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public string ItemDescription { get; set; }
        
        
        
        public DateTime DateCreated { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string Status { get; set; }
    }

    // A helper class to hold the details for a specific shipment in the "View Details" modal.
    public class ShipmentSaleDetails
    {
        public string WaybillNumber { get; set; }
        public decimal TotalWeight { get; set; }
        public string SenderPhoneNumber { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public string ItemDescription { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalCost { get; set; }
    }

    // A helper class to hold aggregated sales for a single user within a location.
    public class UserSaleDetails
    {
        public string UserEmail { get; set; }
        public decimal TotalSalesByUser { get; set; }
        public List<ShipmentSaleDetails> Shipments { get; set; } = new List<ShipmentSaleDetails>();
    }

    // A helper class for the main admin report table, showing a daily breakdown.
    public class DailyLocationSale
    {
        public string LocationName { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalVat { get; set; }
        public decimal TotalPackagingFee { get; set; }
        public List<UserSaleDetails> UserSales { get; set; } = new List<UserSaleDetails>();
    }

    // A helper class for the summary panel on the right of both report pages.
    public class OverallSalesSummary
    {
        public decimal GrandTotalSales { get; set; }
        public decimal TotalPos { get; set; }
        public decimal TotalCash { get; set; }
        public decimal TotalTransfer { get; set; }
    }

    // The main ViewModel for the ADMIN Sales Report page.
    public class SalesReportVM
    {
        public List<DailyLocationSale> DailySales { get; set; }
        public OverallSalesSummary Summary { get; set; }
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }

    // The main ViewModel for the PERSONAL "My Sales Report" page.
    public class MySalesReportVM
    {
        public List<UniversalShipmentRecord> Shipments { get; set; }
        public OverallSalesSummary Summary { get; set; }
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}

