using E_commercePro.Data;
using E_commercePro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_commercePro.Controllers.admin
{
    public class AdOfferController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly IWebHostEnvironment _environment;

        public AdOfferController(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment= environment;
        }
      
        public IActionResult OfferList()
        {

            var offerList = _db.Offers
                         .Include(p => p.category)
                         .ToList();

            return View(offerList);
        }

        public IActionResult AddOffer()
        {
            var listedCategories = _db.Categories.Where(c => !c.IsListed).ToList();


            ViewBag.CategoryList = new SelectList(listedCategories, "Id", "Name");
            return View();
            
        }

        [HttpPost]
        public async Task<IActionResult> AddOffer(Offer offermodel, IFormFile images)
        {
            if (ModelState.IsValid)
            {
                var imagePath1 = await SaveImage(images);

                offermodel.banner = imagePath1;

                _db.Offers.Add(offermodel);
                await _db.SaveChangesAsync();

                return RedirectToAction("OfferList");
            }
            return View(offermodel);
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
                var filePath = Path.Combine(uploadsDir, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                return "/uploads/" + uniqueFileName;
            }

            return null;
        }

        [HttpPost]
        public IActionResult DeleteOffer(int id)
        {
            var offerceToDelete = _db.Offers.Find(id);

            if (offerceToDelete == null)
            {
                return NotFound();
            }

            _db.Offers.Remove(offerceToDelete);
            _db.SaveChanges();

            return Json(new { success = true });
        }

        public IActionResult OfferEdit(int? id)
        {

            if (id == null || id == 0)
            {
                return NotFound();
            }


            Offer? offerFromDb = _db.Offers.Find(id);

            if (offerFromDb == null)
            {

                return NotFound();
            }

            ViewBag.CategoryList = new SelectList(_db.Categories, "Id", "Name");
            return View(offerFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> OfferEdit(Offer offermodel, IFormFile images1)
        {
            if (ModelState.IsValid)
            {
               
                var imagePath1 = await SaveImage(images1);


                offermodel.banner = imagePath1;


                _db.Offers.Update(offermodel);
                _db.SaveChanges();
                TempData["success"] = "Product Update successfully";
                return RedirectToAction("OfferList");
            }
            return View();
        }



    }
}
