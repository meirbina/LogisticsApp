using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SMS.IRepository;
using SMS.Models;
using SMS.Models.ViewModels;

namespace SMS.Controllers
{
    public class PackagingPriceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PackagingPriceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Main page
        public async Task<IActionResult> Index()
        {
            var viewModel = new PackagingPriceVM
            {
                // Only get records that are active
                PackagingPriceList = await _unitOfWork.PackagingPriceRepository.GetAllAsync(filter: p => p.IsActive),
                NewPackagingPrice = new PackagingPrice()
            };
            return View(viewModel);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PackagingPriceVM viewModel)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.PackagingPriceRepository.AddAsync(viewModel.NewPackagingPrice);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Packaging Price created successfully!";
            }
            else
            {
                TempData["error"] = "Failed to create Packaging Price.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PackagingPrice packagingPrice)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.PackagingPriceRepository.UpdateAsync(packagingPrice);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Packaging Price updated successfully!";
            }
            else
            {
                TempData["error"] = "Failed to update Packaging Price.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Delete (Soft Delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var itemToDelete = await _unitOfWork.PackagingPriceRepository.GetByIdAsync(id);
            if (itemToDelete == null)
            {
                TempData["error"] = "Packaging Price not found.";
                return RedirectToAction(nameof(Index));
            }

            // This is a SOFT delete
            itemToDelete.IsActive = false;
            await _unitOfWork.PackagingPriceRepository.UpdateAsync(itemToDelete);
            await _unitOfWork.CompleteAsync();
            TempData["success"] = "Packaging Price deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: API endpoint for AJAX call
        [HttpGet]
        public async Task<IActionResult> GetPackagingPrice(int id)
        {
            var packagingPrice = await _unitOfWork.PackagingPriceRepository.GetByIdAsync(id);
            if (packagingPrice == null)
            {
                return NotFound();
            }
            return Json(packagingPrice);
        }
    }
}