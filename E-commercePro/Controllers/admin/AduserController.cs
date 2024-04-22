using E_commercePro.Data;
using E_commercePro.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_commercePro.Controllers.admin
{
    public class AduserController : Controller
    {

        private readonly ApplicationDbContext _db;

        public AduserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult UserList()
        {
            List<UserSignUp> objSignList = _db.Sign_Up.ToList();
            return View(objSignList);

        }

        [HttpPost]
        public IActionResult ToggleBlock(int id)
        {
            var obj = _db.Sign_Up.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            obj.IsBlocked = !obj.IsBlocked;
            _db.SaveChanges();

            return RedirectToAction(nameof(UserList));
        }


        //user creating section
        public IActionResult UserCreate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UserCreate(UserSignUp gn)
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
                _db.Sign_Up.Add(gn);
                _db.SaveChanges();
                TempData["success"] = "Create successfully";
              
                return RedirectToAction("UserList");
            }
            return View(gn);
        }

        //user creating section end


        //user edit section 
        public IActionResult Useredit(int? id)
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
        public IActionResult Useredit(UserSignUp gn)
        {
            if (ModelState.IsValid)
            {

                _db.Sign_Up.Update(gn);
                _db.SaveChanges();
                TempData["success"] = " Update successfully";
                return RedirectToAction("UserList", "Aduser");
            }
            return View();

        }

        //user edit section end
    }
}
