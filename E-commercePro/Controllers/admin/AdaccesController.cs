using E_commercePro.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using E_commercePro.Models;

namespace E_commercePro.Controllers.admin
{
    public class AdaccesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdaccesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Dashboard()
        {
           
            return View();
        }

        


        


        public async Task<IActionResult> LogOut()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Access");

            return View();
        }


        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            UserSignUp? signFromDb = _db.Sign_Up.Find(id);

            if (signFromDb == null)
            {

                return NotFound();
            }
            return View(signFromDb);
        }

        [HttpPost]
        public IActionResult Edit(UserSignUp gn)
        {
            if (ModelState.IsValid)
            {

                _db.Sign_Up.Update(gn);
                _db.SaveChanges();
                TempData["success"] = "Category Update successfully";
                return RedirectToAction("Dashboard", "Admin");
            }
            return View();

        }

       
      
    }
}
