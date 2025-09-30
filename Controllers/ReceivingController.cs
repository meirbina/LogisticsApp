using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS.DataContext;
using SMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Controllers
{
    [Authorize]
    public class ReceivingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReceivingController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Main page to view incoming manifests
        public async Task<IActionResult> IncomingManifests()
        {
            var user = await _userManager.GetUserAsync(User);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);
            
            // Show manifests that are dispatched AND destined for the current user's location
            var manifests = await _context.Manifests
                .Where(m => m.IsSignedOff && m.DestinationLocationId == employee.LocationId)
                .OrderByDescending(m => m.DispatchedDate)
                .ToListAsync();

            return View(manifests);
        }

        // API to get shipments for a specific manifest
        [HttpGet]
        public async Task<IActionResult> GetShipmentsForManifest(int manifestDbId)
        {
            var waybills = await _context.ManifestShipments
                .Where(ms => ms.ManifestId == manifestDbId)
                .Select(ms => ms.WaybillNumber)
                .ToListAsync();

            var regular = await _context.Shipments
                .Where(s => waybills.Contains(s.WaybillNumber))
                .Select(s => new { s.WaybillNumber, s.Items.FirstOrDefault().Description, s.Status })
                .ToListAsync();
            
            var merchant = await _context.MerchantShipments
                .Where(s => waybills.Contains(s.WaybillNumber))
                .Select(s => new { s.WaybillNumber, s.Items.FirstOrDefault().Description, s.Status })
                .ToListAsync();

            return Json(regular.Concat(merchant));
        }

        // Action to process the receiving of selected shipments
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
            var receivedDate = System.DateTime.Now;

            // Update Regular Shipments
            var regularToUpdate = await _context.Shipments
                .Where(s => selectedWaybills.Contains(s.WaybillNumber))
                .ToListAsync();
            regularToUpdate.ForEach(s => { s.Status = "Arrived"; s.ReceivedBy = user.Email; s.ReceivedDate = receivedDate; });

            // Update Merchant Shipments
            var merchantToUpdate = await _context.MerchantShipments
                .Where(s => selectedWaybills.Contains(s.WaybillNumber))
                .ToListAsync();
            merchantToUpdate.ForEach(s => { s.Status = "Arrived"; s.ReceivedBy = user.Email; s.ReceivedDate = receivedDate; });

            await _context.SaveChangesAsync();
            TempData["success"] = $"{selectedWaybills.Count} shipment(s) have been successfully marked as 'Arrived'.";
            return RedirectToAction("IncomingManifests");
        }
    }
}