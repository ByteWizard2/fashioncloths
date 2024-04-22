using E_commercePro.Data;
using E_commercePro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_commercePro.Controllers.user
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly IEmailSender _emailSender;  // Inject the service for OTP generation and email sending

        public UserController(ApplicationDbContext db, IEmailSender emailSender)
        {
            _db = db;

            _emailSender = emailSender; // Initialize _emailSender with injected service

        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(UserSignUp gn)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingEmail = _db.Sign_Up.FirstOrDefault(u => u.Email == gn.Email);

                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "Email is already taken.");
                    return View("SignUp", gn);
                }

                // Generate OTP
                var otp = _emailSender.GenerateOTPAsync().Result; // Generate OTP synchronously for simplicity


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
                    ModelState.AddModelError("ee", "Error sending email. Please try again later.");
                    return View("SignUp", gn);
                }

                _db.Sign_Up.Add(gn);
                _db.SaveChanges();
                TempData["success"] = "Register successfully";




                // Redirect to OTP verification page
                return RedirectToAction("VerifyOTP", "OtpController1", new { email = gn.Email });




            }
            return View(gn);
        }

        public IActionResult Login()
        {
            //if (HttpContext.Session.GetString("UserSession") != null)
            //{
            //    return RedirectToAction("UserHome", "Home");
            //}

            return View();
        }

        [HttpPost]
        public IActionResult Login(UserLogin lg)
        {
          

            var myUser = _db.Sign_Up.FirstOrDefault(x => x.Email == lg.Email && x.Password == lg.Password);
            if (myUser != null)
            {
                if (myUser.IsBlocked)
                {
                    ViewBag.Message = "Your account has been blocked. Please contact the administrator.";
                    return View();
                }
                // Convert the user ID to a string
                string userIdString = myUser.ID.ToString();

                // Store the logged-in user's ID in the session
                HttpContext.Session.SetString("UserId", userIdString);

                // Store user email in session or TempData for later use
                HttpContext.Session.SetString("UserEmail", myUser.Email);

                TempData["success"] = "Login successfully";
                return RedirectToAction("MainHome", "Home");


            }
            else
            {
                ViewBag.Message = "Login Failed...";
            }
            return View();
        }
    }
}



