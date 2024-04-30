using E_commercePro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using E_commercePro.Data;
using Microsoft.EntityFrameworkCore;
using E_commercePro.Utility;
using Microsoft.Data.SqlClient;
using System.Linq;
using E_commercePro.ViewModel;

namespace E_commercePro.Controllers.user
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [AllowAnonymous]
        public IActionResult UserHome()
        
        
        
        {

           

            return View();
        }

        [AllowAnonymous]
        public IActionResult MainHome(string sortOrder)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var user = _db.Sign_Up.FirstOrDefault(u => u.ID == Convert.ToInt32(userId));

            var count = _db.Carts.Where(u => u.UserId == user.ID).Count();

            HttpContext.Session.SetInt32(CartCount.sessionCount, count);


            IQueryable<Product> productList = _db.Products
                .Include(p => p.category)// Include here
                .Include(p => p.Ratings)
                .Where(p => !p.category.IsListed);
                





            // Sorting logic
            switch (sortOrder)
            {
                case "name_desc":
                    productList = productList.OrderByDescending(p => p.Name);
                    break;
                case "Price":
                    productList = productList.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    productList = productList.OrderByDescending(p => p.Price);
                    break;
                //case "Rating":
                //    productList = productList.OrderBy(p => p.AverageRating);
                //    break;
                //case "rating_desc":
                //    productList = productList.OrderByDescending(p => p.AverageRating);
                //    break;
                //case "Date":
                //    productList = productList.OrderBy(p => p.CreatedAt);
                //    break;
                //case "date_desc":
                //    productList = productList.OrderByDescending(p => p.CreatedAt);
                //    break;
                default:
                    productList = productList.OrderBy(p => p.Name);
                    break;
            }



            var activeOffers = _db.Offers.ToList();

            var products = productList.ToList();

            // Pass the active offers to the view
            return View(new ProductOfferViewModel { Products = products, ActiveOffers = activeOffers });
        }





        [AllowAnonymous]
        public IActionResult ProductDetails(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var user = _db.Sign_Up.FirstOrDefault(u => u.ID == Convert.ToInt32(userId));
            var count = _db.Carts.Where(u => u.UserId == user.ID).Count();
            HttpContext.Session.SetInt32(CartCount.sessionCount, count);

            var product = _db.Products
                .Include(p => p.Ratings) // Include the Ratings collection
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var activeOffer = _db.Offers.FirstOrDefault(o => o.CategoryId == product.CategoryId);
            decimal discountedPrice = (decimal)product.Price; // Default to original price if no offer is found

            if (activeOffer != null)
            {
                discountedPrice = (decimal)product.Price - ((decimal)product.Price * (decimal)activeOffer.discount / 100m);
            }

            var viewModel = new ProductDetailsViewModel
            {
                Product = product,
                DiscountedPrice = discountedPrice,
                HasActiveOffer = activeOffer != null,
                ActiveOffer = activeOffer
            };

            return View(viewModel);
        }

        [AllowAnonymous]
        public IActionResult Logout()
        {
            // Clear authentication cookies
            HttpContext.SignOutAsync();

            // Clear local session cookies
            HttpContext.Session.Clear();
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                HttpContext.Session.Remove("UserSession");
                return RedirectToAction("Login", "User");
            }
            // Redirect to the login page
            return RedirectToAction("Login", "User");
        }






        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int id)
        {
           

            var userId = HttpContext.Session.GetString("UserId");
            var userIdInt = Convert.ToInt32(userId);

            // Check if the product is already in the user's wishlist
            var existingWishlistItem = await _db.whishlist
                .FirstOrDefaultAsync(w => w.UserId == userIdInt && w.ProductId == id);

            if (existingWishlistItem != null)
            {
                // The product is already in the wishlist, return a JSON response
                return Json(new { message = "This product is already in your wishlist." });
            }

            // If the product is not in the wishlist, add it
            var wishlist = new whishlist
            {
                UserId = userIdInt,
                ProductId = id
            };

            _db.whishlist.Add(wishlist);
            await _db.SaveChangesAsync();

            // Return a JSON response indicating success
            return Json(new { message = "Product added to your wishlist." });
        }

        [AllowAnonymous]
        public async Task<IActionResult> WhishList()

        {
            var userId = HttpContext.Session.GetString("UserId");
            var userIdInt = Convert.ToInt32(userId);

            var wishlistItems = await _db.whishlist
                .Where(w => w.UserId == userIdInt)
                .Include(w => w.Product)
                .Select(w => w.Product)
                .ToListAsync();

            return View(wishlistItems);
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int id)
        {

           

            var userId = HttpContext.Session.GetString("UserId");
            var userIdInt = Convert.ToInt32(userId);

            var wishlistItem = await _db.whishlist
                .FirstOrDefaultAsync(w => w.UserId == userIdInt && w.ProductId == id);

            if (wishlistItem == null)
            {
                return NotFound();
            }

            _db.whishlist.Remove(wishlistItem);
            await _db.SaveChangesAsync();

            return Json(new { message = "Item removed from wishlist." });
        }


    }
}
