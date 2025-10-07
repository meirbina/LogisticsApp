using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.IRepository;
using SMS.Models;
using System.Threading.Tasks;

namespace SMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GenericPackageController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public GenericPackageController(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

        public async Task<IActionResult> Index()
        {
            var packages = await _unitOfWork.GenericPackageRepository.GetAllAsync();
            return View(packages);
        }

        [HttpPost]
        public async Task<IActionResult> Create(GenericPackage package)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.GenericPackageRepository.AddAsync(package);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Generic Package created successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(GenericPackage package)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.GenericPackageRepository.UpdateAsync(package);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Generic Package updated successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var package = await _unitOfWork.GenericPackageRepository.GetByIdAsync(id);
            if (package != null)
            {
                // Or you can implement soft delete by setting IsActive = false
                await _unitOfWork.GenericPackageRepository.DeleteAsync(package.Id);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Generic Package deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetPackage(int id)
        {
            var package = await _unitOfWork.GenericPackageRepository.GetByIdAsync(id);
            return package == null ? NotFound() : Json(package);
        }
    }
}