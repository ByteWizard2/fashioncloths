using E_commercePro.Data;
using E_commercePro.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_commercePro.Controllers.user
{
    public class ResndOtpController : Controller
    {

        private readonly ApplicationDbContext _db;

        private readonly IEmailSender _emailSender;  // Inject the service for OTP generation and email sending

        public ResndOtpController(ApplicationDbContext db, IEmailSender emailSender)
        {
            _db = db;

            _emailSender = emailSender; // Initialize _emailSender with injected service

        }
        [HttpPost]
        public IActionResult ResendOTP(string email, UserSignUp gn)
        {
            // Generate a new OTP
            var otp = _emailSender.GenerateOTPAsync().Result;


            /// Include OTP in email message
            string emailSubject = "OTP Verification";
            string emailMessage = $"Your OTP is: {otp}. Please use this code to verify your account.";

            try
            {
                // Send email with OTP
                _emailSender.SendEmailAsync(gn.Email, emailSubject, emailMessage).Wait();

                // Save OTP details to database
                var otpDetails = new OtpDetails
                {
                    Email = gn.Email,
                    OTP = otp,
                    ExpiryDateTime = DateTime.UtcNow.AddMinutes(1) // Example: OTP expires in 1 minutes
                };
                _db.OtpDetails.Add(otpDetails);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log and handle any exceptions
                Console.WriteLine($"Error sending email: {ex.Message}");
                ModelState.AddModelError("", "Error sending email. Please try again later.");
                return View("SignUp", gn);
            }

            TempData["success"] = "New OTP sent successfully";
            return RedirectToAction("VerifyOTP", "OtpController1", new { email = gn.Email });

        }


    }
}
