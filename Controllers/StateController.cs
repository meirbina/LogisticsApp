// using Microsoft.AspNetCore.Mvc;
// using SMS.IRepository;
// using SMS.Models;
// using SMS.Models.ViewModels;
// using System.Threading.Tasks;
//
// namespace SMS.Controllers // Or your appropriate namespace
// {
//     public class StateController : Controller
//     {
//         private readonly IUnitOfWork _unitOfWork;
//
//         public StateController(IUnitOfWork unitOfWork)
//         {
//             _unitOfWork = unitOfWork;
//         }
//
//         // GET: Loads the main page
//         public async Task<IActionResult> Index()
//         {
//             var viewModel = new StateVM
//             {
//                 StateList = await _unitOfWork.StateRepository.GetAllAsync(),
//                 NewState = new State()
//             };
//             return View(viewModel);
//         }
//
//         // POST: Creates a new state
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Create(StateVM viewModel)
//         {
//             if (ModelState.IsValid)
//             {
//                 // The 'InState' checkbox is not on the create form, so it defaults to false.
//                 // It can be enabled in the Edit modal.
//                 await _unitOfWork.StateRepository.AddAsync(viewModel.NewState);
//                 await _unitOfWork.CompleteAsync();
//                 TempData["success"] = "State created successfully!";
//                 return RedirectToAction(nameof(Index));
//             }
//
//             TempData["error"] = "Failed to create state. Please check the form.";
//             // If creation fails, reload the list and return to the view
//             viewModel.StateList = await _unitOfWork.StateRepository.GetAllAsync();
//             return View("Index", viewModel);
//         }
//
//         // POST: Edits an existing state from the modal
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Edit(State state)
//         {
//             if (ModelState.IsValid)
//             {
//                 await _unitOfWork.StateRepository.UpdateAsync(state);
//                 await _unitOfWork.CompleteAsync();
//                 TempData["success"] = "State updated successfully!";
//             }
//             else
//             {
//                 TempData["error"] = "Failed to update state.";
//             }
//             return RedirectToAction(nameof(Index));
//         }
//
//         // POST: Deletes a state from the modal
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Delete(int id)
//         {
//             var stateToDelete = await _unitOfWork.StateRepository.GetByIdAsync(id);
//             if (stateToDelete == null)
//             {
//                 TempData["error"] = "State not found.";
//                 return RedirectToAction(nameof(Index));
//             }
//             
//             await _unitOfWork.StateRepository.DeleteAsync(stateToDelete.Id);
//             await _unitOfWork.CompleteAsync();
//             TempData["success"] = "State deleted successfully.";
//             return RedirectToAction(nameof(Index));
//         }
//
//         // GET: API endpoint to fetch data for the edit modal
//         [HttpGet]
//         public async Task<IActionResult> GetState(int id)
//         {
//             var state = await _unitOfWork.StateRepository.GetByIdAsync(id);
//             if (state == null)
//             {
//                 return NotFound();
//             }
//             return Json(state);
//         }
//     }
// }












using Microsoft.AspNetCore.Mvc;
using SMS.IRepository;
using SMS.Models;
using SMS.Models.ViewModels;
using System.Threading.Tasks;

namespace SMS.Controllers
{
    public class StateController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StateController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new StateVM
            {
                StateList = await _unitOfWork.StateRepository.GetAllAsync(),
                NewState = new State()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StateVM viewModel)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.StateRepository.AddAsync(viewModel.NewState);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "State created successfully!";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Failed to create state.";
            viewModel.StateList = await _unitOfWork.StateRepository.GetAllAsync();
            return View("Index", viewModel);
        }

        // ===================================================================
        // THE FIX IS HERE: A More Robust Edit Action
        // ===================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(State stateFromForm) // Renamed for clarity
        {
            // First, validate the incoming data from the form.
            if (ModelState.IsValid)
            {
                // Instead of trusting the incoming object directly, fetch the ORIGINAL state
                // from the database. This object is being tracked by Entity Framework.
                var stateFromDb = await _unitOfWork.StateRepository.GetByIdAsync(stateFromForm.Id);

                if (stateFromDb == null)
                {
                    TempData["error"] = "State not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Now, manually update the properties of the database object
                // with the values from the form object.
                stateFromDb.Name = stateFromForm.Name;
                stateFromDb.InState = stateFromForm.InState; // This is the crucial line

                // Because you modified the stateFromDb object that EF is tracking,
                // calling SaveChanges (or CompleteAsync) will now correctly
                // generate the SQL to update BOTH Name and InState.
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "State updated successfully!";
            }
            else
            {
                TempData["error"] = "Failed to update state. Invalid data provided.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var stateToDelete = await _unitOfWork.StateRepository.GetByIdAsync(id);
            if (stateToDelete == null)
            {
                TempData["error"] = "State not found.";
                return RedirectToAction(nameof(Index));
            }
            await _unitOfWork.StateRepository.DeleteAsync(stateToDelete.Id);
            await _unitOfWork.CompleteAsync();
            TempData["success"] = "State deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetState(int id)
        {
            var state = await _unitOfWork.StateRepository.GetByIdAsync(id);
            if (state == null)
            {
                return NotFound();
            }
            return Json(state);
        }
    }
}