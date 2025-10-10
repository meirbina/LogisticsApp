// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Rendering;
// using Microsoft.EntityFrameworkCore;
// using Newtonsoft.Json;
// using SMS.DataContext;
// using SMS.Models;
// using SMS.Models.ViewModels;
// using System;
// using System.Linq;
// using System.Threading.Tasks;
//
// namespace SMS.Controllers
// {
//     [Authorize]
//     public class MerchantShipmentController : Controller
//     {
//         private readonly UserManager<ApplicationUser> _userManager;
//         private readonly AppDbContext _context;
//
//         public MerchantShipmentController(UserManager<ApplicationUser> userManager, AppDbContext context)
//         {
//             _userManager = userManager;
//             _context = context;
//         }
//
//         public async Task<IActionResult> Create()
//         {
//             var vm = new CreateMerchantShipmentVM();
//             await RepopulateDropdownsForCreateView(vm);
//             return View(vm);
//         }
//
//         // --- NEW ACTION TO HANDLE THE "BACK TO EDIT" BUTTON ---
//         public async Task<IActionResult> EditShipment()
//         {
//             if (TempData["ShipmentForReview"] is string vmJson)
//             {
//                 var vm = JsonConvert.DeserializeObject<CreateMerchantShipmentVM>(vmJson);
//                 
//                 // Re-peek the data into TempData so it persists
//                 TempData["ShipmentForReview"] = vmJson;
//
//                 // Repopulate all dropdown lists before sending back to the view
//                 await RepopulateDropdownsForCreateView(vm);
//
//                 // Return the "Create" view but with the pre-filled model
//                 return View("Create", vm);
//             }
//             // If TempData is lost, redirect to a fresh form
//             return RedirectToAction("Create");
//         }
//
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> ReviewShipment(CreateMerchantShipmentVM vm)
//         {
//             if (!ModelState.IsValid)
//             {
//                 TempData["error"] = "Please fill out all required fields.";
//                 await RepopulateDropdownsForCreateView(vm);
//                 return View("Create", vm);
//             }
//
//             var (priceDetails, error) = await CalculateMerchantShipmentPrice(vm);
//             if (!string.IsNullOrEmpty(error))
//             {
//                 TempData["error"] = error;
//                 await RepopulateDropdownsForCreateView(vm);
//                 return View("Create", vm);
//             }
//
//             await RepopulateDropdownsForCreateView(vm); // Repopulate for the Review page
//             vm.PriceDetails = priceDetails;
//             TempData["ShipmentForReview"] = JsonConvert.SerializeObject(vm);
//             
//             return View("Review", vm);
//         }
//
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> ConfirmShipment(string couponCode = null)
//         {
//             var vmJson = TempData["ShipmentForReview"] as string;
//             if (string.IsNullOrEmpty(vmJson))
//             {
//                 return Json(new { success = false, message = "Session expired. Please create the shipment again." });
//             }
//             var vm = JsonConvert.DeserializeObject<CreateMerchantShipmentVM>(vmJson);
//             
//             var user = await _userManager.GetUserAsync(User);
//             var employee = await _context.Employees.Include(e => e.Location).FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);
//             if (employee == null) return Json(new { success = false, message = "Could not find an employee record for the current user." });
//
//             var (finalPriceDetails, error) = await CalculateMerchantShipmentPrice(vm);
//             if (!string.IsNullOrEmpty(error)) return Json(new { success = false, message = error });
//             
//             if (!string.IsNullOrEmpty(couponCode))
//             {
//                 var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code.ToUpper() == couponCode.ToUpper() && c.IsActive);
//                 if (coupon != null)
//                 {
//                     decimal discount = (coupon.DiscountType == DiscountType.Percentage) ? (finalPriceDetails.TotalToPay * (coupon.DiscountValue / 100)) : coupon.DiscountValue;
//                     finalPriceDetails.TotalToPay = Math.Max(0, finalPriceDetails.TotalToPay - discount);
//                 }
//             }
//
//             MerchantShipment newShipment;
//             using (var transaction = await _context.Database.BeginTransactionAsync())
//             {
//                 var destLocation = await _context.Locations.FindAsync(vm.DestinationLocationId);
//                 newShipment = new MerchantShipment
//                 {
//                     MerchantId = vm.MerchantId,
//                     ReceiverName = vm.ReceiverName, ReceiverPhoneNumber = vm.ReceiverPhoneNumber, ReceiverAddress = vm.ReceiverAddress,
//                     ReceiverStateId = vm.ReceiverStateId, DestinationLocationId = vm.DestinationLocationId,
//                     PaymentMethod = vm.PaymentMethod, DeclaredValue = vm.DeclaredValue, InsuranceId = vm.InsuranceId,
//                     ShipmentCost = finalPriceDetails.ShipmentCost, PackagingCost = finalPriceDetails.PackagingCost,
//                     InsuranceCost = finalPriceDetails.InsuranceCost, Vat = finalPriceDetails.Vat, TotalCost = finalPriceDetails.TotalToPay,
//                     DateCreated = DateTime.Now, CreatedBy = user.Email, Status = "Pending",
//                     WaybillNumber = GenerateWaybill(employee.Location.LocationCode, destLocation.LocationCode)
//                 };
//                 _context.MerchantShipments.Add(newShipment);
//                 await _context.SaveChangesAsync();
//
//                 foreach (var itemVM in vm.Items)
//                 {
//                     _context.MerchantShipmentItems.Add(new MerchantShipmentItem
//                     {
//                         MerchantShipmentId = newShipment.Id, Description = itemVM.Description, Weight = itemVM.Weight,
//                         Condition = itemVM.Condition, PackagingPriceId = itemVM.PackagingPriceId, NumberOfPackagingItems = itemVM.NumberOfPackagingItems
//                     });
//                 }
//                 await _context.SaveChangesAsync();
//                 await transaction.CommitAsync();
//             }
//             
//             return Json(new { success = true, redirectUrl = Url.Action("Waybill", new { id = newShipment.Id }) });
//         }
//         
//         public async Task<IActionResult> Waybill(int id)
//         {
//             var shipment = await _context.MerchantShipments
//                 .Include(s => s.Merchant)
//                 .Include(s => s.Items).ThenInclude(i => i.PackagingPrice)
//                 .Include(s => s.DestinationLocation).ThenInclude(l => l.State)
//                 .FirstOrDefaultAsync(s => s.Id == id);
//             
//             if (shipment == null) return NotFound();
//             return View(shipment);
//         }
//
//         #region API and Helpers
//         [HttpGet]
//         public async Task<JsonResult> GetMerchantDetails(int merchantId)
//         {
//             var merchant = await _context.Merchants.FindAsync(merchantId);
//             return merchant == null ? Json(null) : Json(new {
//                 businessName = merchant.BusinessName,
//                 businessEmail = merchant.BusinessEmail,
//                 businessPhoneNumber = merchant.BusinessPhoneNumber,
//                 businessAddress = merchant.BusinessAddress
//             });
//         }
//
//         [HttpPost]
//         public async Task<JsonResult> CalculatePrice([FromBody] CreateMerchantShipmentVM vm)
//         {
//             if (vm == null || vm.MerchantId == 0)
//             {
//                 return Json(new { success = true, price = new PriceDetailsVM() });
//             }
//             var (priceDetails, error) = await CalculateMerchantShipmentPrice(vm);
//             if (!string.IsNullOrEmpty(error))
//             {
//                 return Json(new { success = false, message = error });
//             }
//             return Json(new { success = true, price = priceDetails });
//         }
//
//         private async Task<(PriceDetailsVM, string)> CalculateMerchantShipmentPrice(CreateMerchantShipmentVM vm)
//         {
//             var details = new PriceDetailsVM();
//             if (vm.Items == null || !vm.Items.Any())
//             {
//                 return (details, null);
//             }
//             var merchantPrices = await _context.WeightPrices.Where(p => p.MerchantId == vm.MerchantId).ToListAsync();
//             if (!merchantPrices.Any())
//             {
//                 return (null, "Pricing has not been set up for this merchant. Please configure their weight prices.");
//             }
//             foreach (var item in vm.Items)
//             {
//                 var priceRule = merchantPrices.FirstOrDefault(p => item.Weight > p.StartKg && item.Weight <= p.EndKg);
//                 if (priceRule == null)
//                 {
//                     return (null, $"Price not set for an item with weight {item.Weight}kg. Please check the merchant's pricing setup.");
//                 }
//                 details.ShipmentCost += priceRule.Price;
//             }
//             foreach (var item in vm.Items)
//             {
//                 if (item.PackagingPriceId > 0 && item.NumberOfPackagingItems > 0)
//                 {
//                     var packaging = await _context.PackagingPrices.FindAsync(item.PackagingPriceId);
//                     if (packaging != null) details.PackagingCost += packaging.Amount * item.NumberOfPackagingItems;
//                 }
//             }
//             if (vm.InsuranceId.HasValue && vm.InsuranceId > 0 && vm.DeclaredValue > 0)
//             {
//                 var insurance = await _context.Insurances.FindAsync(vm.InsuranceId.Value);
//                 if (insurance != null) details.InsuranceCost = (vm.DeclaredValue * insurance.Amount) / 100;
//             }
//             details.SubTotal = details.ShipmentCost + details.PackagingCost + details.InsuranceCost;
//             details.Vat = details.SubTotal * 0.075m;
//             details.TotalToPay = details.SubTotal + details.Vat;
//             return (details, null);
//         }
//         
//         private async Task RepopulateDropdownsForCreateView(CreateMerchantShipmentVM vm)
//         {
//             var user = await _userManager.GetUserAsync(User);
//             vm.LoggedInUserEmail = user.Email;
//             vm.MerchantList = new SelectList(await _context.Merchants.OrderBy(m => m.BusinessName).ToListAsync(), "Id", "BusinessName", vm.MerchantId);
//             vm.States = new SelectList(await _context.States.ToListAsync(), "Id", "Name", vm.ReceiverStateId);
//             vm.PackagingItems = new SelectList(await _context.PackagingPrices.Where(p => p.IsActive).ToListAsync(), "Id", "Name");
//             vm.InsuranceOptions = new SelectList(await _context.Insurances.ToListAsync(), "Id", "Name", vm.InsuranceId);
//         }
//
//         private string GenerateWaybill(string originCode, string destCode)
//         {
//             var random = new Random();
//             return $"{originCode.Substring(0, 2).ToUpper()}{random.Next(100000, 999999)}{destCode.Substring(0, 2).ToUpper()}";
//         }
//         #endregion
//     }
// }
//
//
//
//
//









using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SMS.DataContext;
using SMS.Models;
using SMS.Models.ViewModels;
using SMS.Services; // <-- ADD THIS USING STATEMENT
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Controllers
{
    [Authorize]
    public class MerchantShipmentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly ISmsService _smsService; // <-- INJECT THE SERVICE

        public MerchantShipmentController(UserManager<ApplicationUser> userManager, AppDbContext context, ISmsService smsService)
        {
            _userManager = userManager;
            _context = context;
            _smsService = smsService; // <-- INITIALIZE THE SERVICE
        }

        public async Task<IActionResult> Create()
        {
            var vm = new CreateMerchantShipmentVM();
            await RepopulateDropdownsForCreateView(vm);
            return View(vm);
        }

        public async Task<IActionResult> EditShipment()
        {
            if (TempData["ShipmentForReview"] is string vmJson)
            {
                var vm = JsonConvert.DeserializeObject<CreateMerchantShipmentVM>(vmJson);
                TempData["ShipmentForReview"] = vmJson;
                await RepopulateDropdownsForCreateView(vm);
                return View("Create", vm);
            }
            return RedirectToAction("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewShipment(CreateMerchantShipmentVM vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Please fill out all required fields.";
                await RepopulateDropdownsForCreateView(vm);
                return View("Create", vm);
            }

            var (priceDetails, error) = await CalculateMerchantShipmentPrice(vm);
            if (!string.IsNullOrEmpty(error))
            {
                TempData["error"] = error;
                await RepopulateDropdownsForCreateView(vm);
                return View("Create", vm);
            }

            await RepopulateDropdownsForCreateView(vm);
            vm.PriceDetails = priceDetails;
            TempData["ShipmentForReview"] = JsonConvert.SerializeObject(vm);
            
            return View("Review", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmShipment(string couponCode = null)
        {
            var vmJson = TempData["ShipmentForReview"] as string;
            if (string.IsNullOrEmpty(vmJson))
                return Json(new { success = false, message = "Session expired. Please create the shipment again." });
            
            var vm = JsonConvert.DeserializeObject<CreateMerchantShipmentVM>(vmJson);
            var user = await _userManager.GetUserAsync(User);
            var employee = await _context.Employees.Include(e => e.Location).FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);
            if (employee == null) return Json(new { success = false, message = "Could not find an employee record for the current user." });

            var (finalPriceDetails, error) = await CalculateMerchantShipmentPrice(vm);
            if (!string.IsNullOrEmpty(error)) return Json(new { success = false, message = error });
            
            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code.ToUpper() == couponCode.ToUpper() && c.IsActive);
                if (coupon != null)
                {
                    decimal discount = (coupon.DiscountType == DiscountType.Percentage) ? (finalPriceDetails.TotalToPay * (coupon.DiscountValue / 100)) : coupon.DiscountValue;
                    finalPriceDetails.TotalToPay = Math.Max(0, finalPriceDetails.TotalToPay - discount);
                }
            }

            var merchant = await _context.Merchants.FindAsync(vm.MerchantId);
            if (merchant == null) return Json(new { success = false, message = "Selected merchant not found." });

            MerchantShipment newShipment;
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var destLocation = await _context.Locations.FindAsync(vm.DestinationLocationId);
                newShipment = new MerchantShipment
                {
                    MerchantId = vm.MerchantId, ReceiverName = vm.ReceiverName, ReceiverPhoneNumber = vm.ReceiverPhoneNumber, 
                    ReceiverAddress = vm.ReceiverAddress, ReceiverStateId = vm.ReceiverStateId, DestinationLocationId = vm.DestinationLocationId,
                    PaymentMethod = vm.PaymentMethod, DeclaredValue = vm.DeclaredValue, InsuranceId = vm.InsuranceId,
                    ShipmentCost = finalPriceDetails.ShipmentCost, PackagingCost = finalPriceDetails.PackagingCost,
                    InsuranceCost = finalPriceDetails.InsuranceCost, Vat = finalPriceDetails.Vat, TotalCost = finalPriceDetails.TotalToPay,
                    DateCreated = DateTime.Now, CreatedBy = user.Email, Status = "Pending",
                    WaybillNumber = GenerateWaybill(employee.Location.LocationCode, destLocation.LocationCode)
                };
                _context.MerchantShipments.Add(newShipment);
                await _context.SaveChangesAsync();

                foreach (var itemVM in vm.Items)
                {
                    _context.MerchantShipmentItems.Add(new MerchantShipmentItem {
                        MerchantShipmentId = newShipment.Id, Description = itemVM.Description, Weight = itemVM.Weight,
                        Condition = itemVM.Condition, PackagingPriceId = itemVM.PackagingPriceId, NumberOfPackagingItems = itemVM.NumberOfPackagingItems
                    });
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            
            // --- THE FIX: Send the SMS after the transaction is successfully committed ---
            if (!string.IsNullOrEmpty(merchant.BusinessPhoneNumber))
            {
                // "Fire and forget" this task so it doesn't slow down the user's response.
                _ = _smsService.SendShipmentCreationSmsAsync(merchant.BusinessPhoneNumber, newShipment.WaybillNumber, newShipment.TotalCost);
            }
            
            return Json(new { success = true, redirectUrl = Url.Action("Waybill", new { id = newShipment.Id }) });
        }
        
        public async Task<IActionResult> Waybill(int id)
        {
            var shipment = await _context.MerchantShipments
                .Include(s => s.Merchant).Include(s => s.Items).ThenInclude(i => i.PackagingPrice)
                .Include(s => s.DestinationLocation).ThenInclude(l => l.State)
                .FirstOrDefaultAsync(s => s.Id == id);
            
            if (shipment == null) return NotFound();
            return View(shipment);
        }

        #region API and Helpers
        [HttpGet]
        public async Task<JsonResult> GetMerchantDetails(int merchantId)
        {
            var merchant = await _context.Merchants.FindAsync(merchantId);
            return merchant == null ? Json(null) : Json(new {
                businessName = merchant.BusinessName,
                businessEmail = merchant.BusinessEmail,
                businessPhoneNumber = merchant.BusinessPhoneNumber,
                businessAddress = merchant.BusinessAddress
            });
        }

        [HttpPost]
        public async Task<JsonResult> CalculatePrice([FromBody] CreateMerchantShipmentVM vm)
        {
            if (vm == null || vm.MerchantId == 0)
                return Json(new { success = true, price = new PriceDetailsVM() });
            
            var (priceDetails, error) = await CalculateMerchantShipmentPrice(vm);
            if (!string.IsNullOrEmpty(error))
                return Json(new { success = false, message = error });
            
            return Json(new { success = true, price = priceDetails });
        }

        private async Task<(PriceDetailsVM, string)> CalculateMerchantShipmentPrice(CreateMerchantShipmentVM vm)
        {
            var details = new PriceDetailsVM();
            if (vm.Items == null || !vm.Items.Any())
                return (details, null);
            
            var merchantPrices = await _context.WeightPrices.Where(p => p.MerchantId == vm.MerchantId).ToListAsync();
            if (!merchantPrices.Any())
                return (null, "Pricing has not been set up for this merchant. Please configure their weight prices.");
            
            foreach (var item in vm.Items)
            {
                var priceRule = merchantPrices.FirstOrDefault(p => item.Weight > p.StartKg && item.Weight <= p.EndKg);
                if (priceRule == null)
                    return (null, $"Price not set for an item with weight {item.Weight}kg. Please check the merchant's pricing setup.");
                details.ShipmentCost += priceRule.Price;
            }
            foreach (var item in vm.Items)
            {
                if (item.PackagingPriceId > 0 && item.NumberOfPackagingItems > 0)
                {
                    var packaging = await _context.PackagingPrices.FindAsync(item.PackagingPriceId);
                    if (packaging != null) details.PackagingCost += packaging.Amount * item.NumberOfPackagingItems;
                }
            }
            if (vm.InsuranceId.HasValue && vm.InsuranceId > 0 && vm.DeclaredValue > 0)
            {
                var insurance = await _context.Insurances.FindAsync(vm.InsuranceId.Value);
                if (insurance != null) details.InsuranceCost = (vm.DeclaredValue * insurance.Amount) / 100;
            }
            details.SubTotal = details.ShipmentCost + details.PackagingCost + details.InsuranceCost;
            details.Vat = details.SubTotal * 0.075m;
            details.TotalToPay = details.SubTotal + details.Vat;
            return (details, null);
        }
        
        private async Task RepopulateDropdownsForCreateView(CreateMerchantShipmentVM vm)
        {
            var user = await _userManager.GetUserAsync(User);
            vm.LoggedInUserEmail = user.Email;
            vm.MerchantList = new SelectList(await _context.Merchants.OrderBy(m => m.BusinessName).ToListAsync(), "Id", "BusinessName", vm.MerchantId);
            vm.States = new SelectList(await _context.States.ToListAsync(), "Id", "Name", vm.ReceiverStateId);
            vm.PackagingItems = new SelectList(await _context.PackagingPrices.Where(p => p.IsActive).ToListAsync(), "Id", "Name");
            vm.InsuranceOptions = new SelectList(await _context.Insurances.ToListAsync(), "Id", "Name", vm.InsuranceId);
        }

        private string GenerateWaybill(string originCode, string destCode)
        {
            var random = new Random();
            return $"{originCode.Substring(0, 2).ToUpper()}{random.Next(100000, 999999)}{destCode.Substring(0, 2).ToUpper()}";
        }
        #endregion
    }
}