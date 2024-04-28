using E_commercePro.Data;
using E_commercePro.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

public class AdProductController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _environment;

    public AdProductController(ApplicationDbContext db, IWebHostEnvironment environment)
    {
        _db = db;
        _environment = environment;
    }
    //refer viewmodal

    public IActionResult Productlist(int? pageNumber)
    {
        int pageSize = 10;
        pageNumber ??= 1;

        // Filter products to exclude those belonging to listed categories
        var productList = _db.Products
            .Where(p => !p.category.IsListed)
            .Include(p => p.category)
            .ToList();

        var totalProducts = productList.Count();
        var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

        var paginatedProducts = productList.Skip((pageNumber.Value - 1) * pageSize).Take(pageSize).ToList();

        ViewData["PageNumber"] = pageNumber;
        ViewData["TotalPages"] = totalPages;

        return View(paginatedProducts);
    }



    //create product
    public IActionResult CreateProduct()
    {
        var listedCategories = _db.Categories.Where(c => !c.IsListed).ToList();


        ViewBag.CategoryList = new SelectList(listedCategories, "Id", "Name");

        return View();
    }



    [HttpPost]
    public async Task<IActionResult> CreateProduct(Product productModel, IFormFile image1, IFormFile image2, IFormFile image3, IFormFile image4)
    {
        if (ModelState.IsValid)
        {
            // Handle image uploads
            var imagePath1 = await SaveImage(image1);
            var imagePath2 = await SaveImage(image2);
            var imagePath3 = await SaveImage(image3);
            var imagePath4 = await SaveImage(image4);

            // Save image paths to the product model
            productModel.ImageUrls = new List<string> { imagePath1, imagePath2, imagePath3, imagePath4 };

            // Save the product to the database
            _db.Products.Add(productModel);
            await _db.SaveChangesAsync();

            TempData["success"] = "Product created successfully";

            return RedirectToAction("ProductList");
        }

        var listedCategories = _db.Categories.Where(c => !c.IsListed).ToList();
        ViewBag.CategoryList = new SelectList(listedCategories, "Id", "Name");

        return View(productModel);
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



    public IActionResult Edit(int? id)
    {

        if (id == null || id == 0)
        {
            return NotFound();
        }


        Product? productFromDb = _db.Products.Find(id);

        if (productFromDb == null)
        {

            return NotFound();
        }

        ViewBag.CategoryList = new SelectList(_db.Categories, "Id", "Name");
        return View(productFromDb);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Product productModel, string[] existingImageUrls)
    {
        if (ModelState.IsValid)
        {
            // Update existing image paths
            productModel.ImageUrls = existingImageUrls.ToList();

            _db.Products.Update(productModel);
            _db.SaveChanges();

            TempData["success"] = "Product Update successfully";

            return RedirectToAction("Productlist");
        }

        ViewBag.CategoryList = new SelectList(_db.Categories, "Id", "Name");
        return View(productModel);
    }





    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        Product? productFromDb = _db.Products.Find(id);

        if (productFromDb == null)
        {

            return NotFound();
        }

        ViewBag.CategoryList = new SelectList(_db.Categories, "Id", "Name");
        return View(productFromDb);
    }


    [HttpPost]
    [HttpPost]
    public IActionResult DeletePost(int? id)
    {
        Product productFromDb = _db.Products.Find(id);
        if (productFromDb == null)
        {
            return NotFound();
        }

        // Manually delete all ordered items that reference the product
        var orderedItems = _db.OrderedItems.Where(oi => oi.ProductId == id);
        _db.OrderedItems.RemoveRange(orderedItems);

        // Now, delete the product
        _db.Products.Remove(productFromDb);
        _db.SaveChanges();

        TempData["success"] = "Product deleted successfully";
        return RedirectToAction("Productlist");
    }



}
