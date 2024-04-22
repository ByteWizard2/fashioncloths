using E_commercePro.Data;
using E_commercePro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commercePro.Controllers.admin
{
    public class AdCouponsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdCouponsController(ApplicationDbContext db)
        {
            _db = db;

        }
        public IActionResult CouponList()
        {
         var coupons = _db.Coupons.ToList();
            return View(coupons);
        }


        public IActionResult AddCoupon()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCoupon(CouponsList cp )
        {
            if (ModelState.IsValid)
            {
                _db.Coupons.Add(cp);
                _db.SaveChanges();

                TempData["SuccessMessage"] = "Coupon added successfully";
                return RedirectToAction("CouponList");

            }
           return View(cp);
        }

        public IActionResult EdCoupon(int Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var coupon =  _db.Coupons.Find(Id);

            if (coupon == null)
            {
                return NotFound();
            }

            ViewData["Description"] = coupon.Description;

            return View(coupon);

            
        }
        [HttpPost]
        public IActionResult EdCoupon(CouponsList cp)
        {
            if (ModelState.IsValid)
            {
                _db.Coupons.Update(cp);
                _db.SaveChanges();

                TempData["SuccessMessage"] = "Coupon Edit successfully";
                return RedirectToAction("CouponList");

            }
            return View(cp);
        }


        [HttpPost]
        public IActionResult DelCoupon(int id)
        {
            var delcoupn = _db.Coupons.Find(id);

            if (delcoupn == null)
            {
                return NotFound();
            }

            _db.Coupons.Remove(delcoupn);
            _db.SaveChanges();

            return Json(new { success = true });
        }
    }
}
