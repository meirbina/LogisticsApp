using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SMS.IRepository;
using SMS.Models;
using SMS.Models.ViewModels;

namespace SMS.Controllers
{
    public class ServiceTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Main page
        public async Task<IActionResult> Index()
        {
            var viewModel = new ServiceTypeVM
            {
                // Only get records that are not soft-deleted
                ServiceTypeList = await _unitOfWork.ServiceTypeRepository.GetAllAsync(filter: s => !s.IsDeleted),
                NewServiceType = new ServiceType()
            };
            return View(viewModel);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceTypeVM viewModel)
        {
            if (ModelState.IsValid)
            {
                // Set the creation date automatically
                viewModel.NewServiceType.CreatedDate = DateTime.Now;
                await _unitOfWork.ServiceTypeRepository.AddAsync(viewModel.NewServiceType);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Service Type created successfully!";
            }
            else
            {
                TempData["error"] = "Failed to create Service Type.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceType serviceType)
        {
            // if (ModelState.IsValid)
            // {
            //     // Retrieve the original created date so it doesn't get overwritten
            //     var originalServiceType = await _unitOfWork.ServiceTypeRepository.GetByIdAsync(serviceType.Id);
            //     if (originalServiceType != null)
            //     {
            //         serviceType.CreatedDate = originalServiceType.CreatedDate; // Preserve original creation date
            //         await _unitOfWork.ServiceTypeRepository.UpdateAsync(serviceType);
            //         await _unitOfWork.CompleteAsync();
            //         TempData["success"] = "Service Type updated successfully!";
            //     }
            //     else
            //     {
            //          TempData["error"] = "Service Type not found.";
            //     }
            // }
            // else
            // {
            //     TempData["error"] = "Failed to update Service Type.";
            // }
            // return RedirectToAction(nameof(Index));
            if (ModelState.IsValid)
            {
                await _unitOfWork.ServiceTypeRepository.UpdateAsync(serviceType);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Service Type updated successfully!";
            }
            else
            {
                TempData["error"] = "Failed to update service type.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Delete (Soft Delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var typeToDelete = await _unitOfWork.ServiceTypeRepository.GetByIdAsync(id);
            if (typeToDelete == null)
            {
                TempData["error"] = "Service Type not found.";
                return RedirectToAction(nameof(Index));
            }

            // This is a SOFT delete
            typeToDelete.IsDeleted = true;
            await _unitOfWork.ServiceTypeRepository.UpdateAsync(typeToDelete);
            await _unitOfWork.CompleteAsync();
            TempData["success"] = "Service Type deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: API endpoint for AJAX call
        [HttpGet]
        public async Task<IActionResult> GetServiceType(int id)
        {
            var serviceType = await _unitOfWork.ServiceTypeRepository.GetByIdAsync(id);
            if (serviceType == null)
            {
                return NotFound();
            }
            return Json(serviceType);
        }
    }
}