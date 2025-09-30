using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.IRepository;
using SMS.Models;
using System.Threading.Tasks;

namespace SMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CouponController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CouponController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var coupons = await _unitOfWork.CouponRepository.GetAllAsync();
            return View(coupons);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.CouponRepository.AddAsync(coupon);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Coupon created successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.CouponRepository.UpdateAsync(coupon);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Coupon updated successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(id);
            if (coupon != null)
            {
                await _unitOfWork.CouponRepository.DeleteAsync(coupon.Id);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Coupon deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetCoupon(int id)
        {
            var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(id);
            return coupon == null ? NotFound() : Json(coupon);
        }
    }
}