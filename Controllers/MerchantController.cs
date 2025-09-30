using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS.DataContext;
using SMS.IRepository;
using SMS.Models;
using SMS.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MerchantController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;

        public MerchantController(IUnitOfWork unitOfWork, AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new MerchantVM
            {
                MerchantList = await _unitOfWork.MerchantRepository.GetAllAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MerchantVM vm)
        {
            if (!ModelState.IsValid || vm.Form.WeightPrices == null || !vm.Form.WeightPrices.Any())
            {
                TempData["error"] = "Please fill all merchant details and add at least one weight price range.";
                return RedirectToAction(nameof(Index));
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var merchant = new Merchant
                    {
                        BusinessName = vm.Form.BusinessName, OwnerFirstName = vm.Form.OwnerFirstName, OwnerLastName = vm.Form.OwnerLastName,
                        BusinessEmail = vm.Form.BusinessEmail, BusinessPhoneNumber = vm.Form.BusinessPhoneNumber, BusinessAddress = vm.Form.BusinessAddress
                    };
                    await _unitOfWork.MerchantRepository.AddAsync(merchant);
                    await _unitOfWork.CompleteAsync();

                    await _unitOfWork.WalletRepository.AddAsync(new Wallet { MerchantId = merchant.Id });
                    
                    foreach (var price in vm.Form.WeightPrices)
                    {
                        price.MerchantId = merchant.Id;
                        await _unitOfWork.WeightPriceRepository.AddAsync(price);
                    }
                    await _unitOfWork.CompleteAsync();

                    await transaction.CommitAsync();
                    TempData["success"] = "Merchant created successfully with wallet.";
                }
                catch (System.Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["error"] = $"Error creating merchant: {ex.Message}";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MerchantVM vm)
        {
            if (!ModelState.IsValid || vm.Form.WeightPrices == null || !vm.Form.WeightPrices.Any())
            {
                TempData["error"] = "Please fill all merchant details and add at least one weight price range.";
                return RedirectToAction(nameof(Index));
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var merchantInDb = await _context.Merchants.FindAsync(vm.Form.Id);
                    if (merchantInDb == null) return NotFound();

                    merchantInDb.BusinessName = vm.Form.BusinessName;
                    merchantInDb.OwnerFirstName = vm.Form.OwnerFirstName;
                    merchantInDb.OwnerLastName = vm.Form.OwnerLastName;
                    merchantInDb.BusinessEmail = vm.Form.BusinessEmail;
                    merchantInDb.BusinessPhoneNumber = vm.Form.BusinessPhoneNumber;
                    merchantInDb.BusinessAddress = vm.Form.BusinessAddress;

                    var existingPrices = await _context.WeightPrices.Where(p => p.MerchantId == vm.Form.Id).ToListAsync();
                    _context.WeightPrices.RemoveRange(existingPrices);

                    foreach (var price in vm.Form.WeightPrices)
                    {
                        price.MerchantId = vm.Form.Id;
                        _context.WeightPrices.Add(price);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["success"] = "Merchant updated successfully.";
                }
                catch (System.Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["error"] = $"Error updating merchant: {ex.Message}";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var merchant = await _context.Merchants.FindAsync(id);
            if (merchant != null)
            {
                _context.Merchants.Remove(merchant);
                await _context.SaveChangesAsync();
                TempData["success"] = "Merchant and all related data deleted successfully.";
            }
            else
            {
                TempData["error"] = "Merchant not found.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetMerchant(int id)
        {
            var merchant = await _context.Merchants
                .Include(m => m.WeightPrices)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (merchant == null) return NotFound();
            return Json(merchant);
        }
    }
}