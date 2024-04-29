 using E_commercePro.Data;
using E_commercePro.Migrations;
using E_commercePro.services;
using Microsoft.AspNetCore.Mvc;
using E_commercePro.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using E_commercePro.ViewModel;
using System.Security.Claims;
using System.Net;
using Razorpay.Api;
using Microsoft.AspNetCore.Http;
using Transaction = E_commercePro.Models.Transaction;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Identity;
using DocumentFormat.OpenXml.Spreadsheet;
using E_commercePro.Enum;
using Microsoft.CodeAnalysis;



namespace E_commercePro.Controllers.user
{
    public class UserProfileController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserProfileController(ApplicationDbContext context)
        {
            //_userManager = userManager;
            _db = context;

        }

        public async Task<IActionResult> Profile()
        {
            // Retrieve the email from session
            string userEmail = HttpContext.Session.GetString("UserEmail");

            // Query user details based on the email
            var userProfile = _db.Sign_Up.FirstOrDefault(x => x.Email == userEmail);

            if (userProfile == null)
            {
                // Handle case when user profile is not found
                return NotFound();
            }

            return View(userProfile);


        }

        public IActionResult PEdit(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }


            var productFromDb = _db.Sign_Up.Find(Id);

            if (productFromDb == null)
            {

                return NotFound();
            }

            ViewBag.CategoryList = new SelectList(_db.Categories, "Id", "Name");
            return View(productFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> PEdit(E_commercePro.Models.UserSignUp gn)
        {

            if (ModelState.IsValid)
            {


                _db.Sign_Up.Update(gn);
                _db.SaveChanges();
                TempData["success"] = "Product Update successfully";
                return RedirectToAction("Profile");
            }
            return View();
        }


        //Address



        public IActionResult AddressView()
        {
            // Get the logged-in user's ID from the session
            var userId = HttpContext.Session.GetString("UserId");

            // Retrieve addresses associated with the logged-in user's ID
            var userAddresses = _db.Addresses.Where(a => a.UserId == Convert.ToInt32(userId)).ToList();

            return View(userAddresses);

        }

        public IActionResult AddAddress()
        {
           
            var address = new E_commercePro.Models.Address();

          
            var userId = HttpContext.Session.GetString("UserId");

            
            address.UserId = Convert.ToInt32(userId);

            return View(address);
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress(E_commercePro.Models.Address add)
        {
            if (ModelState.IsValid)
            {
     
                var userId = HttpContext.Session.GetString("UserId");

               
                add.UserId = Convert.ToInt32(userId);

                _db.Addresses.Add(add);
                await _db.SaveChangesAsync();

                return RedirectToAction("AddressView");
            }

            return View(add);
        }

        public IActionResult EditAd(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            var addressFromDb = _db.Addresses.Find(Id);

            if (addressFromDb == null)
            {
                return NotFound();
            }

            return View(addressFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> EditAd(E_commercePro.Models.Address addressModel)
        {
            if (ModelState.IsValid)
            {
             
                var existingAddress = _db.Addresses.Find(addressModel.AddressId);

                if (existingAddress == null)
                {
                    return NotFound(); 
                }

            
                existingAddress.fillname = addressModel.fillname;
                existingAddress.phone = addressModel.phone;
                existingAddress.street = addressModel.street;
                existingAddress.locality = addressModel.locality;
                existingAddress.district = addressModel.district;
                existingAddress.state = addressModel.state;
                existingAddress.pincode = addressModel.pincode;

                _db.Addresses.Update(existingAddress);
                await _db.SaveChangesAsync();

                TempData["success"] = "Address updated successfully";
                return RedirectToAction("AddressView");
            }

            return View(addressModel);
        }

        [HttpPost]
        public IActionResult DeleteAddress(int id)
        {
            var addressToDelete = _db.Addresses.Find(id);

            if (addressToDelete == null)
            {
                return NotFound();
            }

            _db.Addresses.Remove(addressToDelete);
            _db.SaveChanges();

            return Json(new { success = true });
        }


        //Change Password
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(E_commercePro.ViewModel.ChangePasswordViewModel pass, string CurrebtPassword)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetString("UserId");

                var user = _db.Sign_Up.FirstOrDefault(u => u.ID == Convert.ToInt32(userId));

                if (user == null)
                {
                    return NotFound();
                }

                if (user.Password == CurrebtPassword)
                {

                    user.Password = pass.NewPassword;
                    user.ConformPassword = pass.NewPassword;
                    _db.SaveChanges();
                    return RedirectToAction("Profile");
                }
                else
                {

                    ModelState.AddModelError(nameof(CurrebtPassword), "Invalid current password");
                    return View(pass);
                }


            }

            return View(pass);
        }



        //Order
        public async Task<IActionResult> OrderList()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var UserId = Convert.ToInt32(userId);
            var orderListViewModel = new OrderListViewModel
            {
                Orders = await _db.Orders
         .Include(o => o.OrderedItems)
         .ThenInclude(oi => oi.Product)
         .Include(o => o.ShippingAddress)
         .Where(o => o.UserId == UserId)
         .Select(o => new OrderDetailViewModel
         {
             Order = o,
             Product = o.Product,
             Address = o.ShippingAddress,
             OrderedItems = o.OrderedItems,
             IsReturnPeriod = o.Status == Enum.OrderStatus.Delivered,
             IsOrderCancelled = Helpers.OrderStatusHelper.IsOrderCancelled(o.Status)
         })
         .ToListAsync()
            };

            return View(orderListViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> CancelOrder(int Id)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    // Retrieve the order
                    var order = await _db.Orders.Include(o => o.UserSignUp).FirstOrDefaultAsync(o => o.Id == Id);
                    if (order != null)
                    {
                        // Check if the order is paid
                        if (order.PaymentStatus == Enum.PaymentStatus.Paid)
                        {
                            // Get the user's wallet
                            var wallet = await _db.Wallets.FirstOrDefaultAsync(w => w.UserId == order.UserId);

                            // Refund the order amount to the wallet
                            if (wallet != null)
                            {
                                wallet.Balance += order.OrderAmount-order.Amount;

                                if (wallet.Transactions == null)
                                {
                                    wallet.Transactions = new List<Transaction>(); // Initialize the transactions list
                                }

                                // Create a new transaction record for the refund
                                wallet.Transactions.Add(new Transaction
                                {
                                    Amount = order.OrderAmount - order.Amount, 
                                    Type = "Refund",
                                    OrderDate = DateTime.Now,
                                    WalletId = wallet.Id,
                                    statuss = true
                                });

                                _db.Wallets.Update(wallet);
                            }
                        }

                        // Update order status to "Cancelled"
                        order.Status = Enum.OrderStatus.Cancelled;
                        _db.Entry(order).State = EntityState.Modified;
                        _db.SaveChanges();
                        transaction.Commit();
                    }

                    // Redirect back to the order list page
                    return RedirectToAction("OrderList");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Handle exception
                    TempData["ErrorMessage"] = "Failed to cancel order";
                    return RedirectToAction("OrderList");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReturnAsync(int id)
        {
            var order = await _db.Orders.Include(o => o.UserSignUp).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.PaymentStatus == Enum.PaymentStatus.Paid)
            {
                // Get the user's wallet
                var wallet = await _db.Wallets.FirstOrDefaultAsync(w => w.UserId == order.UserId);

                // Refund the order amount to the wallet
                if (wallet != null)
                {
                    wallet.Balance += order.OrderAmount - order.Amount;

                    if (wallet.Transactions == null)
                    {
                        wallet.Transactions = new List<Transaction>(); // Initialize the transactions list
                    }

                    // Create a new transaction record for the refund
                    wallet.Transactions.Add(new Transaction
                    {
                        Amount = order.OrderAmount - order.Amount,
                        Type = "Refund",
                        OrderDate = DateTime.Now,
                        WalletId = wallet.Id,
                        statuss = true
                    });

                    _db.Wallets.Update(wallet);
                }
            }

            // Update the order status to "Returned"
            order.Status = Enum.OrderStatus.Returned;
            _db.Orders.Update(order);
            await _db.SaveChangesAsync();

            return RedirectToAction("OrderList");
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            
            var order = await _db.Orders
                .Include(o => o.UserSignUp)
                .Include(o => o.OrderedItems)
                    .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Ratings)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            
            decimal total = (decimal)order.OrderedItems.Sum(oi => oi.Quantity * oi.Product.Price);

            decimal totalPrice = total - order.Amount;
            


            
            var viewModel = new OrderListViewModel
            {
                Order = order,
                User = order.UserSignUp,
                Product = order.OrderedItems.Select(oi => oi.Product).FirstOrDefault(),
                ShippingAddress = order.ShippingAddress,
                TotalPrice = totalPrice,
                ProductRatings = order.OrderedItems.SelectMany(oi => oi.Product.Ratings).ToList(),
                IsReturnPeriod = order.Status == Enum.OrderStatus.Delivered

            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RateProduct(int productId, int stars, int orderId)
         {
            // Validate the stars value
            if (stars < 1 || stars > 5)
            {
                return BadRequest("Invalid rating value");
            }

            // Get the current user
            var userId = HttpContext.Session.GetString("UserId");

            var user = Convert.ToInt32(userId);

            // Check if the user has already rated this product
            var existingRating = _db.Ratings.FirstOrDefault(r => r.ProductId == productId && r.UserId == user);
            if (existingRating != null)
            {
                // User has already rated this product, update the rating
                existingRating.Stars = stars;
                existingRating.RatingTimestamp = DateTime.Now;
            }
            else
            {
                // User hasn't rated this product, create a new rating
                var newRating = new Rating
                {
                    ProductId = productId,
                    UserId = user,
                    Stars = stars,
                    RatingTimestamp = DateTime.Now
                };
               _db.Ratings.Add(newRating);
            }

            // Save changes to the database
            await _db.SaveChangesAsync();

            return RedirectToAction("OrderDetails", "UserProfile", new { id = orderId });
        }



        public async Task<IActionResult> WalletView()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId != null)
            {
                var wallet = await _db.Wallets.Include(w => w.Transactions)
                    .FirstOrDefaultAsync(w => w.UserId == Convert.ToInt32(userId));

                if (wallet == null)
                {
                    wallet = new Wallet
                    {
                        UserId = Convert.ToInt32(userId),
                        Balance = 0,
                        Transactions = new List<Transaction>()
                    };
                    _db.Wallets.Add(wallet);
                    await _db.SaveChangesAsync();
                }

                return View(wallet);
            }

            return NotFound();
        }





        [HttpPost]
        public IActionResult Pay(int Balance)
        {
            HttpContext.Session.SetInt32("Balance", Balance);

           Random _random = new Random();
            string transactionId = _random.Next(0, 10000).ToString();

            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", Convert.ToDecimal(Balance) * 100); // this amount should be same as transaction amount
            input.Add("currency", "INR");
            input.Add("receipt", transactionId);


            string key = "rzp_test_VoCbsmOUwmU7VV";
            string secret = "eO7LMwJlckK1Vo3EfBa2QvEm";

            RazorpayClient client = new RazorpayClient(key, secret);

            Razorpay.Api.Order ordr = client.Order.Create(input);
            ViewBag.orderId = ordr["id"].ToString();
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Online(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var addAmount = HttpContext.Session.GetInt32("Balance");

            if (userId != null)
            {
                var wallet = await _db.Wallets.FirstOrDefaultAsync(w => w.UserId == Convert.ToInt32(userId));

                if (wallet != null)
                {
                    var currentBalance = wallet.Balance;
                    var transaction = new Transaction
                    {
                        Amount = (int)addAmount,
                        Type = "Deposit",
                        WalletId = wallet.Id,
                        statuss = true // Payment successful
                    };

                    _db.Transactions.Add(transaction);
                    await _db.SaveChangesAsync();

                    var newBalance = currentBalance + (int)addAmount;
                    wallet.Balance = newBalance;
                    await _db.SaveChangesAsync();
                }
                else
                {
                    // Payment failed
                    var failedTransaction = new Transaction
                    {
                        Amount = (int)addAmount,
                        Type = "Deposit",
                        statuss = false // Payment failed
                    };

                    _db.Transactions.Add(failedTransaction);
                    await _db.SaveChangesAsync();
                }
            }
            else
            {
                // Payment failed
                var failedTransaction = new Transaction
                {
                    Amount = (int)addAmount,
                    Type = "Deposit",
                    statuss = false // Payment failed
                };

                _db.Transactions.Add(failedTransaction);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("WalletView");
        }



 
        public IActionResult RePay(int proId)
        {
            HttpContext.Session.SetInt32("Oid", proId);

            var orid = _db.Orders.FirstOrDefault(o => o.Id == proId);

            var total = orid.DiscountTotal;
            if (orid.DiscountTotal==0)
            {
                total = orid.OrderAmount;
            }
         

            Random _random = new Random();
            string transactionId = _random.Next(0, 10000).ToString();

            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", total * 100); // this amount should be same as transaction amount
            input.Add("currency", "INR");
            input.Add("receipt", transactionId);


            string key = "rzp_test_VoCbsmOUwmU7VV";
            string secret = "eO7LMwJlckK1Vo3EfBa2QvEm";

            RazorpayClient client = new RazorpayClient(key, secret);

            Razorpay.Api.Order ordr = client.Order.Create(input);
            ViewBag.orderId = ordr["id"].ToString();
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> RePay(string razorpay_payment_id, int razorpay_order_id, string razorpay_signature)
        {
            // Validate Razorpay payment here if needed

            var Oid = HttpContext.Session.GetInt32("Oid");
            var order = await _db.Orders.FindAsync(Oid);
            if (order == null)
            {
                // Handle case where order is not found
                return RedirectToAction("Error", "Order");
            }

            // Update order payment status to 'Paid'
            order.PaymentStatus = PaymentStatus.Paid;
            await _db.SaveChangesAsync();

            return RedirectToAction("Success", "Order");
        }


    }

}

