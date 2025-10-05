// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Rendering;
// using Microsoft.EntityFrameworkCore;
// using Newtonsoft.Json;
// using SMS.DataContext;
// using SMS.IRepository;
// using SMS.Models;
// using SMS.Models.ViewModels;
//
//
// namespace SMS.Controllers
// {
//     [Authorize]
//     public class ShipmentController : Controller
//     {
//         private readonly IUnitOfWork _unitOfWork;
//         private readonly UserManager<ApplicationUser> _userManager;
//         private readonly AppDbContext _context;
//
//         public ShipmentController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, AppDbContext context)
//         {
//             _unitOfWork = unitOfWork;
//             _userManager = userManager;
//             _context = context;
//         }
//
//         // STEP 1: Show the main shipment creation form
//         public async Task<IActionResult> CreateShipment()
//         {
//             var user = await _userManager.GetUserAsync(User);
//             var vm = new CreateShipmentVM
//             {
//                 LoggedInUserEmail = user.Email,
//                 States = new SelectList(await _context.States.ToListAsync(), "Id", "Name"),
//                 PackagingItems = new SelectList(await _context.PackagingPrices.Where(p => p.IsActive).ToListAsync(), "Id", "Name"),
//                 InsuranceOptions = new SelectList(await _context.Insurances.ToListAsync(), "Id", "Name")
//             };
//             return View(vm);
//         }
//
//         // STEP 2: Calculate final price and show the review page
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> ReviewShipment(CreateShipmentVM vm)
//         {
//             if (!ModelState.IsValid)
//             {
//                 // Repopulate required data if validation fails
//                 var user = await _userManager.GetUserAsync(User);
//                 vm.LoggedInUserEmail = user.Email;
//                 vm.States = new SelectList(await _context.States.ToListAsync(), "Id", "Name");
//                 vm.PackagingItems = new SelectList(await _context.PackagingPrices.Where(p => p.IsActive).ToListAsync(), "Id", "Name");
//                 vm.InsuranceOptions = new SelectList(await _context.Insurances.ToListAsync(), "Id", "Name");
//                 TempData["error"] = "Please fill out all required fields.";
//                 return View("CreateShipment", vm);
//             }
//
//             vm.PriceDetails = await CalculateShipmentPrice(vm);
//             TempData["ShipmentForReview"] = JsonConvert.SerializeObject(vm);
//             return View("YourShipmentDetails", vm);
//         }
//
//        
//         
//         
//     
// [HttpPost]
// [ValidateAntiForgeryToken]
// public async Task<IActionResult> ConfirmShipment(string couponCode = null)
// {
//     var vmJson = TempData["ShipmentForReview"] as string;
//     if (string.IsNullOrEmpty(vmJson))
//     {
//         return Json(new { success = false, message = "Session expired. Please create the shipment again." });
//     }
//     var vm = JsonConvert.DeserializeObject<CreateShipmentVM>(vmJson);
//     
//     var user = await _userManager.GetUserAsync(User);
//     var employee = await _context.Employees.Include(e => e.Location).FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);
//     if (employee == null) 
//     {
//         return Json(new { success = false, message = "Could not find an employee record for the current user." });
//     }
//
//     var finalPriceDetails = await CalculateShipmentPrice(vm);
//     if (!string.IsNullOrEmpty(couponCode))
//     {
//         var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code.ToUpper() == couponCode.ToUpper() && c.IsActive);
//         if (coupon != null)
//         {
//             decimal discount = (coupon.DiscountType == DiscountType.Percentage) ? (finalPriceDetails.TotalToPay * (coupon.DiscountValue / 100)) : coupon.DiscountValue;
//             finalPriceDetails.Discount = discount;
//             finalPriceDetails.TotalToPay = Math.Max(0, finalPriceDetails.TotalToPay - discount);
//         }
//     }
//
//     Shipment newShipment;
//     using (var transaction = await _context.Database.BeginTransactionAsync())
//     {
//         newShipment = new Shipment
//         {
//             SenderName = vm.SenderName, SenderPhoneNumber = vm.SenderPhoneNumber, SenderAddress = vm.SenderAddress,
//             ReceiverName = vm.ReceiverName, ReceiverPhoneNumber = vm.ReceiverPhoneNumber, ReceiverAddress = vm.ReceiverAddress,
//             ReceiverStateId = vm.ReceiverStateId, DestinationLocationId = vm.DestinationLocationId,
//             PaymentMethod = vm.PaymentMethod, DeclaredValue = vm.DeclaredValue, InsuranceId = vm.InsuranceId,
//             ShipmentCost = finalPriceDetails.ShipmentCost, PackagingCost = finalPriceDetails.PackagingCost,
//             InsuranceCost = finalPriceDetails.InsuranceCost, Vat = finalPriceDetails.Vat, TotalCost = finalPriceDetails.TotalToPay,
//             DateCreated = DateTime.Now, CreatedBy = user.Email, Status = "Pending"
//         };
//         _context.Shipments.Add(newShipment);
//         await _context.SaveChangesAsync();
//
//         foreach (var itemVM in vm.Items)
//         {
//             _context.ShipmentItems.Add(new ShipmentItem
//             {
//                 ShipmentId = newShipment.Id, Description = itemVM.Description, Weight = itemVM.Weight,
//                 Condition = itemVM.Condition, PackagingPriceId = itemVM.PackagingPriceId, NumberOfPackagingItems = itemVM.NumberOfPackagingItems
//             });
//         }
//         await _context.SaveChangesAsync();
//
//         var destLocation = await _context.Locations.FindAsync(newShipment.DestinationLocationId);
//         newShipment.WaybillNumber = GenerateWaybill(employee.Location.LocationCode, destLocation.LocationCode);
//         await _context.SaveChangesAsync();
//         await transaction.CommitAsync();
//     }
//     
//     // --- THE FIX: Instead of RedirectToAction, return a JSON result with the URL ---
//     return Json(new { success = true, redirectUrl = Url.Action("Waybill", "Shipment", new { id = newShipment.Id }) });
// }
//         
//         // STEP 4: Display the final receipt
//         public async Task<IActionResult> Waybill(int id)
//         {
//             var shipment = await _context.Shipments
//                 .Include(s => s.Items).ThenInclude(i => i.PackagingPrice)
//                 .Include(s => s.DestinationLocation).ThenInclude(l => l.State)
//                 .FirstOrDefaultAsync(s => s.Id == id);
//             
//             if (shipment == null) return NotFound();
//             return View(shipment);
//         }
//
//         // STEP 5: Display list of all shipments
//         // public async Task<IActionResult> AllShipments()
//         // {
//         //     var shipments = await _context.Shipments
//         //                         .Include(s => s.DestinationLocation.State)
//         //                         .OrderByDescending(s => s.DateCreated)
//         //                         .ToListAsync();
//         //     return View(shipments);
//         // }
//         
//         #region API Endpoints and Helpers
//
//         [HttpPost]
//         public async Task<JsonResult> CalculatePrice([FromBody] CreateShipmentVM vm)
//         {
//             if (vm == null || vm.Items == null || !vm.Items.Any()) return Json(new PriceDetailsVM());
//             var priceDetails = await CalculateShipmentPrice(vm);
//             return Json(priceDetails);
//         }
//
//         [HttpPost]
//         public async Task<JsonResult> ApplyCoupon(string couponCode, decimal currentTotal)
//         {
//             var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code.ToUpper() == couponCode.ToUpper() && c.IsActive);
//             if (coupon == null) return Json(new { success = false, message = "Invalid or inactive coupon code." });
//
//             decimal discount = (coupon.DiscountType == DiscountType.Percentage) ? (currentTotal * (coupon.DiscountValue / 100)) : coupon.DiscountValue;
//             decimal newTotal = currentTotal - discount;
//
//             return Json(new { success = true, discount = discount, newTotal = Math.Max(0, newTotal), message = "Coupon applied successfully!" });
//         }
//
//         private async Task<PriceDetailsVM> CalculateShipmentPrice(CreateShipmentVM vm)
//         {
//             var details = new PriceDetailsVM();
//             decimal totalWeight = vm.Items.Sum(i => i.Weight);
//
//             if (totalWeight <= 5) details.ShipmentCost = 2500;
//             else if (totalWeight <= 10) details.ShipmentCost = 4000;
//             else details.ShipmentCost = 4000 + (totalWeight - 10) * 300;
//
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
//             
//             details.SubTotal = details.ShipmentCost + details.PackagingCost + details.InsuranceCost;
//             details.Vat = details.SubTotal * 0.075m;
//             details.TotalToPay = details.SubTotal + details.Vat;
//             return details;
//         }
//
//         private string GenerateWaybill(string originCode, string destCode)
//         {
//             var random = new Random();
//             return $"{originCode.Substring(0, 2).ToUpper()}{random.Next(100000, 999999)}{destCode.Substring(0, 2).ToUpper()}";
//         }
//         
//         [HttpGet]
//         public async Task<JsonResult> GetLocationsByState(int stateId)
//         {
//             var locations = await _context.Locations.Where(l => l.StateId == stateId).Select(l => new { id = l.Id, name = l.Name }).ToListAsync();
//             return Json(locations);
//         }
//         #endregion
//     }
// }
//








using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SMS.DataContext;
using SMS.IRepository;
using SMS.Models;
using SMS.Models.ViewModels;


namespace SMS.Controllers
{
    [Authorize]
    public class ShipmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public ShipmentController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> CreateShipment()
        {
            var user = await _userManager.GetUserAsync(User);
            var vm = new CreateShipmentVM
            {
                LoggedInUserEmail = user.Email,
                States = new SelectList(await _context.States.ToListAsync(), "Id", "Name"),
                PackagingItems = new SelectList(await _context.PackagingPrices.Where(p => p.IsActive).ToListAsync(), "Id", "Name"),
                InsuranceOptions = new SelectList(await _context.Insurances.ToListAsync(), "Id", "Name")
            };
            return View(vm);
        }

        public async Task<IActionResult> EditShipment()
        {
            if (TempData["ShipmentForReview"] is string vmJson)
            {
                var vm = JsonConvert.DeserializeObject<CreateShipmentVM>(vmJson);
                TempData["ShipmentForReview"] = vmJson; 
                vm.LoggedInUserEmail = (await _userManager.GetUserAsync(User)).Email;
                vm.States = new SelectList(await _context.States.ToListAsync(), "Id", "Name", vm.ReceiverStateId);
                vm.PackagingItems = new SelectList(await _context.PackagingPrices.Where(p => p.IsActive).ToListAsync(), "Id", "Name");
                vm.InsuranceOptions = new SelectList(await _context.Insurances.ToListAsync(), "Id", "Name", vm.InsuranceId);
                return View("CreateShipment", vm);
            }
            return RedirectToAction("CreateShipment");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewShipment(CreateShipmentVM vm)
        {
            if (!ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                vm.LoggedInUserEmail = user.Email;
                vm.States = new SelectList(await _context.States.ToListAsync(), "Id", "Name");
                vm.PackagingItems = new SelectList(await _context.PackagingPrices.Where(p => p.IsActive).ToListAsync(), "Id", "Name");
                vm.InsuranceOptions = new SelectList(await _context.Insurances.ToListAsync(), "Id", "Name");
                TempData["error"] = "Please fill out all required fields.";
                return View("CreateShipment", vm);
            }

            vm.PriceDetails = await CalculateShipmentPrice(vm);
            TempData["ShipmentForReview"] = JsonConvert.SerializeObject(vm);
            return View("YourShipmentDetails", vm);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmShipment(string couponCode = null)
        {
            var vmJson = TempData["ShipmentForReview"] as string;
            if (string.IsNullOrEmpty(vmJson))
            {
                return Json(new { success = false, message = "Session expired. Please create the shipment again." });
            }
            var vm = JsonConvert.DeserializeObject<CreateShipmentVM>(vmJson);
            
            var user = await _userManager.GetUserAsync(User);
            var employee = await _context.Employees.Include(e => e.Location).FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);
            if (employee == null) 
            {
                return Json(new { success = false, message = "Could not find an employee record for the current user." });
            }

            var finalPriceDetails = await CalculateShipmentPrice(vm);
            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code.ToUpper() == couponCode.ToUpper() && c.IsActive);
                if (coupon != null)
                {
                    decimal discount = (coupon.DiscountType == DiscountType.Percentage) ? (finalPriceDetails.TotalToPay * (coupon.DiscountValue / 100)) : coupon.DiscountValue;
                    finalPriceDetails.TotalToPay = Math.Max(0, finalPriceDetails.TotalToPay - discount);
                }
            }

            Shipment newShipment;
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                newShipment = new Shipment
                {
                    SenderName = vm.SenderName, SenderPhoneNumber = vm.SenderPhoneNumber, SenderAddress = vm.SenderAddress,
                    ReceiverName = vm.ReceiverName, ReceiverPhoneNumber = vm.ReceiverPhoneNumber, ReceiverAddress = vm.ReceiverAddress,
                    ReceiverStateId = vm.ReceiverStateId, DestinationLocationId = vm.DestinationLocationId,
                    PaymentMethod = vm.PaymentMethod, DeclaredValue = vm.DeclaredValue, InsuranceId = vm.InsuranceId,
                    ShipmentCost = finalPriceDetails.ShipmentCost, PackagingCost = finalPriceDetails.PackagingCost,
                    InsuranceCost = finalPriceDetails.InsuranceCost, Vat = finalPriceDetails.Vat, TotalCost = finalPriceDetails.TotalToPay,
                    DateCreated = DateTime.Now, CreatedBy = user.Email, Status = "Pending"
                };
                _context.Shipments.Add(newShipment);
                await _context.SaveChangesAsync();

                foreach (var itemVM in vm.Items)
                {
                    _context.ShipmentItems.Add(new ShipmentItem
                    {
                        ShipmentId = newShipment.Id, Description = itemVM.Description, Weight = itemVM.Weight,
                        Condition = itemVM.Condition, PackagingPriceId = itemVM.PackagingPriceId, NumberOfPackagingItems = itemVM.NumberOfPackagingItems
                    });
                }
                await _context.SaveChangesAsync();

                var destLocation = await _context.Locations.FindAsync(newShipment.DestinationLocationId);
                newShipment.WaybillNumber = GenerateWaybill(employee.Location.LocationCode, destLocation.LocationCode);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            
            return Json(new { success = true, redirectUrl = Url.Action("Waybill", "Shipment", new { id = newShipment.Id }) });
        }
        
        public async Task<IActionResult> Waybill(int id)
        {
            var shipment = await _context.Shipments
                .Include(s => s.Items).ThenInclude(i => i.PackagingPrice)
                .Include(s => s.DestinationLocation).ThenInclude(l => l.State)
                .FirstOrDefaultAsync(s => s.Id == id);
            
            if (shipment == null) return NotFound();
            return View(shipment);
        }
        
        #region API Endpoints and Helpers
        [HttpPost]
        public async Task<JsonResult> CalculatePrice([FromBody] CreateShipmentVM vm)
        {
            if (vm == null || vm.Items == null || !vm.Items.Any()) return Json(new PriceDetailsVM());
            var priceDetails = await CalculateShipmentPrice(vm);
            return Json(priceDetails);
        }

        [HttpPost]
        public async Task<JsonResult> ApplyCoupon(string couponCode, decimal currentTotal)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code.ToUpper() == couponCode.ToUpper() && c.IsActive);
            if (coupon == null) return Json(new { success = false, message = "Invalid or inactive coupon code." });
            decimal discount = (coupon.DiscountType == DiscountType.Percentage) ? (currentTotal * (coupon.DiscountValue / 100)) : coupon.DiscountValue;
            decimal newTotal = currentTotal - discount;
            return Json(new { success = true, discount = discount, newTotal = Math.Max(0, newTotal), message = "Coupon applied successfully!" });
        }

        private async Task<PriceDetailsVM> CalculateShipmentPrice(CreateShipmentVM vm)
        {
            var details = new PriceDetailsVM();
            decimal totalWeight = vm.Items.Sum(i => i.Weight);
            if (totalWeight <= 5) details.ShipmentCost = 2500;
            else if (totalWeight <= 10) details.ShipmentCost = 4000;
            else details.ShipmentCost = 4000 + (totalWeight - 10) * 300;
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
            return details;
        }

        private string GenerateWaybill(string originCode, string destCode)
        {
            var random = new Random();
            return $"{originCode.Substring(0, 2).ToUpper()}{random.Next(100000, 999999)}{destCode.Substring(0, 2).ToUpper()}";
        }
        
        [HttpGet]
        public async Task<JsonResult> GetLocationsByState(int stateId)
        {
            var locations = await _context.Locations.Where(l => l.StateId == stateId).Select(l => new { id = l.Id, name = l.Name }).ToListAsync();
            return Json(locations);
        }
        #endregion
    }
}