using Microsoft.AspNetCore.Mvc;
using SMS.IRepository;
using SMS.Models;
using SMS.Models.ViewModels;

namespace SMS.Controllers;

public class CompanyController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: /Company/Index
        public async Task<IActionResult> Index()
        {
            var viewModel = new CompanyVM
            {
                CompanyList = await _unitOfWork.CompanyRepository.GetAllAsync()
            };
            return View(viewModel);
        }

        // POST: /Company/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyVM vm)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.CompanyRepository.AddAsync(vm.NewCompany);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Company created successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = "Save failed. Please check the form.";
            // If we fail, reload the page with the list populated
            vm.CompanyList = await _unitOfWork.CompanyRepository.GetAllAsync();
            return View("Index", vm);
        }

        // POST: /Company/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Company company)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.CompanyRepository.UpdateAsync(company);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Company updated successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /Company/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _unitOfWork.CompanyRepository.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            TempData["success"] = "Company deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        #region API Call
        // GET: /Company/GetCompany/5
        [HttpGet]
        public async Task<JsonResult> GetCompany(int id)
        {
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(id);
            return Json(company);
        }
        #endregion
    }
