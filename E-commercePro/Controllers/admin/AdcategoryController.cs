using E_commercePro.Data;
using E_commercePro.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_commercePro.Controllers.admin
{
    public class AdcategoryController : Controller
    {

        private readonly ApplicationDbContext _db;

        public AdcategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Categorylist()
        {
            List<category> objCategoryList = _db.Categories.ToList();
            return View(objCategoryList);
        }

        [HttpPost]
        public IActionResult ToggleList(int Id)
        {
            var obj = _db.Categories.Find(Id);
            if (obj == null)
            {
                return NotFound();
            }

            obj.IsListed = !obj.IsListed;
            _db.SaveChanges();

            TempData["success"] = obj.IsListed ? "Category listed successfully" : "Category unlisted successfully";
            return RedirectToAction("Categorylist", "Adcategory");
        }

        [HttpPost]
        public IActionResult CreateCategory(category ct)
        {
            if (ModelState.IsValid)
            {
                // Check if category name already exists
                var existingCategory = _db.Categories.FirstOrDefault(u => u.Name == ct.Name);

                if (existingCategory != null)
                {
                    ModelState.AddModelError("Name", "Category Name is already taken.");
                    return View("Categorylist", _db.Categories.ToList()); // Pass existing categories to the view
                }

                _db.Categories.Add(ct);
                _db.SaveChanges();
                TempData["success"] = "Create successfully";

                return RedirectToAction("Categorylist");
            }
            return View("Categorylist", _db.Categories.ToList()); // Pass existing categories to the view
        }



        public IActionResult Edit(int? id)
        {

            if (id == null || id == 0)
            {
                return NotFound();
            }

            category? categoryFromDb = _db.Categories.Find(id);

            if (categoryFromDb == null)
            {

                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(category ct) 
        {
            if (ModelState.IsValid)
            {

                _db.Categories.Update(ct);
                _db.SaveChanges();
                TempData["success"] = "Category Update successfully";
                return RedirectToAction("Categorylist", "Adcategory");
            }
            return View();
        }


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            category? categoryFromDb = _db.Categories.Find(id);

            if (categoryFromDb == null)
            {

                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult DeletePost(int? id)
        {
            category? categoryFromDb = _db.Categories.Find(id);
            if (categoryFromDb == null)
            {

                return NotFound();
            }

            _db.Categories.Remove(categoryFromDb);
            _db.SaveChanges();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Categorylist", "Adcategory");



        }
    }
}
