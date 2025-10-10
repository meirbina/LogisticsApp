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
//     public class CollectionController : Controller
//     {
//         private readonly AppDbContext _context;
//         private readonly UserManager<ApplicationUser> _userManager;
//
//         public CollectionController(AppDbContext context, UserManager<ApplicationUser> userManager)
//         {
//             _context = context;
//             _userManager = userManager;
//         }
//
//         // Action to show shipments that have arrived but not yet collected
//         public async Task<IActionResult> ReleasedShipments()
//         {
//             // This ViewModel is a temporary, dynamic type perfect for this view.
//             var shipmentsForCollection = Enumerable.Empty<object>().Select(r => new { WaybillNumber = "", SenderName = "", SenderPhoneNumber = "", ReceiverName = "", ReceiverPhoneNumber = "", TotalWeight = 0.0m }).ToList();
//
//             // Fetch regular shipments that are 'Arrived' and not 'IsCollected'
//             var regularShipments = await _context.Shipments
//                 .Where(s => s.Status == "Arrived" && !s.IsCollected)
//                 .Select(s => new { 
//                     s.WaybillNumber, 
//                     s.SenderName, 
//                     s.SenderPhoneNumber, 
//                     s.ReceiverName, 
//                     s.ReceiverPhoneNumber, 
//                     TotalWeight = s.Items.Sum(i => i.Weight) 
//                 })
//                 .ToListAsync();
//
//             // Fetch merchant shipments that are 'Arrived' and not 'IsCollected'
//             var merchantShipments = await _context.MerchantShipments
//                 .Where(s => s.Status == "Arrived" && !s.IsCollected)
//                 .Select(s => new { 
//                     s.WaybillNumber, 
//                     SenderName = s.Merchant.BusinessName, 
//                     SenderPhoneNumber = s.Merchant.BusinessPhoneNumber, 
//                     s.ReceiverName, 
//                     s.ReceiverPhoneNumber, 
//                     TotalWeight = s.Items.Sum(i => i.Weight) 
//                 })
//                 .ToListAsync();
//
//             shipmentsForCollection.AddRange(regularShipments);
//             shipmentsForCollection.AddRange(merchantShipments);
//             
//             return View(shipmentsForCollection.OrderBy(s => s.WaybillNumber).ToList());
//         }
//
//         // Action to release a shipment to a receiver
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Release(ShipmentCollection collectionDetails)
//         {
//             if (!ModelState.IsValid)
//             {
//                 TempData["error"] = "Please fill all required fields.";
//                 return RedirectToAction("ReleasedShipments");
//             }
//             
//             var user = await _userManager.GetUserAsync(User);
//             collectionDetails.CollectionDate = DateTime.Now;
//             collectionDetails.ReleasedBy = user.Email;
//
//             _context.ShipmentCollections.Add(collectionDetails);
//             await _context.SaveChangesAsync(); // Save to get the new collection's ID
//
//             // Find and update the original shipment record(s) by linking the collection record
//             var regularShipment = await _context.Shipments.FirstOrDefaultAsync(s => s.WaybillNumber == collectionDetails.WaybillNumber);
//             if(regularShipment != null)
//             {
//                 regularShipment.IsCollected = true;
//                 regularShipment.ShipmentCollectionId = collectionDetails.Id;
//             }
//
//             var merchantShipment = await _context.MerchantShipments.FirstOrDefaultAsync(s => s.WaybillNumber == collectionDetails.WaybillNumber);
//             if(merchantShipment != null)
//             {
//                 merchantShipment.IsCollected = true;
//                 merchantShipment.ShipmentCollectionId = collectionDetails.Id;
//             }
//
//             await _context.SaveChangesAsync();
//             TempData["success"] = $"Shipment {collectionDetails.WaybillNumber} has been released successfully.";
//             return RedirectToAction("ReleasedShipments");
//         }
//
//         // Page to view all shipments that have been collected
//         public async Task<IActionResult> CollectedHistory()
//         {
//             var collections = await _context.ShipmentCollections
//                 .OrderByDescending(c => c.CollectionDate)
//                 .ToListAsync();
//             return View(collections);
//         }
//     }
// }



using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS.DataContext;
using SMS.Models;
using SMS.Services; // <-- ADD THIS USING STATEMENT
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Controllers
{
    [Authorize]
    public class CollectionController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISmsService _smsService; // <-- INJECT THE SERVICE

        public CollectionController(AppDbContext context, UserManager<ApplicationUser> userManager, ISmsService smsService)
        {
            _context = context;
            _userManager = userManager;
            _smsService = smsService; // <-- INITIALIZE THE SERVICE
        }

        public async Task<IActionResult> ReleasedShipments()
        {
            var shipmentsForCollection = Enumerable.Empty<object>().Select(r => new { WaybillNumber = "", SenderName = "", SenderPhoneNumber = "", ReceiverName = "", ReceiverPhoneNumber = "", TotalWeight = 0.0m }).ToList();

            var regularShipments = await _context.Shipments
                .Where(s => s.Status == "Arrived" && !s.IsCollected)
                .Select(s => new { s.WaybillNumber, s.SenderName, s.SenderPhoneNumber, s.ReceiverName, s.ReceiverPhoneNumber, TotalWeight = s.Items.Sum(i => i.Weight) })
                .ToListAsync();

            var merchantShipments = await _context.MerchantShipments.Include(s => s.Merchant)
                .Where(s => s.Status == "Arrived" && !s.IsCollected)
                .Select(s => new { s.WaybillNumber, SenderName = s.Merchant.BusinessName, SenderPhoneNumber = s.Merchant.BusinessPhoneNumber, s.ReceiverName, s.ReceiverPhoneNumber, TotalWeight = s.Items.Sum(i => i.Weight) })
                .ToListAsync();
            
            // --- THE FIX: Include Generic Shipments in the list ---
            var genericShipments = await _context.GenericShipments
                .Where(s => s.Status == "Arrived" && !s.IsCollected)
                .Select(s => new { s.WaybillNumber, s.SenderName, s.SenderPhoneNumber, s.ReceiverName, s.ReceiverPhoneNumber, TotalWeight = 0.0m })
                .ToListAsync();

            shipmentsForCollection.AddRange(regularShipments);
            shipmentsForCollection.AddRange(merchantShipments);
            shipmentsForCollection.AddRange(genericShipments);
            
            return View(shipmentsForCollection.OrderBy(s => s.WaybillNumber).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Release(ShipmentCollection collectionDetails)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Please fill all required fields.";
                return RedirectToAction("ReleasedShipments");
            }
            
            var user = await _userManager.GetUserAsync(User);
            collectionDetails.CollectionDate = System.DateTime.Now;
            collectionDetails.ReleasedBy = user.Email;

            _context.ShipmentCollections.Add(collectionDetails);
            await _context.SaveChangesAsync();

            string senderPhoneNumber = null;

            var regularShipment = await _context.Shipments.FirstOrDefaultAsync(s => s.WaybillNumber == collectionDetails.WaybillNumber);
            if(regularShipment != null)
            {
                regularShipment.IsCollected = true;
                regularShipment.ShipmentCollectionId = collectionDetails.Id;
                senderPhoneNumber = regularShipment.SenderPhoneNumber;
            }

            var merchantShipment = await _context.MerchantShipments.Include(s => s.Merchant).FirstOrDefaultAsync(s => s.WaybillNumber == collectionDetails.WaybillNumber);
            if(merchantShipment != null)
            {
                merchantShipment.IsCollected = true;
                merchantShipment.ShipmentCollectionId = collectionDetails.Id;
                senderPhoneNumber = merchantShipment.Merchant?.BusinessPhoneNumber;
            }
            
            var genericShipment = await _context.GenericShipments.FirstOrDefaultAsync(s => s.WaybillNumber == collectionDetails.WaybillNumber);
            if(genericShipment != null)
            {
                genericShipment.IsCollected = true;
                genericShipment.ShipmentCollectionId = collectionDetails.Id;
                senderPhoneNumber = genericShipment.SenderPhoneNumber;
            }

            await _context.SaveChangesAsync();

            // --- THE FIX: Send the SMS to the sender after saving ---
            if (!string.IsNullOrEmpty(senderPhoneNumber))
            {
                // "Fire and forget" the SMS task
                _ = _smsService.SendCollectionSmsAsync(senderPhoneNumber, collectionDetails.WaybillNumber, collectionDetails.CollectedByName);
            }

            TempData["success"] = $"Shipment {collectionDetails.WaybillNumber} has been released successfully.";
            return RedirectToAction("ReleasedShipments");
        }

        public async Task<IActionResult> CollectedHistory()
        {
            var collections = await _context.ShipmentCollections
                .OrderByDescending(c => c.CollectionDate)
                .ToListAsync();
            return View(collections);
        }
    }
}