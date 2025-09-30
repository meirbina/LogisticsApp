using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS.DataContext;
using SMS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // The action now accepts optional startDate and endDate parameters
        public async Task<IActionResult> AllShipments(DateTime? startDate, DateTime? endDate)
        {
            // --- NEW DATE FILTERING LOGIC ---
            // If no dates are provided, default to today.
            var start = startDate ?? DateTime.Today;
            // For the end date, we add one full day minus a tick to include everything on that day.
            var end = endDate ?? DateTime.Today.AddDays(1).AddTicks(-1);
            
            // To prevent queries over excessively large ranges, you might add a limit, e.g.,
            // if ((end - start).TotalDays > 90) { /* return error or adjust range */ }

            var allShipments = new List<ShipmentDisplayViewModel>();

            var employeeLocationMap = await _context.Employees
                .Include(e => e.ApplicationUser)
                .Include(e => e.Location.State)
                .Where(e => e.ApplicationUser != null && e.Location != null)
                .ToDictionaryAsync(e => e.ApplicationUser.Email, e => $"{e.Location.State.Name} ==> {e.Location.Name}");

            // Fetch and map Regular Shipments within the date range
            var regularShipments = await _context.Shipments
                .Include(s => s.DestinationLocation.State)
                .Where(s => s.DateCreated >= start && s.DateCreated <= end) // Apply date filter
                .ToListAsync();

            foreach (var s in regularShipments)
            {
                allShipments.Add(new ShipmentDisplayViewModel
                {
                    Id = s.Id, ShipmentType = "Regular", WaybillNumber = s.WaybillNumber,
                    DateCreated = s.DateCreated, Departure = employeeLocationMap.GetValueOrDefault(s.CreatedBy, "N/A"),
                    SenderName = s.SenderName, ReceiverName = s.ReceiverName,
                    Destination = $"{s.DestinationLocation.Name}, {s.DestinationLocation.State.Name}",
                    TotalCost = s.TotalCost, Status = s.Status
                });
            }
            
            // Fetch and map Merchant Shipments within the date range
            var merchantShipments = await _context.MerchantShipments
                .Include(s => s.Merchant)
                .Include(s => s.DestinationLocation.State)
                .Where(s => s.DateCreated >= start && s.DateCreated <= end) // Apply date filter
                .ToListAsync();

            foreach (var s in merchantShipments)
            {
                allShipments.Add(new ShipmentDisplayViewModel
                {
                    Id = s.Id, ShipmentType = "Merchant", WaybillNumber = s.WaybillNumber,
                    DateCreated = s.DateCreated, Departure = employeeLocationMap.GetValueOrDefault(s.CreatedBy, "N/A"),
                    SenderName = s.Merchant.BusinessName, ReceiverName = s.ReceiverName,
                    Destination = $"{s.DestinationLocation.Name}, {s.DestinationLocation.State.Name}",
                    TotalCost = s.TotalCost, Status = s.Status
                });
            }

            var vm = new AllShipmentsVM
            {
                Shipments = allShipments.OrderByDescending(s => s.DateCreated).ToList(),
                StartDate = start, // Pass the dates back to the view
                EndDate = end.Date // Pass just the date part for the input field
            };

            return View(vm);
        }
        
        public async Task<IActionResult> Index()
        {
            var vm = await GetDashboardData();
            return View(vm);
        }

        [HttpGet]
        public async Task<JsonResult> GetRealTimeStats()
        {
            var data = await GetDashboardData();
            return Json(new {
                totalDailyShipments = data.TotalDailyShipments,
                totalDailyReleasedShipments = data.TotalDailyReleasedShipments,
                totalDailySales = data.TotalDailySales,
                shipmentsPerTerminalChart = data.ShipmentsPerTerminalChart
            });
        }

        private async Task<DashboardVM> GetDashboardData()
        {
            var todayStart = DateTime.Today;
            var todayEnd = DateTime.Today.AddDays(1).AddTicks(-1);

            var dailyRegularCount = await _context.Shipments.CountAsync(s => s.DateCreated >= todayStart && s.DateCreated <= todayEnd);
            var dailyMerchantCount = await _context.MerchantShipments.CountAsync(s => s.DateCreated >= todayStart && s.DateCreated <= todayEnd);
            
            var totalDailySalesRegular = await _context.Shipments.Where(s => s.DateCreated >= todayStart && s.DateCreated <= todayEnd).SumAsync(s => (decimal?)s.TotalCost) ?? 0;
            var totalDailySalesMerchant = await _context.MerchantShipments.Where(s => s.DateCreated >= todayStart && s.DateCreated <= todayEnd).SumAsync(s => (decimal?)s.TotalCost) ?? 0;

            int totalDailyReleased = await _context.ShipmentCollections.CountAsync(c => c.CollectionDate >= todayStart && c.CollectionDate <= todayEnd);
            
            var vm = new DashboardVM
            {
                TotalDailyShipments = dailyRegularCount + dailyMerchantCount,
                TotalRegisteredMerchants = await _context.Merchants.CountAsync(),
                TotalActiveEmployees = await _context.Employees.CountAsync(),
                TotalDailyReleasedShipments = totalDailyReleased,
                TotalDailySales = totalDailySalesRegular + totalDailySalesMerchant
            };

            var employeeLocationMap = await _context.Employees
                .Include(e => e.ApplicationUser).Include(e => e.Location)
                .Where(e => e.ApplicationUser != null && e.Location != null)
                .ToDictionaryAsync(e => e.ApplicationUser.Email, e => e.Location.Name);
            
            var regularShipmentCreators = await _context.Shipments.Where(s => s.DateCreated >= todayStart && s.DateCreated <= todayEnd).Select(s => s.CreatedBy).ToListAsync();
            var merchantShipmentCreators = await _context.MerchantShipments.Where(s => s.DateCreated >= todayStart && s.DateCreated <= todayEnd).Select(s => s.CreatedBy).ToListAsync();
            
            var allCreators = regularShipmentCreators.Concat(merchantShipmentCreators);

            var shipmentsPerTerminal = allCreators
                .Select(creatorEmail => employeeLocationMap.GetValueOrDefault(creatorEmail, "Unassigned"))
                .GroupBy(locationName => locationName)
                .Select(g => new { Location = g.Key, Count = g.Count() })
                .OrderBy(x => x.Location)
                .ToList();

            vm.ShipmentsPerTerminalChart = new ChartData
            {
                Labels = shipmentsPerTerminal.Select(x => x.Location).ToList(),
                Data = shipmentsPerTerminal.Select(x => (decimal)x.Count).ToList()
            };
            
            // --- THE FIX: Use the complete UniversalShipmentRecord for the list ---
            var recentRegular = await _context.Shipments
                .OrderByDescending(s => s.DateCreated).Take(5)
                .Select(s => new UniversalShipmentRecord { WaybillNumber = s.WaybillNumber, DateCreated = s.DateCreated, SenderName = s.SenderName, ReceiverName = s.ReceiverName, Status = s.Status })
                .ToListAsync();

            var recentMerchant = await _context.MerchantShipments
                .Include(s => s.Merchant)
                .OrderByDescending(s => s.DateCreated).Take(5)
                .Select(s => new UniversalShipmentRecord { WaybillNumber = s.WaybillNumber, DateCreated = s.DateCreated, SenderName = s.Merchant.BusinessName, ReceiverName = s.ReceiverName, Status = s.Status })
                .ToListAsync();

            vm.RecentShipments = recentRegular.Concat(recentMerchant)
                                    .OrderByDescending(s => s.DateCreated)
                                    .Take(5).ToList();
            
            return vm;
        }
    }
}