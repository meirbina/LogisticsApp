using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SMS.DataContext;
using SMS.Models;
using SMS.Models.ViewModels;

namespace SMS.Controllers
{
    [Authorize]
    public class GenericShipmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GenericShipmentController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Create()
        {
            var vm = new CreateGenericShipmentVM();
            await RepopulateDropdowns(vm);
            return View(vm);
        }

        public async Task<IActionResult> EditShipment()
        {
            if (TempData["GenericShipmentForReview"] is string vmJson)
            {
                var vm = JsonConvert.DeserializeObject<CreateGenericShipmentVM>(vmJson);
                TempData["GenericShipmentForReview"] = vmJson;
                await RepopulateDropdowns(vm);
                return View("Create", vm);
            }
            return RedirectToAction("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewShipment(CreateGenericShipmentVM vm)
        {
            if ((vm.ShipmentType == "Merchant" && (!vm.MerchantId.HasValue || vm.MerchantId == 0)) ||
                (vm.ShipmentType == "Regular" && string.IsNullOrEmpty(vm.SenderName)))
            { ModelState.AddModelError("", "Please fill all required sender fields."); }
            if (vm.Items == null || !vm.Items.Any() || vm.Items.Any(i => i.GenericPackageId == 0))
            { ModelState.AddModelError("", "Please add at least one valid package to the shipment."); }

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Please fill all required fields correctly.";
                await RepopulateDropdowns(vm);
                return View("Create", vm);
            }

            vm.PriceDetails = await CalculatePriceInternal(vm);
            TempData["GenericShipmentForReview"] = JsonConvert.SerializeObject(vm);
            await RepopulateDropdowns(vm);
            return View("Review", vm);
        }

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> ConfirmShipment(string couponCode = null)
        // {
        //     var vmJson = TempData["GenericShipmentForReview"] as string;
        //     if (string.IsNullOrEmpty(vmJson))
        //         return Json(new { success = false, message = "Session expired." });
        //
        //     var vm = JsonConvert.DeserializeObject<CreateGenericShipmentVM>(vmJson);
        //     var user = await _userManager.GetUserAsync(User);
        //     var employee = await _context.Employees.Include(e => e.Location).FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);
        //     if (employee == null) return Json(new { success = false, message = "User is not a valid employee." });
        //
        //     var finalPrice = await CalculatePriceInternal(vm);
        //     if (!string.IsNullOrEmpty(couponCode))
        //     {
        //         var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code.ToUpper() == couponCode.ToUpper() && c.IsActive);
        //         if (coupon != null)
        //         {
        //             decimal discount = (coupon.DiscountType == DiscountType.Percentage) ? (finalPrice.TotalToPay * (coupon.DiscountValue / 100)) : coupon.DiscountValue;
        //             finalPrice.TotalToPay = Math.Max(0, finalPrice.TotalToPay - discount);
        //         }
        //     }
        //
        //     // --- THE FIX: Fetch external data BEFORE creating the new object ---
        //     Merchant merchant = null;
        //     if (vm.ShipmentType == "Merchant" && vm.MerchantId.HasValue)
        //     {
        //         merchant = await _context.Merchants.FindAsync(vm.MerchantId.Value);
        //     }
        //     var destLocation = await _context.Locations.FindAsync(vm.DestinationLocationId);
        //     
        //     var newShipment = new GenericShipment
        //     {
        //         ShipmentType = vm.ShipmentType,
        //         MerchantId = (merchant != null) ? merchant.Id : null,
        //         SenderName = (merchant != null) ? merchant.BusinessName : vm.SenderName,
        //         SenderPhoneNumber = (merchant != null) ? merchant.BusinessPhoneNumber : vm.SenderPhoneNumber,
        //         SenderAddress = (merchant != null) ? merchant.BusinessAddress : vm.SenderAddress,
        //         ReceiverName = vm.ReceiverName, 
        //         ReceiverPhoneNumber = vm.ReceiverPhoneNumber, 
        //         ReceiverAddress = vm.ReceiverAddress,
        //         DestinationLocationId = vm.DestinationLocationId, 
        //         TotalCost = finalPrice.TotalToPay, 
        //         DateCreated = DateTime.Now,
        //         CreatedBy = user.Email, 
        //         Status = "Pending",
        //         PaymentMethod = vm.PaymentMethod,
        //         WaybillNumber = $"GEN-{employee.Location.LocationCode.ToUpper()}{destLocation.LocationCode.ToUpper()}{DateTime.Now:yyMMddHHmmss}"
        //     };
        //     
        //     _context.GenericShipments.Add(newShipment);
        //     await _context.SaveChangesAsync();
        //     
        //     foreach(var item in vm.Items)
        //     {
        //         _context.GenericShipmentItems.Add(new GenericShipmentItem { GenericShipmentId = newShipment.Id, GenericPackageId = item.GenericPackageId, Quantity = item.Quantity });
        //     }
        //     await _context.SaveChangesAsync();
        //     
        //     return Json(new { success = true, redirectUrl = Url.Action("Waybill", new { id = newShipment.Id }) });
        // }
        
        
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ConfirmShipment(string couponCode = null)
{
    var vmJson = TempData["GenericShipmentForReview"] as string;
    if (string.IsNullOrEmpty(vmJson))
        return Json(new { success = false, message = "Session expired." });

    var vm = JsonConvert.DeserializeObject<CreateGenericShipmentVM>(vmJson);
    var user = await _userManager.GetUserAsync(User);
    var employee = await _context.Employees.Include(e => e.Location).FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);
    if (employee == null) return Json(new { success = false, message = "User is not a valid employee." });

    var finalPrice = await CalculatePriceInternal(vm);
    if (!string.IsNullOrEmpty(couponCode))
    {
        var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code.ToUpper() == couponCode.ToUpper() && c.IsActive);
        if (coupon != null)
        {
            decimal discount = (coupon.DiscountType == DiscountType.Percentage) ? (finalPrice.TotalToPay * (coupon.DiscountValue / 100)) : coupon.DiscountValue;
            finalPrice.TotalToPay = Math.Max(0, finalPrice.TotalToPay - discount);
        }
    }

    Merchant merchant = null;
    if (vm.ShipmentType == "Merchant" && vm.MerchantId.HasValue)
    {
        merchant = await _context.Merchants.FindAsync(vm.MerchantId.Value);
    }
    var destLocation = await _context.Locations.FindAsync(vm.DestinationLocationId);
    
    // --- THE FIX: Use the universal GenerateWaybill method ---
    // The transaction block is also added for data integrity, just like the other controllers.
    GenericShipment newShipment;
    using (var transaction = await _context.Database.BeginTransactionAsync())
    {
        newShipment = new GenericShipment
        {
            ShipmentType = vm.ShipmentType,
            MerchantId = (merchant != null) ? merchant.Id : null,
            SenderName = (merchant != null) ? merchant.BusinessName : vm.SenderName,
            SenderPhoneNumber = (merchant != null) ? merchant.BusinessPhoneNumber : vm.SenderPhoneNumber,
            SenderAddress = (merchant != null) ? merchant.BusinessAddress : vm.SenderAddress,
            ReceiverName = vm.ReceiverName, 
            ReceiverPhoneNumber = vm.ReceiverPhoneNumber, 
            ReceiverAddress = vm.ReceiverAddress,
            DestinationLocationId = vm.DestinationLocationId, 
            TotalCost = finalPrice.TotalToPay, 
            DateCreated = DateTime.Now,
            CreatedBy = user.Email, 
            Status = "Pending",
            PaymentMethod = vm.PaymentMethod,
            // WaybillNumber is now assigned after saving, just like the other controllers
        };
        
        _context.GenericShipments.Add(newShipment);
        await _context.SaveChangesAsync(); // Save once to get the newShipment.Id
        
        foreach(var item in vm.Items)
        {
            _context.GenericShipmentItems.Add(new GenericShipmentItem { GenericShipmentId = newShipment.Id, GenericPackageId = item.GenericPackageId, Quantity = item.Quantity });
        }
        await _context.SaveChangesAsync();

        // Now, generate and assign the waybill number in the correct format
        newShipment.WaybillNumber = GenerateWaybill(employee.Location.LocationCode, destLocation.LocationCode);
        await _context.SaveChangesAsync();
        
        await transaction.CommitAsync();
    }
    
    return Json(new { success = true, redirectUrl = Url.Action("Waybill", new { id = newShipment.Id }) });
}
        
        public async Task<IActionResult> Waybill(int id)
        {
            var shipment = await _context.GenericShipments
                .Include(s => s.Merchant).Include(s => s.DestinationLocation.State)
                .Include(s => s.Items).ThenInclude(i => i.GenericPackage)
                .FirstOrDefaultAsync(s => s.Id == id);
            
            if(shipment == null) return NotFound();
            return View(shipment);
        }
        
        private string GenerateWaybill(string originCode, string destCode)
        {
            var random = new Random();
            return $"{originCode.Substring(0, 2).ToUpper()}{random.Next(100000, 999999)}{destCode.Substring(0, 2).ToUpper()}";
        }

        [HttpPost]
        public async Task<JsonResult> CalculatePrice([FromBody] CreateGenericShipmentVM vm)
        {
            if (vm == null || (vm.ShipmentType == "Merchant" && (!vm.MerchantId.HasValue || vm.MerchantId == 0)))
            {
                return Json(new { success = true, price = new PriceDetailsVM() });
            }
            var price = await CalculatePriceInternal(vm);
            return Json(new { success = true, price = price });
        }

        private async Task<PriceDetailsVM> CalculatePriceInternal(CreateGenericShipmentVM vm)
        {
            var details = new PriceDetailsVM();
            if(vm.Items == null || !vm.Items.Any()) return details;

            var packageIds = vm.Items.Select(i => i.GenericPackageId).ToList();
            var packages = await _context.GenericPackages.Where(p => packageIds.Contains(p.Id)).ToListAsync();
            
            foreach(var item in vm.Items)
            {
                var package = packages.FirstOrDefault(p => p.Id == item.GenericPackageId);
                if(package != null)
                {
                    var pricePerItem = (vm.ShipmentType == "Merchant" && vm.MerchantId.HasValue && vm.MerchantId > 0) 
                                       ? package.MerchantPrice 
                                       : package.RegularPrice;
                    details.ShipmentCost += pricePerItem * item.Quantity;
                }
            }
            details.TotalToPay = details.ShipmentCost;
            return details;
        }

        private async Task RepopulateDropdowns(CreateGenericShipmentVM vm)
        {
            var user = await _userManager.GetUserAsync(User);
            vm.LoggedInUserEmail = user.Email;
            vm.MerchantList = new SelectList(await _context.Merchants.OrderBy(m => m.BusinessName).ToListAsync(), "Id", "BusinessName", vm.MerchantId);
            vm.LocationList = new SelectList(await _context.Locations.Include(l => l.State).Select(l => new { l.Id, Name = $"{l.State.Name} ==> {l.Name}"}).ToListAsync(), "Id", "Name", vm.DestinationLocationId);
            vm.GenericPackageList = new SelectList(await _context.GenericPackages.Where(p => p.IsActive).ToListAsync(), "Id", "Name");
        }
    }
}