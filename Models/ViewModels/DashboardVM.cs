using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    // A helper class to hold data for the bar chart (Sales per Terminal)
    public class ChartData
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<decimal> Data { get; set; } = new List<decimal>();
    }

    // The main ViewModel for the entire Dashboard page
    public class DashboardVM
    {
        // KPI Boxes
        public int TotalDailyShipments { get; set; }
        public int TotalRegisteredMerchants { get; set; }
        public int TotalActiveEmployees { get; set; }
        public int TotalDailyReleasedShipments { get; set; }
        public decimal TotalDailySales { get; set; }

        // Data for the "Total Shipments per Terminal" Bar Chart
        public ChartData ShipmentsPerTerminalChart { get; set; }

        // List for the "Recent Shipments" table
        public List<UniversalShipmentRecord> RecentShipments { get; set; }
    }
}