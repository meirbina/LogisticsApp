using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS.DataContext;
using SMS.Models.ViewModels;


namespace SMS.Controllers
{
    [Authorize]
    public class TrackingController : Controller
    {
        private readonly AppDbContext _context;
        public TrackingController(AppDbContext context) { _context = context; }

        public IActionResult Index(TrackShipmentVM vm)
        {
            if (vm == null) vm = new TrackShipmentVM();
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            var results = new List<TrackedShipmentResult>();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return View("Index", new TrackShipmentVM { SearchTerm = searchTerm, Results = results });
            }

            // Search Regular Shipments
            var regular = await _context.Shipments.Include(s => s.Items)
                .Where(s => s.WaybillNumber == searchTerm || s.SenderPhoneNumber == searchTerm || s.ReceiverPhoneNumber == searchTerm)
                .Select(s => new TrackedShipmentResult {
                    WaybillNumber = s.WaybillNumber, SenderPhoneNumber = s.SenderPhoneNumber, ReceiverPhoneNumber = s.ReceiverPhoneNumber,
                    TotalWeight = s.Items.Sum(i => i.Weight), AmountPaid = s.TotalCost, Status = s.Status,
                    DateCreated = s.DateCreated, PaymentMethod = s.PaymentMethod
                }).ToListAsync();

            // Search Merchant Shipments
            var merchant = await _context.MerchantShipments.Include(s => s.Merchant).Include(s => s.Items)
                .Where(s => s.WaybillNumber == searchTerm || s.Merchant.BusinessPhoneNumber == searchTerm || s.ReceiverPhoneNumber == searchTerm)
                .Select(s => new TrackedShipmentResult {
                    WaybillNumber = s.WaybillNumber, SenderPhoneNumber = s.Merchant.BusinessPhoneNumber, ReceiverPhoneNumber = s.ReceiverPhoneNumber,
                    TotalWeight = s.Items.Sum(i => i.Weight), AmountPaid = s.TotalCost, Status = s.Status,
                    DateCreated = s.DateCreated, PaymentMethod = s.PaymentMethod
                }).ToListAsync();

            // Search Generic Shipments
            var generic = await _context.GenericShipments
                .Where(s => s.WaybillNumber == searchTerm || s.SenderPhoneNumber == searchTerm || s.ReceiverPhoneNumber == searchTerm)
                .Select(s => new TrackedShipmentResult {
                    WaybillNumber = s.WaybillNumber, SenderPhoneNumber = s.SenderPhoneNumber, ReceiverPhoneNumber = s.ReceiverPhoneNumber,
                    TotalWeight = 0, AmountPaid = s.TotalCost, Status = s.Status,
                    DateCreated = s.DateCreated, PaymentMethod = s.PaymentMethod
                }).ToListAsync();

            results.AddRange(regular);
            results.AddRange(merchant);
            results.AddRange(generic);

            var vm = new TrackShipmentVM
            {
                SearchTerm = searchTerm,
                Results = results.OrderByDescending(r => r.DateCreated).ToList()
            };

            return View("Index", vm);
        }
    }
}