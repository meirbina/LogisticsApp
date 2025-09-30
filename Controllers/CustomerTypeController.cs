using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SMS.IRepository;
using SMS.Models;
using SMS.Models.ViewModels;

namespace SMS.Controllers
{
    public class CustomerTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Main page that displays the list and contains the modals
        public async Task<IActionResult> Index()
        {
            var viewModel = new CustomerTypeVM
            {
                CustomerTypeList = await _unitOfWork.CustomerTypeRepository.GetAllAsync(),
                NewCustomerType = new CustomerType()
            };
            return View(viewModel);
        }

        // POST: Create (Called from the "Add" modal)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerTypeVM viewModel)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.CustomerTypeRepository.AddAsync(viewModel.NewCustomerType);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Customer Type created successfully!";
            }
            else
            {
                TempData["error"] = "Failed to create customer type.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Edit (Called from the "Edit" modal)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerType customerType)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.CustomerTypeRepository.UpdateAsync(customerType);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Customer Type updated successfully!";
            }
            else
            {
                TempData["error"] = "Failed to update customer type.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Delete (Called from the "Delete" modal)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var typeToDelete = await _unitOfWork.CustomerTypeRepository.GetByIdAsync(id);
            if (typeToDelete == null)
            {
                TempData["error"] = "Customer Type not found.";
                return RedirectToAction(nameof(Index));
            }

            await _unitOfWork.CustomerTypeRepository.DeleteAsync(typeToDelete.Id);
            await _unitOfWork.CompleteAsync();
            TempData["success"] = "Customer Type deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: API endpoint for AJAX call to populate the edit modal
        [HttpGet]
        public async Task<IActionResult> GetCustomerType(int id)
        {
            var customerType = await _unitOfWork.CustomerTypeRepository.GetByIdAsync(id);
            if (customerType == null)
            {
                return NotFound();
            }
            return Json(customerType);
        }
    }
}