using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.IRepository;
using SMS.Models;
using System.Threading.Tasks;

namespace SMS.Controllers
{
    [Authorize(Roles = "Admin")] // Or your appropriate role
    public class VehicleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public VehicleController(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

        public async Task<IActionResult> Index()
        {
            var vehicles = await _unitOfWork.VehicleRepository.GetAllAsync();
            return View(vehicles);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.VehicleRepository.AddAsync(vehicle);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Vehicle registered successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
        // Add Edit/Delete actions if needed, following the same pattern.
    }
}