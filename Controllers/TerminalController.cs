// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using SMS.DataContext;
// using SMS.Models.ViewModels;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using SMS.Models;
//
// namespace SMS.Controllers
// {
//     [Authorize]
//     public class TerminalController : Controller
//     {
//         private readonly AppDbContext _context;
//         private readonly UserManager<ApplicationUser> _userManager;
//
//         public TerminalController(AppDbContext context, UserManager<ApplicationUser> userManager)
//         {
//             _context = context;
//             _userManager = userManager;
//         }
//
//         public async Task<IActionResult> Shipments(DateTime? startDate, DateTime? endDate)
//         {
//             var user = await _userManager.GetUserAsync(User);
//             var employee = await _context.Employees.FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);
//             if (employee == null)
//             {
//                 TempData["error"] = "Your user account is not assigned to a terminal. Please contact an administrator.";
//                 return RedirectToAction("Index", "Home");
//             }
//             int userLocationId = employee.LocationId;
//
//             var start = startDate ?? DateTime.Today;
//             var end = endDate ?? DateTime.Today.AddDays(1).AddTicks(-1);
//
//             var allShipmentsInRange = new List<TerminalShipmentRecord>();
//
//             var employeeLocationMap = await _context.Employees
//                 .Include(e => e.ApplicationUser)
//                 .Include(e => e.Location.State)
//                 .Where(e => e.ApplicationUser != null)
//                 .ToDictionaryAsync(e => e.ApplicationUser.Email, e => new { LocationId = e.LocationId, FormattedName = $"{e.Location.State.Name} ==> {e.Location.Name}" });
//
//             var regularShipments = await _context.Shipments
//                 .Include(s => s.DestinationLocation.State)
//                 .Where(s => s.DateCreated >= start && s.DateCreated <= end)
//                 .Select(s => new TerminalShipmentRecord {
//                     WaybillNumber = s.WaybillNumber, DateCreated = s.DateCreated, Status = s.Status,
//                     SenderName = s.SenderName, ReceiverName = s.ReceiverName,
//                     DestinationLocation = $"{s.DestinationLocation.State.Name} ==> {s.DestinationLocation.Name}",
//                     TotalCost = s.TotalCost, CreatedBy = s.CreatedBy
//                 })
//                 .ToListAsync();
//
//             var merchantShipments = await _context.MerchantShipments
//                 .Include(s => s.Merchant)
//                 .Include(s => s.DestinationLocation.State)
//                 .Where(s => s.DateCreated >= start && s.DateCreated <= end)
//                 .Select(s => new TerminalShipmentRecord {
//                     WaybillNumber = s.WaybillNumber, DateCreated = s.DateCreated, Status = s.Status,
//                     SenderName = s.Merchant.BusinessName, ReceiverName = s.ReceiverName,
//                     DestinationLocation = $"{s.DestinationLocation.State.Name} ==> {s.DestinationLocation.Name}",
//                     TotalCost = s.TotalCost, CreatedBy = s.CreatedBy
//                 })
//                 .ToListAsync();
//             
//             allShipmentsInRange.AddRange(regularShipments);
//             allShipmentsInRange.AddRange(merchantShipments);
//
//             var incoming = new List<TerminalShipmentRecord>();
//             var outgoing = new List<TerminalShipmentRecord>();
//
//             var userEmailsInLocation = await _context.Employees
//                                                 .Where(e => e.LocationId == userLocationId)
//                                                 .Select(e => e.ApplicationUser.Email)
//                                                 .ToListAsync();
//
//             var destinationLocationFormattedName = (await _context.Locations.Include(l=>l.State).FirstOrDefaultAsync(l=>l.Id == userLocationId))
//                                                     ?.State.Name + " ==> " + (await _context.Locations.FirstOrDefaultAsync(l => l.Id == userLocationId))?.Name;
//
//             foreach (var record in allShipmentsInRange)
//             {
//                 var departureInfo = employeeLocationMap.GetValueOrDefault(record.CreatedBy);
//                 if (departureInfo != null)
//                 {
//                     record.DepartureLocation = departureInfo.FormattedName;
//                     if (departureInfo.LocationId == userLocationId)
//                     {
//                         outgoing.Add(record);
//                     }
//                 }
//                 else
//                 {
//                     record.DepartureLocation = "N/A";
//                 }
//                 
//                 if (record.DestinationLocation == destinationLocationFormattedName)
//                 {
//                     incoming.Add(record);
//                 }
//             }
//
//             var vm = new TerminalShipmentsVM
//             {
//                 IncomingShipments = incoming.OrderByDescending(s => s.DateCreated).ToList(),
//                 OutgoingShipments = outgoing.OrderByDescending(s => s.DateCreated).ToList(),
//                 StartDate = start,
//                 EndDate = end.Date
//             };
//
//             return View(vm);
//         }
//     }
// }





using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS.DataContext;
using SMS.Models;
using SMS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Controllers
{
    [Authorize]
    public class TerminalController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TerminalController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Shipments(DateTime? startDate, DateTime? endDate)
        {
            var user = await _userManager.GetUserAsync(User);
            var employee = await _context.Employees
                .Include(e => e.Location.State)
                .FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);

            if (employee == null)
            {
                TempData["error"] = "Your user account is not assigned to a terminal. Please contact an administrator.";
                return RedirectToAction("Index", "Home");
            }
            int userLocationId = employee.LocationId;
            string userLocationFormattedName = $"{employee.Location.State.Name} ==> {employee.Location.Name}";

            var start = startDate ?? DateTime.Today;
            var end = endDate ?? DateTime.Today.AddDays(1).AddTicks(-1);

            var allShipmentsInRange = new List<TerminalShipmentRecord>();

            var employeeLocationMap = await _context.Employees
                .Include(e => e.ApplicationUser)
                .Include(e => e.Location.State)
                .Where(e => e.ApplicationUser != null && e.Location != null)
                .ToDictionaryAsync(
                    e => e.ApplicationUser.Email, 
                    e => new { LocationId = e.LocationId, FormattedName = $"{e.Location.State.Name} ==> {e.Location.Name}" }
                );

            // Fetch and map Regular Shipments into our dedicated ViewModel
            var regularShipments = await _context.Shipments
                .Include(s => s.DestinationLocation.State)
                .Where(s => s.DateCreated >= start && s.DateCreated <= end)
                .Select(s => new TerminalShipmentRecord {
                    WaybillNumber = s.WaybillNumber,
                    DateCreated = s.DateCreated, // Correct property name
                    Status = s.Status,           // Correct property name
                    SenderName = s.SenderName,     // Correct property name
                    ReceiverName = s.ReceiverName,   // Correct property name
                    DestinationLocation = $"{s.DestinationLocation.State.Name} ==> {s.DestinationLocation.Name}",
                    TotalCost = s.TotalCost,
                    CreatedBy = s.CreatedBy
                })
                .ToListAsync();

            // Fetch and map Merchant Shipments into our dedicated ViewModel
            var merchantShipments = await _context.MerchantShipments
                .Include(s => s.Merchant)
                .Include(s => s.DestinationLocation.State)
                .Where(s => s.DateCreated >= start && s.DateCreated <= end)
                .Select(s => new TerminalShipmentRecord {
                    WaybillNumber = s.WaybillNumber,
                    DateCreated = s.DateCreated, // Correct property name
                    Status = s.Status,           // Correct property name
                    SenderName = s.Merchant.BusinessName, // Correct property name
                    ReceiverName = s.ReceiverName,   // Correct property name
                    DestinationLocation = $"{s.DestinationLocation.State.Name} ==> {s.DestinationLocation.Name}",
                    TotalCost = s.TotalCost,
                    CreatedBy = s.CreatedBy
                })
                .ToListAsync();
            
            allShipmentsInRange.AddRange(regularShipments);
            allShipmentsInRange.AddRange(merchantShipments);

            var incoming = new List<TerminalShipmentRecord>();
            var outgoing = new List<TerminalShipmentRecord>();

            foreach (var record in allShipmentsInRange)
            {
                var departureInfo = employeeLocationMap.GetValueOrDefault(record.CreatedBy);
                if (departureInfo != null)
                {
                    record.DepartureLocation = departureInfo.FormattedName;
                    if (departureInfo.LocationId == userLocationId)
                    {
                        outgoing.Add(record);
                    }
                }
                else
                {
                    record.DepartureLocation = "N/A";
                }
                
                if (record.DestinationLocation == userLocationFormattedName)
                {
                    incoming.Add(record);
                }
            }

            var vm = new TerminalShipmentsVM
            {
                IncomingShipments = incoming.OrderByDescending(s => s.DateCreated).ToList(),
                OutgoingShipments = outgoing.OrderByDescending(s => s.DateCreated).ToList(),
                StartDate = start,
                EndDate = end.Date
            };

            return View(vm);
        }
    }
}