// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using SMS.DataContext;
// using SMS.Models;
// using SMS.Models.ViewModels;
//
//
// namespace SMS.Controllers
// {
//     [Authorize]
//     public class ReportController : Controller
//     {
//         private readonly AppDbContext _context;
//         private readonly UserManager<ApplicationUser> _userManager;
//
//         public ReportController(AppDbContext context, UserManager<ApplicationUser> userManager)
//         {
//             _context = context;
//             _userManager = userManager;
//         }
//
//         [Authorize(Roles = "Admin")]
//         public async Task<IActionResult> SalesReport(DateTime? startDate, DateTime? endDate)
//         {
//             var start = startDate ?? DateTime.Today;
//             // --- FIX: Ensure the end date includes the entire day (until 23:59:59) ---
//             var end = endDate ?? DateTime.Today;
//             var endOfDay = end.AddDays(1).AddTicks(-1);
//
//             var allShipments = new List<UniversalShipmentRecord>();
//             var regularShipments = await _context.Shipments.Include(s => s.Items).Where(s => s.DateCreated >= start && s.DateCreated <= endOfDay).ToListAsync();
//             var merchantShipments = await _context.MerchantShipments.Include(s => s.Merchant).Include(s => s.Items).Where(s => s.DateCreated >= start && s.DateCreated <= endOfDay).ToListAsync();
//
//             allShipments.AddRange(regularShipments.Select(s => new UniversalShipmentRecord {
//                 Date = s.DateCreated, CreatedBy = s.CreatedBy, TotalCost = s.TotalCost, Vat = s.Vat,
//                 PackagingCost = s.PackagingCost, PaymentMethod = s.PaymentMethod, WaybillNumber = s.WaybillNumber,
//                 TotalWeight = s.Items.Sum(i => i.Weight), SenderPhoneNumber = s.SenderPhoneNumber,
//                 ReceiverPhoneNumber = s.ReceiverPhoneNumber, ItemDescription = string.Join(", ", s.Items.Select(i => i.Description))
//             }));
//             allShipments.AddRange(merchantShipments.Select(s => new UniversalShipmentRecord {
//                 Date = s.DateCreated, CreatedBy = s.CreatedBy, TotalCost = s.TotalCost, Vat = s.Vat,
//                 PackagingCost = s.PackagingCost, PaymentMethod = s.PaymentMethod, WaybillNumber = s.WaybillNumber,
//                 TotalWeight = s.Items.Sum(i => i.Weight), SenderPhoneNumber = s.Merchant.BusinessPhoneNumber,
//                 ReceiverPhoneNumber = s.ReceiverPhoneNumber, ItemDescription = string.Join(", ", s.Items.Select(i => i.Description))
//             }));
//
//             if (!allShipments.Any())
//             {
//                 return View(new SalesReportVM { StartDate = start, EndDate = end, DailySales = new List<DailyLocationSale>(), Summary = new OverallSalesSummary() });
//             }
//
//             var employeeLocationMap = await _context.Employees
//                 .Include(e => e.ApplicationUser).Include(e => e.Location)
//                 .Where(e => e.ApplicationUser != null).ToDictionaryAsync(e => e.ApplicationUser.Email, e => e.Location.Name);
//             
//             // --- FIX: Group by both Date and Location for a daily breakdown ---
//             var dailySales = allShipments
//                 .GroupBy(s => new { Date = s.Date.Date, Location = employeeLocationMap.GetValueOrDefault(s.CreatedBy, "Unassigned / Deleted User") })
//                 .Select(g => new DailyLocationSale {
//                     LocationName = g.Key.Location, Date = g.Key.Date, TotalSales = g.Sum(s => s.TotalCost),
//                     TotalVat = g.Sum(s => s.Vat), TotalPackagingFee = g.Sum(s => s.PackagingCost),
//                     UserSales = g.GroupBy(ug => ug.CreatedBy)
//                                  .Select(ug => new UserSaleDetails {
//                                      UserEmail = ug.Key, TotalSalesByUser = ug.Sum(s => s.TotalCost),
//                                      Shipments = ug.Select(s => new ShipmentSaleDetails {
//                                          WaybillNumber = s.WaybillNumber, TotalWeight = s.TotalWeight,
//                                          SenderPhoneNumber = s.SenderPhoneNumber, ReceiverPhoneNumber = s.ReceiverPhoneNumber,
//                                          ItemDescription = s.ItemDescription, PaymentMethod = s.PaymentMethod, TotalCost = s.TotalCost
//                                      }).OrderBy(s => s.WaybillNumber).ToList()
//                                  }).OrderBy(us => us.UserEmail).ToList()
//                 })
//                 .OrderBy(ds => ds.LocationName).ThenBy(ds => ds.Date).ToList();
//
//             var summary = new OverallSalesSummary {
//                 GrandTotalSales = allShipments.Sum(s => s.TotalCost),
//                 TotalPos = allShipments.Where(s => s.PaymentMethod == "POS").Sum(s => s.TotalCost),
//                 TotalCash = allShipments.Where(s => s.PaymentMethod == "Cash").Sum(s => s.TotalCost),
//                 TotalTransfer = allShipments.Where(s => s.PaymentMethod == "Transfer").Sum(s => s.TotalCost)
//             };
//
//             var vm = new SalesReportVM {
//                 DailySales = dailySales, Summary = summary, StartDate = start, EndDate = end
//             };
//             return View(vm);
//         }
//
//         [Authorize]
//         public async Task<IActionResult> MySalesReport(DateTime? startDate, DateTime? endDate)
//         {
//             var start = startDate ?? DateTime.Today;
//             var end = endDate ?? DateTime.Today.AddDays(1).AddTicks(-1);
//             
//             var user = await _userManager.GetUserAsync(User);
//             if (user == null) return Unauthorized();
//
//             var allUserShipments = new List<UniversalShipmentRecord>();
//
//             var regularShipments = await _context.Shipments.Include(s => s.Items)
//                 .Where(s => s.CreatedBy == user.Email && s.DateCreated >= start && s.DateCreated <= end).ToListAsync();
//             
//             var merchantShipments = await _context.MerchantShipments.Include(s => s.Merchant).Include(s => s.Items)
//                 .Where(s => s.CreatedBy == user.Email && s.DateCreated >= start && s.DateCreated <= end).ToListAsync();
//
//             // --- FIX: Explicitly map ALL properties to fix the 0001-01-01 date and 0.00 cost bug ---
//             allUserShipments.AddRange(regularShipments.Select(s => new UniversalShipmentRecord {
//                 Date = s.DateCreated, CreatedBy = s.CreatedBy, TotalCost = s.TotalCost,
//                 Vat = s.Vat, PackagingCost = s.PackagingCost, PaymentMethod = s.PaymentMethod,
//                 WaybillNumber = s.WaybillNumber, TotalWeight = s.Items.Sum(i => i.Weight),
//                 SenderPhoneNumber = s.SenderPhoneNumber, ReceiverPhoneNumber = s.ReceiverPhoneNumber,
//                 ItemDescription = string.Join(", ", s.Items.Select(i => i.Description))
//             }));
//             allUserShipments.AddRange(merchantShipments.Select(s => new UniversalShipmentRecord {
//                 Date = s.DateCreated, CreatedBy = s.CreatedBy, TotalCost = s.TotalCost,
//                 Vat = s.Vat, PackagingCost = s.PackagingCost, PaymentMethod = s.PaymentMethod,
//                 WaybillNumber = s.WaybillNumber, TotalWeight = s.Items.Sum(i => i.Weight),
//                 SenderPhoneNumber = s.Merchant.BusinessPhoneNumber, ReceiverPhoneNumber = s.ReceiverPhoneNumber,
//                 ItemDescription = string.Join(", ", s.Items.Select(i => i.Description))
//             }));
//
//             var summary = new OverallSalesSummary {
//                 GrandTotalSales = allUserShipments.Sum(s => s.TotalCost),
//                 TotalPos = allUserShipments.Where(s => s.PaymentMethod == "POS").Sum(s => s.TotalCost),
//                 TotalCash = allUserShipments.Where(s => s.PaymentMethod == "Cash").Sum(s => s.TotalCost),
//                 TotalTransfer = allUserShipments.Where(s => s.PaymentMethod == "Transfer").Sum(s => s.TotalCost)
//             };
//             
//             var vm = new MySalesReportVM {
//                 Shipments = allUserShipments.OrderByDescending(s => s.Date).ToList(),
//                 Summary = summary, StartDate = start, EndDate = end.Date
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
using SMS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMS.Models;

namespace SMS.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SalesReport(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today;
            var end = endDate ?? DateTime.Today;
            var endOfDay = end.AddDays(1).AddTicks(-1);

            var allShipments = new List<UniversalShipmentRecord>();

            // 1. Fetch Regular Shipments
            var regularShipments = await _context.Shipments
                .Include(s => s.Items)
                .Where(s => s.DateCreated >= start && s.DateCreated <= endOfDay)
                .ToListAsync();

            // 2. Fetch Merchant Shipments
            var merchantShipments = await _context.MerchantShipments
                .Include(s => s.Merchant)
                .Include(s => s.Items)
                .Where(s => s.DateCreated >= start && s.DateCreated <= endOfDay)
                .ToListAsync();

            // 3. Fetch Generic Shipments
            var genericShipments = await _context.GenericShipments
                .Include(s => s.Items)
                .Where(s => s.DateCreated >= start && s.DateCreated <= endOfDay)
                .ToListAsync();

            // Map Regular Shipments to Universal Record
            allShipments.AddRange(regularShipments.Select(s => new UniversalShipmentRecord {
                Date = s.DateCreated, CreatedBy = s.CreatedBy, TotalCost = s.TotalCost, Vat = s.Vat,
                PackagingCost = s.PackagingCost, PaymentMethod = s.PaymentMethod, WaybillNumber = s.WaybillNumber,
                TotalWeight = s.Items.Sum(i => i.Weight), SenderPhoneNumber = s.SenderPhoneNumber,
                ReceiverPhoneNumber = s.ReceiverPhoneNumber, ItemDescription = string.Join(", ", s.Items.Select(i => i.Description))
            }));
            
            // Map Merchant Shipments to Universal Record
            allShipments.AddRange(merchantShipments.Select(s => new UniversalShipmentRecord {
                Date = s.DateCreated, CreatedBy = s.CreatedBy, TotalCost = s.TotalCost, Vat = s.Vat,
                PackagingCost = s.PackagingCost, PaymentMethod = s.PaymentMethod, WaybillNumber = s.WaybillNumber,
                TotalWeight = s.Items.Sum(i => i.Weight), SenderPhoneNumber = s.Merchant.BusinessPhoneNumber,
                ReceiverPhoneNumber = s.ReceiverPhoneNumber, ItemDescription = string.Join(", ", s.Items.Select(i => i.Description))
            }));

            // Map Generic Shipments to Universal Record
            allShipments.AddRange(genericShipments.Select(s => new UniversalShipmentRecord {
                Date = s.DateCreated, CreatedBy = s.CreatedBy, TotalCost = s.TotalCost, Vat = 0, // Generic has no VAT/Packaging
                PackagingCost = 0, PaymentMethod = s.PaymentMethod, WaybillNumber = s.WaybillNumber,
                TotalWeight = 0, // Generic doesn't use weight
                SenderPhoneNumber = s.SenderPhoneNumber, ReceiverPhoneNumber = s.ReceiverPhoneNumber,
                ItemDescription = "Generic Packages"
            }));

            if (!allShipments.Any())
            {
                return View(new SalesReportVM { StartDate = start, EndDate = end, DailySales = new List<DailyLocationSale>(), Summary = new OverallSalesSummary() });
            }

            var employeeLocationMap = await _context.Employees
                .Include(e => e.ApplicationUser).Include(e => e.Location)
                .Where(e => e.ApplicationUser != null).ToDictionaryAsync(e => e.ApplicationUser.Email, e => e.Location.Name);
            
            var dailySales = allShipments
                .GroupBy(s => new { Date = s.Date.Date, Location = employeeLocationMap.GetValueOrDefault(s.CreatedBy, "Unassigned / Deleted User") })
                .Select(g => new DailyLocationSale {
                    LocationName = g.Key.Location, Date = g.Key.Date, TotalSales = g.Sum(s => s.TotalCost),
                    TotalVat = g.Sum(s => s.Vat), TotalPackagingFee = g.Sum(s => s.PackagingCost),
                    UserSales = g.GroupBy(ug => ug.CreatedBy)
                                 .Select(ug => new UserSaleDetails {
                                     UserEmail = ug.Key, TotalSalesByUser = ug.Sum(s => s.TotalCost),
                                     Shipments = ug.Select(s => new ShipmentSaleDetails {
                                         WaybillNumber = s.WaybillNumber, TotalWeight = s.TotalWeight,
                                         SenderPhoneNumber = s.SenderPhoneNumber, ReceiverPhoneNumber = s.ReceiverPhoneNumber,
                                         ItemDescription = s.ItemDescription, PaymentMethod = s.PaymentMethod, TotalCost = s.TotalCost
                                     }).OrderBy(s => s.WaybillNumber).ToList()
                                 }).OrderBy(us => us.UserEmail).ToList()
                })
                .OrderBy(ds => ds.LocationName).ThenBy(ds => ds.Date).ToList();

            var summary = new OverallSalesSummary {
                GrandTotalSales = allShipments.Sum(s => s.TotalCost),
                TotalPos = allShipments.Where(s => s.PaymentMethod == "POS").Sum(s => s.TotalCost),
                TotalCash = allShipments.Where(s => s.PaymentMethod == "Cash").Sum(s => s.TotalCost),
                TotalTransfer = allShipments.Where(s => s.PaymentMethod == "Transfer").Sum(s => s.TotalCost)
            };

            var vm = new SalesReportVM {
                DailySales = dailySales, Summary = summary, StartDate = start, EndDate = end
            };
            return View(vm);
        }

        [Authorize]
        public async Task<IActionResult> MySalesReport(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today;
            var end = endDate ?? DateTime.Today.AddDays(1).AddTicks(-1);
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var allUserShipments = new List<UniversalShipmentRecord>();

            var regularShipments = await _context.Shipments.Include(s => s.Items)
                .Where(s => s.CreatedBy == user.Email && s.DateCreated >= start && s.DateCreated <= end).ToListAsync();
            
            var merchantShipments = await _context.MerchantShipments.Include(s => s.Merchant).Include(s => s.Items)
                .Where(s => s.CreatedBy == user.Email && s.DateCreated >= start && s.DateCreated <= end).ToListAsync();

            var genericShipments = await _context.GenericShipments.Include(s => s.Items)
                .Where(s => s.CreatedBy == user.Email && s.DateCreated >= start && s.DateCreated <= end).ToListAsync();

            allUserShipments.AddRange(regularShipments.Select(s => new UniversalShipmentRecord {
                Date = s.DateCreated, CreatedBy = s.CreatedBy, TotalCost = s.TotalCost,
                Vat = s.Vat, PackagingCost = s.PackagingCost, PaymentMethod = s.PaymentMethod,
                WaybillNumber = s.WaybillNumber, TotalWeight = s.Items.Sum(i => i.Weight),
                SenderPhoneNumber = s.SenderPhoneNumber, ReceiverPhoneNumber = s.ReceiverPhoneNumber,
                ItemDescription = string.Join(", ", s.Items.Select(i => i.Description))
            }));
            allUserShipments.AddRange(merchantShipments.Select(s => new UniversalShipmentRecord {
                Date = s.DateCreated, CreatedBy = s.CreatedBy, TotalCost = s.TotalCost,
                Vat = s.Vat, PackagingCost = s.PackagingCost, PaymentMethod = s.PaymentMethod,
                WaybillNumber = s.WaybillNumber, TotalWeight = s.Items.Sum(i => i.Weight),
                SenderPhoneNumber = s.Merchant.BusinessPhoneNumber, ReceiverPhoneNumber = s.ReceiverPhoneNumber,
                ItemDescription = string.Join(", ", s.Items.Select(i => i.Description))
            }));
            allUserShipments.AddRange(genericShipments.Select(s => new UniversalShipmentRecord {
                Date = s.DateCreated, CreatedBy = s.CreatedBy, TotalCost = s.TotalCost, Vat = 0,
                PackagingCost = 0, PaymentMethod = s.PaymentMethod, WaybillNumber = s.WaybillNumber,
                TotalWeight = 0, SenderPhoneNumber = s.SenderPhoneNumber,
                ReceiverPhoneNumber = s.ReceiverPhoneNumber, ItemDescription = "Generic Packages"
            }));

            var summary = new OverallSalesSummary {
                GrandTotalSales = allUserShipments.Sum(s => s.TotalCost),
                TotalPos = allUserShipments.Where(s => s.PaymentMethod == "POS").Sum(s => s.TotalCost),
                TotalCash = allUserShipments.Where(s => s.PaymentMethod == "Cash").Sum(s => s.TotalCost),
                TotalTransfer = allUserShipments.Where(s => s.PaymentMethod == "Transfer").Sum(s => s.TotalCost)
            };
            
            var vm = new MySalesReportVM {
                Shipments = allUserShipments.OrderByDescending(s => s.Date).ToList(),
                Summary = summary, StartDate = start, EndDate = end.Date
            };

            return View(vm);
        }
    }
}