



using E_commercePro.Data;
using E_commercePro.Models;
using E_commercePro.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace E_commercePro.Controllers.user
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CartController(ApplicationDbContext context)
        {
            _db = context;
        }

    

        public async Task<IActionResult> AddToCart(int productId, int quantityChange)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var user = _db.Sign_Up.FirstOrDefault(u => u.ID == Convert.ToInt32(userId));

            if (user != null)
            {
                var product = await _db.Products.FindAsync(productId);

                if (product != null)
                {
                    if (product.stock >= quantityChange)
                    {
                        var cartItem = await _db.Carts.FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == user.ID);

                        if (cartItem != null)
                        {
                            // If the quantityChange is positive, increase the quantity; if negative, decrease the quantity
                            cartItem.Quantity += quantityChange;
                            _db.Carts.Update(cartItem);
                        }
                        else
                        {
                            // If the item is not in the cart, add it with the specified quantity
                            Cart newItemToCart = new Cart
                            {
                                ProductId = productId,
                                UserId = user.ID,
                                Quantity = quantityChange > 0 ? quantityChange : 1 // Make sure the quantity is at least 1
                            };
                            await _db.Carts.AddAsync(newItemToCart);
                        }

                        await _db.SaveChangesAsync(); // Save changes to the cart only
                        TempData["success"] = "Product is added cart successfully";

                        return RedirectToAction("ProductDetails", "Home", new { id = productId });
                    }
                    else
                    {
                        // Stock quantity insufficient
                        return RedirectToAction("ProductDetails", "Home", new { id = productId, insufficientStock = true });
                    }
                }
            }

            // Handle other cases or errors
            return RedirectToAction("ProductDetails", "Home", new { id = productId });
        }




        public IActionResult CartList()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var user = _db.Sign_Up.FirstOrDefault(u => u.ID == Convert.ToInt32(userId));

            if (user != null)
            {
                var cartItems = _db.Carts.Include(u => u.Product).Where(u => u.UserId == user.ID).ToList();
                decimal subtotal = cartItems.Sum(item => item.Quantity * (decimal)item.Product.Price); // Calculate subtotal

                var currentDate = DateTime.Now; // Get the current date
                var validCoupons = _db.Coupons.Where(c => c.expiryDate >= currentDate).ToList(); // Filter out expired coupons



                CartListVM cartListVM = new CartListVM()
                {
                    productList = cartItems,
                    Subtotal = subtotal,
                    TotalPrice = subtotal,
                    coupon = validCoupons
                };

                return View(cartListVM);
            }

            return View();
        }




        [HttpPost]
        public async Task<IActionResult> DeleteCartItem(int productId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var user = _db.Sign_Up.FirstOrDefault(u => u.ID == Convert.ToInt32(userId));

            if (user != null)
            {
                var cartItem = await _db.Carts.FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == user.ID);

                if (cartItem != null)
                {
                    _db.Carts.Remove(cartItem);
                    await _db.SaveChangesAsync();
                }
            }
            TempData["success"] = "Product removed successfully";
            // You can return JSON or any other response as needed
            return Ok();
        }

     


        [HttpPost]
        public async Task<IActionResult> UpdateCartItemQuantity(int productId, int quantity)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var user = _db.Sign_Up.FirstOrDefault(u => u.ID == Convert.ToInt32(userId));

            if (user != null)
            {
                var cartItem = await _db.Carts.Include(c => c.Product).FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == user.ID);

                if (cartItem != null)
                {
                    var product = cartItem.Product;

                    // Calculate the change in quantity
                    int quantityChange = quantity;

                    // Check if there's enough stock available
                    if (product.stock >= Math.Abs(quantityChange))
                    {
                        // Update the quantity of the cart item
                        cartItem.Quantity = quantity;
                        _db.Carts.Update(cartItem);

                        await _db.SaveChangesAsync();
                        return Ok(); 
                    }
                    else
                    {
                      
                        return BadRequest("The selected quantity exceeds available stock");
                    }
                }
            }

            return NotFound(); 
        }



        [HttpPost]
        public IActionResult ApplyCoupon(string couponCode)
        {
           
            var coupon = _db.Coupons.FirstOrDefault(c => c.Code == couponCode);

        


            if (coupon != null)

            {

                HttpContext.Session.SetString("cpcode", couponCode);
                // Calculate the subtotal of the user's cart
                var userId = HttpContext.Session.GetString("UserId");
                var user = _db.Sign_Up.FirstOrDefault(u => u.ID == Convert.ToInt32(userId));
                var cartItems = _db.Carts.Include(u => u.Product).Where(u => u.UserId == user.ID).ToList();
                decimal subtotal = cartItems.Sum(item => item.Quantity * (decimal)item.Product.Price);
        

                if (subtotal >= coupon.Minamount)
                {

                    return Json(new { success = true, discount = coupon.Discount, message="Coupon added successfully" });
                }
                else
                {

                
                    return Json(new { success = false, message = "Your total amount is not enough to avail this coupon." });
                   

                }
            }
            else
            {

              
                return Json(new { success = false, message = "Invalid coupon code." });
            }

        }

        [HttpPost]
        public IActionResult RemoveCoupon()
        {
            try
            {
                // Get the current user's ID from the session or any other source
                var userId = HttpContext.Session.GetString("UserId");

                // Remove the applied coupon from the user's session
                HttpContext.Session.Remove("cpcode" + userId);

               
                return Json(new { success = true, message = "Your Coupon removed successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { success = false, message = "An error occurred while removing the coupon." });
            }
        }

        [HttpGet]
        public IActionResult CheckSubtotalAndCouponMinAmount()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var user = _db.Sign_Up.FirstOrDefault(u => u.ID == Convert.ToInt32(userId));
            var cartItems = _db.Carts.Include(u => u.Product).Where(u => u.UserId == user.ID).ToList();
            decimal subtotal = cartItems.Sum(item => item.Quantity * (decimal)item.Product.Price);

            var getcd = HttpContext.Session.GetString("cpcode");
            var coupon = _db.Coupons.FirstOrDefault(c => c.Code == getcd);

            if(coupon != null)
            {
                if (subtotal < coupon.Minamount)
                {
                    return Json(new { success = false, message = "Subtotal is less than the minimum amount required for this coupon." });
                }
            }

          

            return Json(new { success = true });
        }


    }
}

