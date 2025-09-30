using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.IRepository;
using SMS.Models;
using SMS.Models.ViewModels;

namespace SMS.Controllers
{
    public class LocationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LocationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // A private helper to generate the hardcoded list for LocationType
        private List<SelectListItem> GetLocationTypes()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Station", Value = "Station" },
                new SelectListItem { Text = "Hub", Value = "Hub" }
            };
        }

        // GET: Index (Main page for both list and create form)
        public async Task<IActionResult> Index()
        {
            var locationList = await _unitOfWork.LocationRepository.GetAllAsync(includeProperties: "State");
            var stateList = await _unitOfWork.StateRepository.GetAllAsync();

            var viewModel = new LocationVM
            {
                LocationList = locationList,
                Location = new Location(), // For the create form
                StateList = stateList.Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                }),
                LocationTypeList = GetLocationTypes()
            };

            return View(viewModel);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LocationVM viewModel)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.LocationRepository.AddAsync(viewModel.Location);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Location created successfully!";
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, repopulate the ViewModel and return to the view
            TempData["error"] = "Failed to create location. Please check the form.";
            var locationList = await _unitOfWork.LocationRepository.GetAllAsync(includeProperties: "State");
            var stateList = await _unitOfWork.StateRepository.GetAllAsync();

            viewModel.LocationList = locationList;
            viewModel.StateList = stateList.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            });
            viewModel.LocationTypeList = GetLocationTypes();

            return View("Index", viewModel);
        }

        // POST: Edit (From Modal)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Location location)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.LocationRepository.UpdateAsync(location);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Location updated successfully!";
            }
            else
            {
                TempData["error"] = "Failed to update location.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Delete (From Modal)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var locationToDelete = await _unitOfWork.LocationRepository.GetByIdAsync(id);
            if (locationToDelete == null)
            {
                TempData["error"] = "Location not found.";
                return RedirectToAction(nameof(Index));
            }

            await _unitOfWork.LocationRepository.DeleteAsync(locationToDelete.Id);
            await _unitOfWork.CompleteAsync();
            TempData["success"] = "Location deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: GetLocation/5 (API endpoint for AJAX call)
        [HttpGet]
        public async Task<IActionResult> GetLocation(int id)
        {
            var location = await _unitOfWork.LocationRepository.GetByIdAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            return Json(location);
        }
    }
}