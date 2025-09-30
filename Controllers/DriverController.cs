// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Rendering;
// using SMS.IRepository;
// using SMS.Models;
// using SMS.Models.ViewModels;
//
// namespace SMS.Controllers
// {
//     public class DriverController : Controller
//     {
//         private readonly IUnitOfWork _unitOfWork;
//
//         public DriverController(IUnitOfWork unitOfWork)
//         {
//             _unitOfWork = unitOfWork;
//         }
//
//         public async Task<IActionResult> Index()
//         {
//             // Fetching the list of events  
//             IEnumerable<Driver> DriverList = await _unitOfWork.DriverRepository.GetAllAsync(includeProperties: "Company");
//
//             return View(DriverList); // Return the list directly  
//         }
//
//         public async Task<IActionResult> Create()
//         {
//             //Fetching the list of Driver
//             IEnumerable<Company> companyList = await _unitOfWork.CompanyRepository.GetAllAsync();
//             DriverVM DriverVM = new()
//             {
//                 Driver = new(),
//                 CompanyList = companyList.Select(i => new SelectListItem
//                 {
//                     Text = i.Name,
//                     Value = i.Id.ToString()
//                 }),
//
//             };
//             return View(DriverVM);
//         }
//
//
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Create(DriverVM DriverVM)
//         {
//             if (ModelState.IsValid)
//             {
//                 await _unitOfWork.DriverRepository.AddAsync(DriverVM.Driver);
//                 await _unitOfWork.CompleteAsync(); // Save changes to the database              
//                 return RedirectToAction(nameof(Index));
//             }
//             return View(DriverVM);
//         }
//
//         public async Task<IActionResult> Edit(int id)
//         {
//             var DriverObj = await _unitOfWork.DriverRepository.GetByIdAsync(id);
//             if (DriverObj == null) return NotFound();
//
//             IEnumerable<Company> companyList = await _unitOfWork.CompanyRepository.GetAllAsync();
//
//             DriverVM DriverVM = new()
//             {
//                 CompanyList = companyList.Select(i => new SelectListItem
//                 {
//                     Text = i.Name,
//                     Value = i.Id.ToString()
//                 }),
//
//                 Driver = DriverObj
//             };
//
//             return View(DriverVM);
//         }
//
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Edit(DriverVM DriverVM)
//         {
//             if (ModelState.IsValid)
//             {
//                 await _unitOfWork.DriverRepository.UpdateAsync(DriverVM.Driver);
//                 await _unitOfWork.CompleteAsync();
//                 return RedirectToAction(nameof(Index));
//             }
//
//             // Re-fetch companyList in case of validation failure
//             DriverVM.CompanyList = (await _unitOfWork.CompanyRepository.GetAllAsync()).Select(i => new SelectListItem
//             {
//                 Text = i.Name,
//                 Value = i.Id.ToString()
//             });
//
//             return View(DriverVM);
//         }
//         public async Task<IActionResult> Details(int id)
//         {
//             var DriverObj = await _unitOfWork.DriverRepository.GetByIdAsync(id, includeProperties: "Company");
//             if (DriverObj == null) return NotFound();
//
//             return View(DriverObj);
//         }
//
//
//         public async Task<IActionResult> Delete(int id)
//         {
//             var DriverObj = await _unitOfWork.DriverRepository.GetByIdAsync(id, includeProperties: "Company");
//             if (DriverObj == null) return NotFound();
//
//             return View(DriverObj);
//         }
//
//         [HttpPost, ActionName("Delete")]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> DeleteConfirmed(int id)
//         {
//             var DriverObj = await _unitOfWork.DriverRepository.GetByIdAsync(id);
//             if (DriverObj == null) return NotFound();
//
//             await _unitOfWork.DriverRepository.DeleteAsync(DriverObj.Id);
//             await _unitOfWork.CompleteAsync();
//
//             return RedirectToAction(nameof(Index));
//         }
//     }
// }









using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.IRepository;
using SMS.Models;
using SMS.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Controllers
{
    public class DriverController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DriverController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Loads the main page with the form and the list
        public async Task<IActionResult> Index()
        {
            var viewModel = new DriverVM
            {
                DriverList = await _unitOfWork.DriverRepository.GetAllAsync(includeProperties: "Company"),
                CompanyList = (await _unitOfWork.CompanyRepository.GetAllAsync()).Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
                NewDriver = new Driver()
            };
            return View(viewModel);
        }

        // POST: Handles creating a new driver from the main page form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DriverVM viewModel)
        {
            // We only validate the NewDriver part of the view model
            if (ModelState.IsValid)
            {
                await _unitOfWork.DriverRepository.AddAsync(viewModel.NewDriver);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Driver created successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // If validation fails, we must reload the page with the necessary data
                TempData["error"] = "Save failed. Please check the form for errors.";
                viewModel.DriverList = await _unitOfWork.DriverRepository.GetAllAsync(includeProperties: "Company");
                viewModel.CompanyList = (await _unitOfWork.CompanyRepository.GetAllAsync()).Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
                return View("Index", viewModel);
            }
        }

        // POST: Handles editing a driver from the Edit Modal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Driver driver)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.DriverRepository.UpdateAsync(driver);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Driver updated successfully!";
            }
            else
            {
                TempData["error"] = "Update failed. Invalid data submitted.";
            }
            return RedirectToAction(nameof(Index));
        }
        
        // GET: API endpoint for fetching driver data for the edit modal
        [HttpGet]
        public async Task<IActionResult> GetDriver(int id)
        {
            var driver = await _unitOfWork.DriverRepository.GetByIdAsync(id);
            if (driver == null)
            {
                return NotFound();
            }
            // Return JSON data that JavaScript can easily use
            return Json(new {
                id = driver.Id,
                name = driver.Name,
                phoneNumber = driver.PhoneNumber,
                companyId = driver.CompanyId
            });
        }

        // POST: Handles deleting a driver from the Delete Modal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var driverToDelete = await _unitOfWork.DriverRepository.GetByIdAsync(id);
            if (driverToDelete == null)
            {
                TempData["error"] = "Driver not found.";
                return RedirectToAction(nameof(Index));
            }

            await _unitOfWork.DriverRepository.DeleteAsync(driverToDelete.Id);
            await _unitOfWork.CompleteAsync();
            TempData["success"] = "Driver deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}




