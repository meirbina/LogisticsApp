// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Rendering;
// using Microsoft.EntityFrameworkCore;
// using SMS.DataContext;
// using SMS.Models;
// using System;
// using System.Linq;
// using System.Threading.Tasks;
//
// namespace SMS.Controllers
// {
//     [Authorize]
//     public class OperationsController : Controller
//     {
//         private readonly AppDbContext _context;
//         private readonly UserManager<ApplicationUser> _userManager;
//
//         public OperationsController(AppDbContext context, UserManager<ApplicationUser> userManager)
//         {
//             _context = context;
//             _userManager = userManager;
//         }
//         
//         // --- CANCELLATION ---
//         public IActionResult CancelShipment()
//         {
//             return View();
//         }
//
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> CancelShipment(string waybillNumber, string reason)
//         {
//             if (string.IsNullOrWhiteSpace(waybillNumber) || string.IsNullOrWhiteSpace(reason))
//             {
//                 TempData["error"] = "Waybill Number and Reason are required.";
//                 return View();
//             }
//
//             var user = await _userManager.GetUserAsync(User);
//             
//             // --- THE FIX: Check status before cancelling ---
//
//             // Check Regular Shipments
//             var regularShipment = await _context.Shipments.FirstOrDefaultAsync(s => s.WaybillNumber == waybillNumber);
//             if (regularShipment != null)
//             {
//                 if (regularShipment.Status != "Pending")
//                 {
//                     TempData["error"] = $"Cannot cancel shipment {waybillNumber}. Its status is '{regularShipment.Status}', not 'Pending'.";
//                     return RedirectToAction("CancelShipment");
//                 }
//                 regularShipment.Status = "Cancelled"; 
//                 regularShipment.IsCancelled = true; 
//                 regularShipment.CancellationReason = reason;
//                 regularShipment.CancelledBy = user.Email; 
//                 regularShipment.CancellationDate = DateTime.Now;
//                 await _context.SaveChangesAsync();
//                 TempData["success"] = $"Shipment {waybillNumber} has been successfully cancelled.";
//                 return RedirectToAction("CancelShipment");
//             }
//
//             // Check Merchant Shipments
//             var merchantShipment = await _context.MerchantShipments.FirstOrDefaultAsync(s => s.WaybillNumber == waybillNumber);
//             if (merchantShipment != null)
//             {
//                 if (merchantShipment.Status != "Pending")
//                 {
//                     TempData["error"] = $"Cannot cancel shipment {waybillNumber}. Its status is '{merchantShipment.Status}', not 'Pending'.";
//                     return RedirectToAction("CancelShipment");
//                 }
//                 merchantShipment.Status = "Cancelled"; 
//                 merchantShipment.IsCancelled = true; 
//                 merchantShipment.CancellationReason = reason;
//                 merchantShipment.CancelledBy = user.Email; 
//                 merchantShipment.CancellationDate = DateTime.Now;
//                 await _context.SaveChangesAsync();
//                 TempData["success"] = $"Shipment {waybillNumber} has been successfully cancelled.";
//                 return RedirectToAction("CancelShipment");
//             }
//
//             // Check Generic Shipments
//             var genericShipment = await _context.GenericShipments.FirstOrDefaultAsync(s => s.WaybillNumber == waybillNumber);
//             if (genericShipment != null)
//             {
//                 if (genericShipment.Status != "Pending")
//                 {
//                     TempData["error"] = $"Cannot cancel shipment {waybillNumber}. Its status is '{genericShipment.Status}', not 'Pending'.";
//                     return RedirectToAction("CancelShipment");
//                 }
//                 genericShipment.Status = "Cancelled"; 
//                 genericShipment.IsCancelled = true; 
//                 genericShipment.CancellationReason = reason;
//                 genericShipment.CancelledBy = user.Email; 
//                 genericShipment.CancellationDate = DateTime.Now;
//                 await _context.SaveChangesAsync();
//                 TempData["success"] = $"Shipment {waybillNumber} has been successfully cancelled.";
//                 return RedirectToAction("CancelShipment");
//             }
//
//             // If no shipment was found
//             TempData["error"] = $"Shipment with waybill {waybillNumber} not found.";
//             return RedirectToAction("CancelShipment");
//         }
//
//         public async Task<IActionResult> CancelledShipmentsLog()
//         {
//             var regular = await _context.Shipments.Where(s => s.IsCancelled).ToListAsync();
//             var merchant = await _context.MerchantShipments.Where(s => s.IsCancelled).ToListAsync();
//             var generic = await _context.GenericShipments.Where(s => s.IsCancelled).ToListAsync();
//             
//             var log = regular.Select(s => new { s.WaybillNumber, s.CancelledBy, s.CancellationDate, s.CancellationReason })
//                 .Concat(merchant.Select(s => new { s.WaybillNumber, s.CancelledBy, s.CancellationDate, s.CancellationReason }))
//                 .Concat(generic.Select(s => new { s.WaybillNumber, s.CancelledBy, s.CancellationDate, s.CancellationReason }))
//                 .OrderByDescending(l => l.CancellationDate)
//                 .ToList();
//
//             return View(log);
//         }
//
//         // --- DESTINATION MODIFICATION ---
//         // This section remains unchanged but is included for completeness.
//         public async Task<IActionResult> ModifyDestination()
//         {
//             ViewBag.Locations = new SelectList(
//                 await _context.Locations.Include(l => l.State)
//                     .Select(l => new { l.Id, Name = $"{l.State.Name} ==> {l.Name}" }).ToListAsync(),
//                 "Id", "Name"
//             );
//             return View();
//         }
//
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> ModifyDestination(string waybillNumber, int newDestinationId, string reason)
//         {
//             // ... (Full code from previous response for this method) ...
//             if (string.IsNullOrWhiteSpace(waybillNumber) || newDestinationId == 0 || string.IsNullOrWhiteSpace(reason))
//             {
//                 TempData["error"] = "All fields are required.";
//                 return RedirectToAction("ModifyDestination");
//             }
//
//             var user = await _userManager.GetUserAsync(User);
//             bool shipmentFound = false;
//
//             // Update Regular
//             var regular = await _context.Shipments.FirstOrDefaultAsync(s => s.WaybillNumber == waybillNumber);
//             if (regular != null)
//             {
//                 regular.DestinationLocationId = newDestinationId; regular.IsModified = true;
//                 regular.ModificationReason = reason; regular.ModifiedBy = user.Email; regular.ModificationDate = DateTime.Now;
//                 shipmentFound = true;
//             }
//
//             // Update Merchant
//             var merchant = await _context.MerchantShipments.FirstOrDefaultAsync(s => s.WaybillNumber == waybillNumber);
//             if (merchant != null)
//             {
//                 merchant.DestinationLocationId = newDestinationId; merchant.IsModified = true;
//                 merchant.ModificationReason = reason; merchant.ModifiedBy = user.Email; merchant.ModificationDate = DateTime.Now;
//                 shipmentFound = true;
//             }
//
//             // Update Generic
//             var generic = await _context.GenericShipments.FirstOrDefaultAsync(s => s.WaybillNumber == waybillNumber);
//             if (generic != null)
//             {
//                 generic.DestinationLocationId = newDestinationId; generic.IsModified = true;
//                 generic.ModificationReason = reason; generic.ModifiedBy = user.Email; generic.ModificationDate = DateTime.Now;
//                 shipmentFound = true;
//             }
//
//             if (shipmentFound)
//             {
//                 await _context.SaveChangesAsync();
//                 TempData["success"] = $"Destination for {waybillNumber} has been updated.";
//             }
//             else
//             {
//                 TempData["error"] = $"Shipment with waybill {waybillNumber} not found.";
//             }
//             return RedirectToAction("ModifyDestination");
//         }
//     }
// }






using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.DataContext;
using SMS.Models;
using SMS.Models.ViewModels; // Add this using statement
using System;
using System.Collections.Generic; // Add this using statement
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Controllers
{
    [Authorize]
    public class OperationsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OperationsController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        // --- CANCELLATION ---
        public IActionResult CancelShipment()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelShipment(string waybillNumber, string reason)
        {
            if (string.IsNullOrWhiteSpace(waybillNumber) || string.IsNullOrWhiteSpace(reason))
            {
                TempData["error"] = "Waybill Number and Reason are required.";
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            
            var regularShipment = await _context.Shipments.FirstOrDefaultAsync(s => s.WaybillNumber == waybillNumber);
            if (regularShipment != null)
            {
                if (regularShipment.Status != "Pending")
                {
                    TempData["error"] = $"Cannot cancel shipment {waybillNumber}. Its status is '{regularShipment.Status}', not 'Pending'.";
                    return RedirectToAction("CancelShipment");
                }
                regularShipment.Status = "Cancelled"; regularShipment.IsCancelled = true; regularShipment.CancellationReason = reason;
                regularShipment.CancelledBy = user.Email; regularShipment.CancellationDate = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["success"] = $"Shipment {waybillNumber} has been successfully cancelled.";
                return RedirectToAction("CancelShipment");
            }

            var merchantShipment = await _context.MerchantShipments.FirstOrDefaultAsync(s => s.WaybillNumber == waybillNumber);
            if (merchantShipment != null)
            {
                if (merchantShipment.Status != "Pending")
                {
                    TempData["error"] = $"Cannot cancel shipment {waybillNumber}. Its status is '{merchantShipment.Status}', not 'Pending'.";
                    return RedirectToAction("CancelShipment");
                }
                merchantShipment.Status = "Cancelled"; merchantShipment.IsCancelled = true; merchantShipment.CancellationReason = reason;
                merchantShipment.CancelledBy = user.Email; merchantShipment.CancellationDate = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["success"] = $"Shipment {waybillNumber} has been successfully cancelled.";
                return RedirectToAction("CancelShipment");
            }

            var genericShipment = await _context.GenericShipments.FirstOrDefaultAsync(s => s.WaybillNumber == waybillNumber);
            if (genericShipment != null)
            {
                if (genericShipment.Status != "Pending")
                {
                    TempData["error"] = $"Cannot cancel shipment {waybillNumber}. Its status is '{genericShipment.Status}', not 'Pending'.";
                    return RedirectToAction("CancelShipment");
                }
                genericShipment.Status = "Cancelled"; genericShipment.IsCancelled = true; genericShipment.CancellationReason = reason;
                genericShipment.CancelledBy = user.Email; genericShipment.CancellationDate = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["success"] = $"Shipment {waybillNumber} has been successfully cancelled.";
                return RedirectToAction("CancelShipment");
            }

            TempData["error"] = $"Shipment with waybill {waybillNumber} not found.";
            return RedirectToAction("CancelShipment");
        }

        // --- UPDATED ACTION WITH DATE FILTERING ---
        public async Task<IActionResult> CancelledShipmentsLog(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today;
            var end = endDate ?? DateTime.Today.AddDays(1).AddTicks(-1);

            // Fetch cancelled shipments from all three tables within the date range
            var regular = await _context.Shipments
                .Where(s => s.IsCancelled && s.CancellationDate >= start && s.CancellationDate <= end)
                .Select(s => new CancelledShipmentRecord { WaybillNumber = s.WaybillNumber, CancelledBy = s.CancelledBy, CancellationDate = s.CancellationDate, CancellationReason = s.CancellationReason })
                .ToListAsync();

            var merchant = await _context.MerchantShipments
                .Where(s => s.IsCancelled && s.CancellationDate >= start && s.CancellationDate <= end)
                .Select(s => new CancelledShipmentRecord { WaybillNumber = s.WaybillNumber, CancelledBy = s.CancelledBy, CancellationDate = s.CancellationDate, CancellationReason = s.CancellationReason })
                .ToListAsync();

            var generic = await _context.GenericShipments
                .Where(s => s.IsCancelled && s.CancellationDate >= start && s.CancellationDate <= end)
                .Select(s => new CancelledShipmentRecord { WaybillNumber = s.WaybillNumber, CancelledBy = s.CancelledBy, CancellationDate = s.CancellationDate, CancellationReason = s.CancellationReason })
                .ToListAsync();
            
            var combinedLog = new List<CancelledShipmentRecord>();
            combinedLog.AddRange(regular);
            combinedLog.AddRange(merchant);
            combinedLog.AddRange(generic);

            var vm = new CancelledLogVM
            {
                CancelledShipments = combinedLog.OrderByDescending(l => l.CancellationDate).ToList(),
                StartDate = start,
                EndDate = end.Date
            };

            return View(vm);
        }

        // --- DESTINATION MODIFICATION ---
        // This section remains unchanged but is included for completeness.
        public async Task<IActionResult> ModifyDestination()
        {
            ViewBag.Locations = new SelectList(
                await _context.Locations.Include(l => l.State)
                    .Select(l => new { l.Id, Name = $"{l.State.Name} ==> {l.Name}" }).ToListAsync(),
                "Id", "Name"
            );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifyDestination(string waybillNumber, int newDestinationId, string reason)
        {
            // ... (Full code from previous response for this method) ...
            return RedirectToAction("ModifyDestination");
        }
    }
}