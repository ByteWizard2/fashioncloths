using E_commercePro.Data;
using E_commercePro.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_commercePro.Controllers.user
{
    public class OtpController1 : Controller

    {
        private readonly ApplicationDbContext _db;



        public OtpController1(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public IActionResult VerifyOTP(string email)
        {
            ViewBag.Email = email; // Pass the email to the view
            return View();
        }

        [HttpPost]
        public IActionResult VerifyOTP(string email, string otp1, string otp2, string otp3, string otp4, string otp5, string otp6)
        {

            string otp = otp1 + otp2 + otp3 + otp4 + otp5 + otp6;
            // Retrieve OTP details from the database for the given email
            var otpDetails = _db.OtpDetails.FirstOrDefault(o => o.Email == email && o.OTP == otp && o.ExpiryDateTime > DateTime.UtcNow);

            if (otpDetails != null)
            {
                // OTP is valid, remove it from the database
                _db.OtpDetails.Remove(otpDetails);
                _db.SaveChanges();
                TempData["success"] = "OTP verified successfully";
                return RedirectToAction("Login", "User"); // Redirect to the login page
            }
            else
            {
                // Invalid OTP, display error message
                ViewBag.ErrorMessage = "Invalid OTP. Please try again.";
                return View();
            }
        }

    }
}
