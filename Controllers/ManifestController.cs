using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.DataContext;
using SMS.Models;
using SMS.Models.ViewModels;

namespace SMS.Controllers
{
    [Authorize]
    public class ManifestController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ManifestController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Create()
        {
            var vm = new ManifestVM
            {
                LocationList = await GetFormattedLocationList(),
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FilterShipments(ManifestVM vm)
        {
            vm.LocationList = await GetFormattedLocationList();
            vm.AvailableShipments = await GetAvailableShipments(vm.DepartureLocationId, vm.DestinationLocationId, vm.StartDate, vm.EndDate);
            return View("Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateManifest(ManifestVM vm)
        {
            var user = await _userManager.GetUserAsync(User);
            var departureLoc = await _context.Locations.FindAsync(vm.DepartureLocationId);
            var destinationLoc = await _context.Locations.FindAsync(vm.DestinationLocationId);
            string manifestCode = $"MAN-{departureLoc.LocationCode.ToUpper()}-{destinationLoc.LocationCode.ToUpper()}-{DateTime.Now:yyyyMMddHHmmss}";

            var newManifest = new Manifest
            {
                ManifestId = manifestCode,
                DepartureLocationId = vm.DepartureLocationId,
                DestinationLocationId = vm.DestinationLocationId,
                DateCreated = DateTime.Now,
                CreatedBy = user.Email
            };
            _context.Manifests.Add(newManifest);
            await _context.SaveChangesAsync();

            vm.GeneratedManifestId = newManifest.ManifestId;
            vm.ManifestDbId = newManifest.Id;
            vm.LocationList = await GetFormattedLocationList();
            vm.AvailableShipments = await GetAvailableShipments(vm.DepartureLocationId, vm.DestinationLocationId, vm.StartDate, vm.EndDate);
            vm.ManifestedShipments = new List<ShipmentDisplayViewModel>();

            return View("Create", vm);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var manifest = await _context.Manifests.FindAsync(id);
            if (manifest == null || manifest.IsSignedOff)
            {
                TempData["error"] = "Manifest cannot be edited because it is not found or has already been dispatched.";
                return RedirectToAction("ManifestList");
            }

            var vm = new ManifestVM
            {
                ManifestDbId = manifest.Id, GeneratedManifestId = manifest.ManifestId,
                DepartureLocationId = manifest.DepartureLocationId, DestinationLocationId = manifest.DestinationLocationId,
                LocationList = await GetFormattedLocationList(), StartDate = DateTime.Today.AddDays(-7), EndDate = DateTime.Today,
                AvailableShipments = await GetAvailableShipments(manifest.DepartureLocationId, manifest.DestinationLocationId, DateTime.Today.AddDays(-7), DateTime.Today),
                ManifestedShipments = await GetManifestedShipments(manifest.Id)
            };
            return View(vm);
        }
        
        // public async Task<IActionResult> ManifestList()
        // {
        //     // --- FIX: Query the Driver model directly ---
        //     ViewBag.Drivers = new SelectList(await _context.Drivers.ToListAsync(), "Id", "Name");
        //     ViewBag.Vehicles = new SelectList(await _context.Vehicles.Where(v => v.IsActive).ToListAsync(), "Id", "PlateNumber");
        //     // Pass the formatted location list for the editable departure dropdown
        //     ViewBag.Locations = await GetFormattedLocationList();
        //
        //     var manifests = await _context.Manifests
        //                             .Include(m => m.DepartureLocation.State)
        //                             .Include(m => m.DestinationLocation.State)
        //                             .OrderByDescending(m => m.DateCreated).ToListAsync();
        //     return View(manifests);
        // }
        
        // Replace the existing ManifestList action with this one.

        public async Task<IActionResult> ManifestList(DateTime? startDate, DateTime? endDate)
        {
            // Default to today's date if none are provided
            var start = startDate ?? DateTime.Today;
            var end = endDate ?? DateTime.Today.AddDays(1).AddTicks(-1);

            var driverUsers = await _userManager.GetUsersInRoleAsync("Driver");
            var driverIds = driverUsers.Select(u => u.Id);
            ViewBag.Drivers = new SelectList(await _context.Employees.Where(e => driverIds.Contains(e.ApplicationUserId)).ToListAsync(), "Id", "Name");
            ViewBag.Vehicles = new SelectList(await _context.Vehicles.Where(v => v.IsActive).ToListAsync(), "Id", "PlateNumber");
            ViewBag.Locations = await GetFormattedLocationList();

            var manifests = await _context.Manifests
                .Include(m => m.DepartureLocation.State)
                .Include(m => m.DestinationLocation.State)
                .Where(m => m.DateCreated >= start && m.DateCreated <= end) // Filter by date
                .OrderByDescending(m => m.DateCreated)
                .ToListAsync();
    
            var vm = new ManifestListVM
            {
                Manifests = manifests,
                StartDate = start,
                EndDate = end.Date
            };

            return View(vm);
        }
        
        public async Task<IActionResult> Details(int id)
        {
            var manifest = await _context.Manifests
                .Include(m => m.DepartureLocation.State)
                .Include(m => m.DestinationLocation.State)
                .Include(m => m.Driver)
                .Include(m => m.Vehicle)
                .Include(m => m.ManifestShipments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manifest == null) return NotFound();
            
            return View(manifest);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOff(int manifestDbId, int vehicleId, int driverId, decimal dispatchFee, int departureLocationId)
        {
            var manifest = await _context.Manifests.FindAsync(manifestDbId);
            if (manifest != null && !manifest.IsSignedOff)
            {
                var user = await _userManager.GetUserAsync(User);
                manifest.VehicleId = vehicleId;
                manifest.DriverId = driverId;
                manifest.DispatchFee = dispatchFee;
                // --- FIX: Save the potentially changed departure location ---
                manifest.DepartureLocationId = departureLocationId;
                manifest.DispatchedBy = user.Email;
                manifest.DispatchedDate = DateTime.Now;
                manifest.IsSignedOff = true;
                await _context.SaveChangesAsync();
                TempData["success"] = $"Manifest {manifest.ManifestId} has been signed off and dispatched.";
            }
            else
            {
                TempData["error"] = "Manifest could not be signed off.";
            }
            return RedirectToAction("ManifestList");
        }
        
        #region API Actions and Helpers
        [HttpPost]
        public async Task<JsonResult> AddToManifest(int manifestDbId, string waybillNumber)
        {
            var manifest = await _context.Manifests.FindAsync(manifestDbId);
            if (manifest == null) return Json(new { success = false, message = "Manifest not found." });
            if (manifest.IsSignedOff) return Json(new { success = false, message = "Cannot add to a manifest that is already dispatched." });

            var existing = await _context.ManifestShipments.FirstOrDefaultAsync(ms => ms.WaybillNumber == waybillNumber);
            if (existing != null) return Json(new { success = false, message = "This shipment is already on a manifest." });

            _context.ManifestShipments.Add(new ManifestShipment { ManifestId = manifestDbId, WaybillNumber = waybillNumber });
            
            var regularShipment = await _context.Shipments.FirstOrDefaultAsync(s => s.WaybillNumber == waybillNumber);
            if(regularShipment != null) regularShipment.Status = "In Transit";
            
            var merchantShipment = await _context.MerchantShipments.FirstOrDefaultAsync(s => s.WaybillNumber == waybillNumber);
            if(merchantShipment != null) merchantShipment.Status = "In Transit";

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<JsonResult> RemoveFromManifest(int manifestDbId, string waybillNumber)
        {
            var manifest = await _context.Manifests.FindAsync(manifestDbId);
            if (manifest != null && manifest.IsSignedOff) return Json(new { success = false, message = "Cannot remove from a manifest that is already dispatched." });

            var manifestShipment = await _context.ManifestShipments
                .FirstOrDefaultAsync(ms => ms.ManifestId == manifestDbId && ms.WaybillNumber == waybillNumber);
            
            if (manifestShipment != null)
            {
                _context.ManifestShipments.Remove(manifestShipment);

                var regularShipment = await _context.Shipments.FirstOrDefaultAsync(s => s.WaybillNumber == waybillNumber);
                if(regularShipment != null) regularShipment.Status = "Pending";
            
                var merchantShipment = await _context.MerchantShipments.FirstOrDefaultAsync(s => s.WaybillNumber == waybillNumber);
                if(merchantShipment != null) merchantShipment.Status = "Pending";

                await _context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }
        
        private async Task<IEnumerable<SelectListItem>> GetFormattedLocationList()
        {
            var locations = await _context.Locations
                .Include(l => l.State)
                .OrderBy(l => l.State.Name).ThenBy(l => l.Name)
                .ToListAsync();
            return locations.Select(l => new SelectListItem
            {
                Text = $"{l.State.Name} ==> {l.Name}",
                Value = l.Id.ToString()
            });
        }

        private async Task<List<ShipmentDisplayViewModel>> GetAvailableShipments(int departureId, int destinationId, DateTime start, DateTime end)
        {
            var endOfDay = end.AddDays(1).AddTicks(-1);
            var available = new List<ShipmentDisplayViewModel>();
            var employeesInDeparture = await _context.Employees
                .Where(e => e.LocationId == departureId)
                .Select(e => e.ApplicationUser.Email)
                .ToListAsync();

            var regularShipments = await _context.Shipments
                .Include(s => s.DestinationLocation)
                .Where(s => employeesInDeparture.Contains(s.CreatedBy) && s.DestinationLocationId == destinationId && s.Status == "Pending" && s.DateCreated >= start && s.DateCreated <= endOfDay)
                .Select(s => new ShipmentDisplayViewModel { WaybillNumber = s.WaybillNumber, DateCreated = s.DateCreated, Destination = s.DestinationLocation.Name })
                .ToListAsync();
            
            var merchantShipments = await _context.MerchantShipments
                .Include(s => s.DestinationLocation)
                .Where(s => employeesInDeparture.Contains(s.CreatedBy) && s.DestinationLocationId == destinationId && s.Status == "Pending" && s.DateCreated >= start && s.DateCreated <= endOfDay)
                .Select(s => new ShipmentDisplayViewModel { WaybillNumber = s.WaybillNumber, DateCreated = s.DateCreated, Destination = s.DestinationLocation.Name })
                .ToListAsync();

            available.AddRange(regularShipments);
            available.AddRange(merchantShipments);
            return available.OrderBy(s => s.DateCreated).ToList();
        }

        private async Task<List<ShipmentDisplayViewModel>> GetManifestedShipments(int manifestDbId)
        {
            var waybillsOnManifest = await _context.ManifestShipments
                .Where(ms => ms.ManifestId == manifestDbId)
                .Select(ms => ms.WaybillNumber)
                .ToListAsync();
            
            var manifested = new List<ShipmentDisplayViewModel>();
            var regular = await _context.Shipments.Include(s => s.DestinationLocation).Where(s => waybillsOnManifest.Contains(s.WaybillNumber))
                .Select(s => new ShipmentDisplayViewModel { WaybillNumber = s.WaybillNumber, Destination = s.DestinationLocation.Name }).ToListAsync();
            
            var merchant = await _context.MerchantShipments.Include(s => s.DestinationLocation).Where(s => waybillsOnManifest.Contains(s.WaybillNumber))
                .Select(s => new ShipmentDisplayViewModel { WaybillNumber = s.WaybillNumber, Destination = s.DestinationLocation.Name }).ToListAsync();
            
            manifested.AddRange(regular);
            manifested.AddRange(merchant);
            return manifested;
        }
        #endregion
    }
}