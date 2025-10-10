// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using SMS.DataContext;
// using SMS.Models;
//
// namespace SMS.Controllers
// {
//     [Authorize]
//     public class ReceivingController : Controller
//     {
//         private readonly AppDbContext _context;
//         private readonly UserManager<ApplicationUser> _userManager;
//
//         public ReceivingController(AppDbContext context, UserManager<ApplicationUser> userManager)
//         {
//             _context = context;
//             _userManager = userManager;
//         }
//
//         // Main page to view incoming manifests (No changes needed here)
//         public async Task<IActionResult> IncomingManifests()
//         {
//             var user = await _userManager.GetUserAsync(User);
//             var employee = await _context.Employees.FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);
//             if (employee == null)
//             {
//                 TempData["error"] = "Your user account is not assigned to a terminal.";
//                 return RedirectToAction("Index", "Home");
//             }
//             
//             var manifests = await _context.Manifests
//                 .Include(m => m.DepartureLocation) // Include for display
//                 .Where(m => m.IsSignedOff && m.DestinationLocationId == employee.LocationId)
//                 .OrderByDescending(m => m.DispatchedDate)
//                 .ToListAsync();
//
//             return View(manifests);
//         }
//
//         // API to get shipments for a specific manifest (UPDATED)
//         [HttpGet]
//         public async Task<IActionResult> GetShipmentsForManifest(int manifestDbId)
//         {
//             var waybills = await _context.ManifestShipments
//                 .Where(ms => ms.ManifestId == manifestDbId)
//                 .Select(ms => ms.WaybillNumber)
//                 .ToListAsync();
//
//             var regular = await _context.Shipments
//                 .Where(s => waybills.Contains(s.WaybillNumber))
//                 .Select(s => new { s.WaybillNumber, Description = s.Items.FirstOrDefault().Description, s.Status })
//                 .ToListAsync();
//             
//             var merchant = await _context.MerchantShipments
//                 .Where(s => waybills.Contains(s.WaybillNumber))
//                 .Select(s => new { s.WaybillNumber, Description = s.Items.FirstOrDefault().Description, s.Status })
//                 .ToListAsync();
//             
//             // --- THE FIX: Query and add Generic Shipments ---
//             var generic = await _context.GenericShipments
//                 .Where(s => waybills.Contains(s.WaybillNumber))
//                 .Select(s => new { s.WaybillNumber, Description = "Generic Package(s)", s.Status })
//                 .ToListAsync();
//
//             // Combine all three lists
//             return Json(regular.Concat(merchant).Concat(generic));
//         }
//
//         // Action to process the receiving of selected shipments (UPDATED)
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> ReceiveSelected(List<string> selectedWaybills)
//         {
//             if (selectedWaybills == null || !selectedWaybills.Any())
//             {
//                 TempData["error"] = "No shipments were selected to receive.";
//                 return RedirectToAction("IncomingManifests");
//             }
//
//             var user = await _userManager.GetUserAsync(User);
//             var receivedDate = System.DateTime.Now;
//
//             // Update Regular Shipments
//             var regularToUpdate = await _context.Shipments
//                 .Where(s => selectedWaybills.Contains(s.WaybillNumber))
//                 .ToListAsync();
//             regularToUpdate.ForEach(s => { s.Status = "Arrived"; s.ReceivedBy = user.Email; s.ReceivedDate = receivedDate; });
//
//             // Update Merchant Shipments
//             var merchantToUpdate = await _context.MerchantShipments
//                 .Where(s => selectedWaybills.Contains(s.WaybillNumber))
//                 .ToListAsync();
//             merchantToUpdate.ForEach(s => { s.Status = "Arrived"; s.ReceivedBy = user.Email; s.ReceivedDate = receivedDate; });
//
//             // --- THE FIX: Update Generic Shipments ---
//             var genericToUpdate = await _context.GenericShipments
//                 .Where(s => selectedWaybills.Contains(s.WaybillNumber))
//                 .ToListAsync();
//             genericToUpdate.ForEach(s => { s.Status = "Arrived"; /* Assuming GenericShipment has ReceivedBy/Date */ });
//
//
//             await _context.SaveChangesAsync();
//             TempData["success"] = $"{selectedWaybills.Count} shipment(s) have been successfully marked as 'Arrived'.";
//             return RedirectToAction("IncomingManifests");
//         }
//     }
// }





using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS.DataContext;
using SMS.Models;
using SMS.Services;

namespace SMS.Controllers
{
    [Authorize]
    public class ReceivingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISmsService _smsService;

        public ReceivingController(AppDbContext context, UserManager<ApplicationUser> userManager, ISmsService smsService)
        {
            _context = context;
            _userManager = userManager;
            _smsService = smsService;
        }

        // Main page to view incoming manifests (unchanged)
        public async Task<IActionResult> IncomingManifests()
        {
            var user = await _userManager.GetUserAsync(User);
            var employee = await _context.Employees.Include(e => e.Location).FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);
            if (employee == null)
            {
                TempData["error"] = "Your user account is not assigned to a terminal.";
                return RedirectToAction("Index", "Home");
            }
            
            var manifests = await _context.Manifests
                .Include(m => m.DepartureLocation)
                .Where(m => m.IsSignedOff && m.DestinationLocationId == employee.LocationId)
                .OrderByDescending(m => m.DispatchedDate)
                .ToListAsync();

            return View(manifests);
        }

        // API to get shipments for a specific manifest (unchanged)
        [HttpGet]
        public async Task<IActionResult> GetShipmentsForManifest(int manifestDbId)
        {
            var waybills = await _context.ManifestShipments.Where(ms => ms.ManifestId == manifestDbId).Select(ms => ms.WaybillNumber).ToListAsync();
            var regular = await _context.Shipments.Where(s => waybills.Contains(s.WaybillNumber)).Select(s => new { s.WaybillNumber, Description = s.Items.FirstOrDefault().Description, s.Status }).ToListAsync();
            var merchant = await _context.MerchantShipments.Where(s => waybills.Contains(s.WaybillNumber)).Select(s => new { s.WaybillNumber, Description = s.Items.FirstOrDefault().Description, s.Status }).ToListAsync();
            var generic = await _context.GenericShipments.Where(s => waybills.Contains(s.WaybillNumber)).Select(s => new { s.WaybillNumber, Description = "Generic Package(s)", s.Status }).ToListAsync();
            return Json(regular.Concat(merchant).Concat(generic));
        }

        // Action to process the receiving of selected shipments (UPDATED)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceiveSelected(List<string> selectedWaybills)
        {
            if (selectedWaybills == null || !selectedWaybills.Any())
            {
                TempData["error"] = "No shipments were selected to receive.";
                return RedirectToAction("IncomingManifests");
            }

            var user = await _userManager.GetUserAsync(User);
            var employee = await _context.Employees.Include(e => e.Location).FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);
            if (employee?.Location == null)
            {
                TempData["error"] = "Could not identify your terminal location and address.";
                return RedirectToAction("IncomingManifests");
            }
            
            // --- THE FIX: Get the terminal name and address ---
            var terminalName = employee.Location.Name;
            var terminalAddress = employee.Location.LocationAddress; // Assuming Location model has LocationAddress property
            var receivedDate = DateTime.Now;
            
            var regularToUpdate = await _context.Shipments.Where(s => selectedWaybills.Contains(s.WaybillNumber) && s.Status != "Arrived").ToListAsync();
            regularToUpdate.ForEach(s => { s.Status = "Arrived"; s.ReceivedBy = user.Email; s.ReceivedDate = receivedDate; });

            var merchantToUpdate = await _context.MerchantShipments.Where(s => selectedWaybills.Contains(s.WaybillNumber) && s.Status != "Arrived").ToListAsync();
            merchantToUpdate.ForEach(s => { s.Status = "Arrived"; s.ReceivedBy = user.Email; s.ReceivedDate = receivedDate; });

            var genericToUpdate = await _context.GenericShipments.Where(s => selectedWaybills.Contains(s.WaybillNumber) && s.Status != "Arrived").ToListAsync();
            genericToUpdate.ForEach(s => { s.Status = "Arrived"; s.ReceivedBy = user.Email; s.ReceivedDate = receivedDate; });

            await _context.SaveChangesAsync();

            // --- THE FIX: Send SMS Notifications with new details ---

            // Send to Regular Senders/Receivers
            foreach(var s in regularToUpdate)
            {
                // Sender Notification
                _ = _smsService.SendArrivalSmsAsync(s.SenderPhoneNumber, s.WaybillNumber, terminalName, terminalAddress, isReceiver: false);
                // Receiver Notification
                _ = _smsService.SendArrivalSmsAsync(s.ReceiverPhoneNumber, s.WaybillNumber, terminalName, terminalAddress, isReceiver: true);
            }

            // Send to Merchant Senders/Receivers
            var merchantSenders = await _context.Merchants
                .Where(m => merchantToUpdate.Select(s => s.MerchantId).Contains(m.Id))
                .ToListAsync();
            foreach(var s in merchantToUpdate)
            {
                var senderPhone = merchantSenders.FirstOrDefault(m => m.Id == s.MerchantId)?.BusinessPhoneNumber;
                // Sender Notification
                _ = _smsService.SendArrivalSmsAsync(senderPhone, s.WaybillNumber, terminalName, terminalAddress, isReceiver: false);
                // Receiver Notification
                _ = _smsService.SendArrivalSmsAsync(s.ReceiverPhoneNumber, s.WaybillNumber, terminalName, terminalAddress, isReceiver: true);
            }

            // Send to Generic Senders/Receivers
            foreach(var s in genericToUpdate)
            {
                // Sender Notification
                _ = _smsService.SendArrivalSmsAsync(s.SenderPhoneNumber, s.WaybillNumber, terminalName, terminalAddress, isReceiver: false);
                // Receiver Notification
                _ = _smsService.SendArrivalSmsAsync(s.ReceiverPhoneNumber, s.WaybillNumber, terminalName, terminalAddress, isReceiver: true);
            }

            TempData["success"] = $"{selectedWaybills.Count} shipment(s) have been successfully marked as 'Arrived' and notifications sent.";
            return RedirectToAction("IncomingManifests");
        }
    }
}