using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.IRepository;
using SMS.Models;
using System.Threading.Tasks;

namespace SMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InsuranceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public InsuranceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var insuranceList = await _unitOfWork.InsuranceRepository.GetAllAsync();
            return View(insuranceList);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Insurance insurance)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.InsuranceRepository.AddAsync(insurance);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Insurance added successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Insurance insurance)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.InsuranceRepository.UpdateAsync(insurance);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Insurance updated successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var insurance = await _unitOfWork.InsuranceRepository.GetByIdAsync(id);
            if (insurance != null)
            {
                await _unitOfWork.InsuranceRepository.DeleteAsync(insurance.Id);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Insurance deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetInsurance(int id)
        {
            var insurance = await _unitOfWork.InsuranceRepository.GetByIdAsync(id);
            return insurance == null ? NotFound() : Json(insurance);
        }
    }
}