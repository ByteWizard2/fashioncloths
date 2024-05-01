using E_commercePro.Data;
using E_commercePro.Enum;
using E_commercePro.Models;
using E_commercePro.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Razorpay.Api;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace E_commercePro.Controllers.user
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrderController(ApplicationDbContext context)
        {
            _db = context;
        }

        public async Task<IActionResult> OrderView()
        {
            var userId = HttpContext.Session.GetString("UserId");

            // Get the user's addresses
            var userAddresses = await _db.Addresses
                .Where(a => a.UserId == Convert.ToInt32(userId))
                .ToListAsync();

            // Get the cart details for the logged-in user
            var cartItems = await _db.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == Convert.ToInt32(userId))
                .ToListAsync();

            // Calculate total price considering quantities
            decimal totalPrice = (decimal)cartItems.Sum(c => c.Product.Price * c.Quantity);

            // Retrieve the coupon information
            var couponCode = HttpContext.Session.GetString("cpcode");
            var coupon = _db.Coupons.FirstOrDefault(c => c.Code == couponCode);
            decimal discountAmount = 0;
            if (coupon != null && totalPrice >= coupon.Minamount)
            {
                discountAmount = coupon.Discount;
            }

            var selectedPaymentMethod = HttpContext.Session.GetString("Paymetd");
            if (selectedPaymentMethod != null)
            {
                bool isWalletPayment = selectedPaymentMethod == "Wallet";
                // Create a ViewModel to hold both user addresses, cart details, and coupon information
                var orderViewMode = new OrderViewModel
                {
                    UserAddresses = userAddresses,
                    CartItems = cartItems,
                    TotalPrice = totalPrice,
                    Discount = discountAmount,
                    CouponCode = couponCode,
                    IsWalletPayment = isWalletPayment
                };

                return View(orderViewMode);
            }


            var orderViewModel = new OrderViewModel
            {
                UserAddresses = userAddresses,
                CartItems = cartItems,
                TotalPrice = totalPrice,
                Discount = discountAmount,
                CouponCode = couponCode,
            
            };

            return View(orderViewModel);
            // Pass the ViewModel to the view

        }

        [HttpPost]
        public async Task<IActionResult> AddAddress(Address add)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetString("UserId");

                add.UserId = Convert.ToInt32(userId);

                _db.Addresses.Add(add);
                await _db.SaveChangesAsync();

                TempData["success"] = " Address Added successfully";
                return RedirectToAction("OrderView");
            }

            return View(add);
        }



        [HttpPost]
        public async Task<IActionResult> PlaceOrder(int selectedAddressId, string selectedPaymentMethod)
        {
           
            if (selectedAddressId != null && selectedPaymentMethod != null)
            {
                HttpContext.Session.SetInt32("addressId", selectedAddressId);

                HttpContext.Session.SetString("Paymetd", selectedPaymentMethod);


                // Get the user's ID from the session
                var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

                //check the address is selcted or not
                var addressId = HttpContext.Session.GetInt32("addressId");
                var Paymtd = HttpContext.Session.GetString("Paymetd");

                if (addressId == 0 || string.IsNullOrEmpty(Paymtd))
                {
                    ModelState.AddModelError("", "Please select both an address and a payment method before placing the order.");
                    var userAddresses = await _db.Addresses
                                       .Where(a => a.UserId == Convert.ToInt32(userId))
                                       .ToListAsync();
                    var orderViewModel = new OrderViewModel
                    {
                        UserAddresses = userAddresses,
                        CartItems = (List<Cart>?)await _db.Carts
                        .Where(a => a.UserId == Convert.ToInt32(userId))
                        .Include(c => c.Product)
                        .ToListAsync(),
                        TotalPrice = (decimal)(await _db.Carts
                        .Where(a => a.UserId == Convert.ToInt32(userId))
                        .Include(c => c.Product)
                        .ToListAsync()).Sum(c => c.Quantity * c.Product.Price)
                    };

                   
                    return View("OrderView", orderViewModel);
                }



                var pay = "Online";

                if (selectedPaymentMethod == pay)
                {

                    try
                    {
                        var user = await _db.Sign_Up.Where(u => u.ID == Convert.ToInt32(HttpContext.Session.GetString("UserId"))).ToListAsync();

                        // Get the user's addresses
                        var userAddresses = await _db.Addresses
                            .Where(a => a.UserId == Convert.ToInt32(HttpContext.Session.GetString("UserId")))
                            .ToListAsync();

                        // Get the cart details for the logged-in user
                        var cartItem = await _db.Carts
                            .Include(c => c.Product)
                            .Where(c => c.UserId == Convert.ToInt32(HttpContext.Session.GetString("UserId")))
                            .ToListAsync();

                        // Calculate total price considering quantities
                        decimal totalPrice = (decimal)cartItem.Sum(c => c.Product.Price * c.Quantity);

                        // Create a ViewModel to hold both user addresses and cart details
                        var orderViewModel = new OrderViewModel
                        {
                            UserAddresses = userAddresses,
                            User = user,
                            CartItems = cartItem,
                            TotalPrice = totalPrice
                        };

                        //coupon
                        string CouponCode = HttpContext.Session.GetString("cpcode");

                        if (CouponCode != null)
                        {
                            var Coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.Code == CouponCode);
                            totalPrice -= Coupon.Discount;
                        }

                


                        Random _random = new Random();
                        string transactionId = _random.Next(0, 10000).ToString();

                        Dictionary<string, object> input = new Dictionary<string, object>();
                        input.Add("amount", Convert.ToDecimal(totalPrice) * 100) ; // this amount should be same as transaction amount
                        input.Add("currency", "INR");
                        input.Add("receipt", transactionId);

                        string key = "rzp_test_VoCbsmOUwmU7VV";
                        string secret = "eO7LMwJlckK1Vo3EfBa2QvEm";

                        RazorpayClient client = new RazorpayClient(key, secret);

                        Razorpay.Api.Order ordere = client.Order.Create(input);
                        ViewBag.orderId = ordere["id"].ToString();
                        return View("Online", orderViewModel);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorMessage = "Sorry, we are experiencing issues with Razorpay. Please try again later.";
                        return View("OrderView");
                    }


                }


                var wall = "Wallet";
                if (selectedPaymentMethod == wall)
                {
                    var user = await _db.Sign_Up.Where(u => u.ID == Convert.ToInt32(HttpContext.Session.GetString("UserId"))).ToListAsync();

                    // Get the user's addresses
                    var userAddresses = await _db.Addresses
                        .Where(a => a.UserId == Convert.ToInt32(HttpContext.Session.GetString("UserId")))
                        .ToListAsync();

                    // Get the cart details for the logged-in user
                    var cartItem = await _db.Carts
                        .Include(c => c.Product)
                        .Where(c => c.UserId == Convert.ToInt32(HttpContext.Session.GetString("UserId")))
                        .ToListAsync();

                    // Calculate total price considering quantities
                    decimal totalPrice = (decimal)cartItem.Sum(c => c.Product.Price * c.Quantity);

                    // Create a ViewModel to hold both user addresses and cart details
                    var orderViewModel = new OrderViewModel
                    {
                        UserAddresses = userAddresses,
                        User = user,
                        CartItems = cartItem,
                        TotalPrice = totalPrice,

                    };

                    //coupon
                    string CouponCode = HttpContext.Session.GetString("cpcode");
                    var Coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.Code == CouponCode);
                    if (Coupon != null)
                    {
                        totalPrice -= Coupon.Discount;
                    }
                   

                    var wallettotal = _db.Wallets.FirstOrDefault(o => o.UserId == Convert.ToInt32(HttpContext.Session.GetString("UserId")));

                    if (totalPrice<= wallettotal.Balance)
                    {
                        return RedirectToAction("Wallet");
                    }
                    else
                    {
                        var userI = HttpContext.Session.GetString("UserId");
                        var useridd = Convert.ToInt32(userI);
                        var wallet = await _db.Wallets.FirstOrDefaultAsync(w => w.UserId == useridd);

                        var withdrawalTransactions = new Transaction
                        {
                            Amount = (int)totalPrice,
                            Type = "Withdrawal",
                            OrderDate = DateTime.Now,
                            WalletId = wallet.Id,
                            statuss = false
                        };

                        _db.Transactions.Add(withdrawalTransactions);

                        await _db.SaveChangesAsync();

                        TempData["error"] = "You don't have insufficient balance in your wallet Please add amount in you wallet";
                        return RedirectToAction("OrderView");
                    }

                    
                }



              


                // Get the user's ID from the session
                var UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

                // Calculate the total order amount
                var cartItems = await _db.Carts
                    .Include(c => c.Product)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();
                var totalOrderAmount = (decimal)cartItems.Sum(c => c.Product.Price * c.Quantity);

                // Check if the product exists in the Products table
                var productId = cartItems.First().ProductId;
                var productExists = await _db.Products.AnyAsync(p => p.Id == productId);
                if (!productExists)
                {
                    ModelState.AddModelError("", "The product associated with the order does not exist.");
                    var userAddresses = await _db.Addresses
                                         .Where(a => a.UserId == Convert.ToInt32(userId))
                                         .ToListAsync();
                    var orderViewModel = new OrderViewModel
                    {
                        UserAddresses = userAddresses,
                        CartItems = (List<Cart>?)await _db.Carts
                        .Where(a => a.UserId == Convert.ToInt32(userId))
                        .Include(c => c.Product)
                        .ToListAsync(),
                        TotalPrice = (decimal)(await _db.Carts
                        .Where(a => a.UserId == Convert.ToInt32(userId))
                        .Include(c => c.Product)
                        .ToListAsync()).Sum(c => c.Quantity * c.Product.Price)
                    };
                    return View("OrderView", orderViewModel);
                }


                //coupon

                string couponCode = HttpContext.Session.GetString("cpcode");

                var coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.Code == couponCode);

                decimal discountAmount = 0;
                if (coupon != null && totalOrderAmount >= coupon.Minamount)
                {
                    discountAmount = coupon.Discount;
                }

                var check = totalOrderAmount - discountAmount;

                //check the cash on delivery amount greater then 1000
                if (check >= 1000)
                {
                    var userAddresses = await _db.Addresses
                                       .Where(a => a.UserId == Convert.ToInt32(userId))
                                       .ToListAsync();
                    var orderViewModel = new OrderViewModel
                    {
                        UserAddresses = userAddresses,
                        CartItems = (List<Cart>?)await _db.Carts
                         .Where(a => a.UserId == Convert.ToInt32(userId))
                        .Include(c => c.Product)
                        .ToListAsync(),
                        TotalPrice = (decimal)(await _db.Carts
                        .Where(a => a.UserId == Convert.ToInt32(userId))
                        .Include(c => c.Product)
                        .ToListAsync()).Sum(c => c.Quantity * c.Product.Price)
                    };

                    TempData["error"] = "Cash on Delivery only availabale in belove 1000 ";
                    TempData.Keep();
                    return View("OrderView", orderViewModel);
                  
                }

                // Create a new Order object
                var order = new OrderE
                {
                    UserId = userId,
                    ShippingAddressId = selectedAddressId,
                    ProductId = productId, // Assign the ProductId
                    OrderAmount = (int)totalOrderAmount,
                    Status = OrderStatus.Pending,
                    PaymentMethod = PaymentMethod.COD,
                    PaymentStatus = PaymentStatus.Pending,
                    OrderDate = DateTime.Now,
                    Amount = (int)discountAmount,
                    CouponCod = couponCode,
                    DiscountTotal= (int)totalOrderAmount-coupon.Discount
                };



                // Add the order to the database
                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                // Create and store OrderedItems for each cart item
                foreach (var cartItem in cartItems)
                {
                    var orderedItem = new OrderedItem
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        ProductPrice = (int)cartItem.Product.Price,
                        // You may also want to copy other properties from the cart item
                    };

                    _db.OrderedItems.Add(orderedItem);
                }

                await _db.SaveChangesAsync();

                // Clear the user's cart after placing the order
                _db.Carts.RemoveRange(cartItems);
                await _db.SaveChangesAsync();

                // Redirect to the Success view
                return RedirectToAction("Success");
            }

            TempData["error"] = "Select the address";
            return RedirectToAction("OrderView");
        }




        public async Task<IActionResult> PayFail()
        {
            var addressId = HttpContext.Session.GetInt32("addressId");
            var paymentMethod = HttpContext.Session.GetString("PaymentMethod");

            // Get the user's ID from the session
            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            // Get the user's addresses
            var userAddresses = await _db.Addresses
                .Where(a => a.UserId == Convert.ToInt32(userId))
                .ToListAsync();

            // Calculate the total order amount
            var cartItems = await _db.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
            var totalOrderAmount = (decimal)cartItems.Sum(c => c.Product.Price * c.Quantity);

            // Check if the product exists in the Products table
            var productId = cartItems.First().ProductId;
            var productExists = await _db.Products.AnyAsync(p => p.Id == productId);

            // Coupon
            string couponCode = HttpContext.Session.GetString("cpcode");
            var coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.Code == couponCode);
            decimal discountAmount = 0;
            if (coupon != null && totalOrderAmount >= coupon.Minamount)
            {
                discountAmount = coupon.Discount;
            }

            //if there is coupon 
            if (couponCode != null)
            {
                // Create a new Order object
                var ordere = new OrderE
                {
                    UserId = userId,
                    ShippingAddressId = addressId ?? 0,
                    ProductId = productId,
                    OrderAmount = (int)totalOrderAmount,
                    Status = OrderStatus.Pending,
                    PaymentMethod = PaymentMethod.Online,
                    PaymentStatus = PaymentStatus.Pending,
                    OrderDate = DateTime.Now,
                    Amount = (int)discountAmount,
                    CouponCod = couponCode ?? "",
                    DiscountTotal = (int)totalOrderAmount - coupon.Discount
                };

                // Add the order to the database
                _db.Orders.Add(ordere);
                await _db.SaveChangesAsync();

                // Create and store OrderedItems for each cart item
                foreach (var cartItem in cartItems)
                {
                    var orderedItem = new OrderedItem
                    {
                        OrderId = ordere.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        ProductPrice = (int)cartItem.Product.Price
                    };

                    _db.OrderedItems.Add(orderedItem);
                }

                await _db.SaveChangesAsync();

                // Redirect to the OrderView action with the necessary data
                var orderViewModel = new OrderViewModel
                {
                    UserAddresses = userAddresses,
                    CartItems = cartItems,
                    TotalPrice = totalOrderAmount,
                    Discount = discountAmount,
                    CouponCode = couponCode,

                };

                TempData["error"] = "Your Payment is failde";
                return View("OrderView", orderViewModel);
            }

            //if there is no coupon 
            // Create a new Order object
            var order = new OrderE
            {
                UserId = userId,
                ShippingAddressId = addressId ?? 0,
                ProductId = productId,
                OrderAmount = (int)totalOrderAmount,
                Status = OrderStatus.Pending,
                PaymentMethod = PaymentMethod.Online,
                PaymentStatus = PaymentStatus.Pending,
                OrderDate = DateTime.Now,
                Amount = 0,
                CouponCod = " ",
                DiscountTotal = 0
            };

            // Add the order to the database
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // Create and store OrderedItems for each cart item
            foreach (var cartItem in cartItems)
            {
                var orderedItem = new OrderedItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    ProductPrice = (int)cartItem.Product.Price
                };

                _db.OrderedItems.Add(orderedItem);
            }

            await _db.SaveChangesAsync();

            // Redirect to the OrderView action with the necessary data
            // Redirect to the OrderView action with the necessary data
            var orderViewModele = new OrderViewModel
            {
                UserAddresses = userAddresses,
                CartItems = cartItems,
                TotalPrice = totalOrderAmount,
                Discount = discountAmount,
                CouponCode = couponCode,

            };


            TempData["error"] = "Your Payment is failde";
            return View("OrderView", orderViewModele);
        }




        [HttpPost]
        public async Task<IActionResult> Online()
        {
            var addressId = HttpContext.Session.GetInt32("addressId");
            var Paymtd = HttpContext.Session.GetString("Paymetd");



           
            // Get the user's ID from the session
            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            // Calculate the total order amount
            var cartItems = await _db.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
            var totalOrderAmount = (decimal)cartItems.Sum(c => c.Product.Price * c.Quantity);

            // Check if the product exists in the Products table
            var productId = cartItems.First().ProductId;
            var productExists = await _db.Products.AnyAsync(p => p.Id == productId);
          

            //coupon

            string couponCode = HttpContext.Session.GetString("cpcode");

            var coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.Code == couponCode);

            decimal discountAmount = 0;
            if (coupon != null && totalOrderAmount >= coupon.Minamount)
            {
                discountAmount = coupon.Discount;
            }

            //if there is  coupon 
            if (couponCode!=null)
            {
                // Create a new Order object
                var ordere = new OrderE
                {
                    UserId = userId,
                    ShippingAddressId = (int)addressId,
                    ProductId = productId, // Assign the ProductId
                    OrderAmount = (int)totalOrderAmount,
                    Status = OrderStatus.Pending,
                    PaymentMethod = PaymentMethod.Online,
                    PaymentStatus = PaymentStatus.Paid,
                    OrderDate = DateTime.Now,
                    Amount = (int)discountAmount,
                    CouponCod = couponCode,
                    DiscountTotal = (int)totalOrderAmount - coupon.Discount
                };
                // Add the order to the database
                _db.Orders.Add(ordere);
                await _db.SaveChangesAsync();

                // Create and store OrderedItems for each cart item
                foreach (var cartItem in cartItems)
                {
                    var orderedItem = new OrderedItem
                    {
                        OrderId = ordere.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        ProductPrice = (int)cartItem.Product.Price,
                        // You may also want to copy other properties from the cart item
                    };

                    _db.OrderedItems.Add(orderedItem);
                }

                await _db.SaveChangesAsync();

                // Clear the user's cart after placing the order
                _db.Carts.RemoveRange(cartItems);
                await _db.SaveChangesAsync();

                // Redirect to the Success view
                return RedirectToAction("Success");

            }


           
            //if there is no coupon 

                // Create a new Order object
                var order = new OrderE
                {
                    UserId = userId,
                    ShippingAddressId = (int)addressId,
                    ProductId = productId, // Assign the ProductId
                    OrderAmount = (int)totalOrderAmount,
                    Status = OrderStatus.Pending,
                    PaymentMethod = PaymentMethod.Online,
                    PaymentStatus = PaymentStatus.Paid,
                    OrderDate = DateTime.Now,
                    Amount = 0,
                    CouponCod =" ",
                    DiscountTotal = 0
                };
            

           



            // Add the order to the database
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // Create and store OrderedItems for each cart item
            foreach (var cartItem in cartItems)
            {
                var orderedItem = new OrderedItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    ProductPrice = (int)cartItem.Product.Price,
                    // You may also want to copy other properties from the cart item
                };

                _db.OrderedItems.Add(orderedItem);
            }

            await _db.SaveChangesAsync();

            // Clear the user's cart after placing the order
            _db.Carts.RemoveRange(cartItems);
            await _db.SaveChangesAsync();

            // Redirect to the Success view
            return RedirectToAction("Success");

            
        }

       
        public async Task<IActionResult> Wallet()
        {
            var addressId = HttpContext.Session.GetInt32("addressId");
            var Paymtd = HttpContext.Session.GetString("Paymetd");



            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

          

            // Calculate the total order amount
            var cartItems = await _db.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
            var totalOrderAmount = (decimal)cartItems.Sum(c => c.Product.Price * c.Quantity);

           

            // Check if the product exists in the Products table
            var productId = cartItems.First().ProductId;
            var productExists = await _db.Products.AnyAsync(p => p.Id == productId);
            if (!productExists)
            {
                ModelState.AddModelError("", "The product associated with the order does not exist.");
                var userAddresses = await _db.Addresses.ToListAsync();
                var orderViewModel = new OrderViewModel
                {
                    UserAddresses = userAddresses,
                    CartItems = (List<Cart>?)await _db.Carts
                    .Where(a => a.UserId == Convert.ToInt32(userId))
                    .Include(c => c.Product)
                    .ToListAsync(),
                    TotalPrice = (decimal)(await _db.Carts
                    .Where(a => a.UserId == Convert.ToInt32(userId))
                    .Include(c => c.Product)
                    .ToListAsync()).Sum(c => c.Quantity * c.Product.Price)
                };
                return View("OrderView", orderViewModel);
            }

            //coupon

            string couponCode = HttpContext.Session.GetString("cpcode");

            var coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.Code == couponCode);

            decimal discountAmount = 0;
            if (coupon != null && totalOrderAmount >= coupon.Minamount)
            {
                discountAmount = coupon.Discount;
            }

            //if there is coupon code user selcted
            if (couponCode!=null)
            {
                // Create a new Order object
                var ordere = new OrderE
                {
                    UserId = userId,
                    ShippingAddressId = (int)addressId,
                    ProductId = productId, // Assign the ProductId
                    OrderAmount = (int)totalOrderAmount,
                    Status = OrderStatus.Pending,
                    PaymentMethod = PaymentMethod.Wallet,
                    PaymentStatus = PaymentStatus.Paid,
                    OrderDate = DateTime.Now,
                    Amount = (int)discountAmount,
                    CouponCod = couponCode,
                    DiscountTotal = (int)totalOrderAmount - coupon.Discount
                };



                // Add the order to the database
                _db.Orders.Add(ordere);
                await _db.SaveChangesAsync();

                // Create and store OrderedItems for each cart item
                foreach (var cartItem in cartItems)
                {
                    var orderedItem = new OrderedItem
                    {
                        OrderId = ordere.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        ProductPrice = (int)cartItem.Product.Price,
                        // You may also want to copy other properties from the cart item
                    };

                    _db.OrderedItems.Add(orderedItem);
                }
                // Retrieve the wallet from the database based on the user ID
                var wallete = await _db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);

                // Check if the wallet exists
                if (wallete == null)
                {

                    return RedirectToAction("WalletError");
                }

                wallete.Balance -= (int)totalOrderAmount;

                // Create a new transaction record for the withdrawal
                var withdrawalTransactione = new Transaction
                {
                    Amount = (int)totalOrderAmount - coupon.Discount,
                    Type = "Withdrawal",
                    OrderDate = DateTime.Now,
                    WalletId = wallete.Id,
                    statuss = true
                };

                // Add the transaction to the database
                _db.Transactions.Add(withdrawalTransactione);

                await _db.SaveChangesAsync();

                // Clear the user's cart after placing the order
                _db.Carts.RemoveRange(cartItems);
                await _db.SaveChangesAsync();

                // Redirect to the Success view
                return RedirectToAction("Success");
            }


            //if there is no coupon code 

            // Create a new Order object
            var order = new OrderE
            {
                UserId = userId,
                ShippingAddressId = (int)addressId,
                ProductId = productId, // Assign the ProductId
                OrderAmount = (int)totalOrderAmount,
                Status = OrderStatus.Pending,
                PaymentMethod = PaymentMethod.Wallet,
                PaymentStatus = PaymentStatus.Paid,
                OrderDate = DateTime.Now,
                Amount = 0,
                CouponCod = "null",
                DiscountTotal = 0
            };



            // Add the order to the database
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // Create and store OrderedItems for each cart item
            foreach (var cartItem in cartItems)
            {
                var orderedItem = new OrderedItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    ProductPrice = (int)cartItem.Product.Price,
                    // You may also want to copy other properties from the cart item
                };

                _db.OrderedItems.Add(orderedItem);
            }
            // Retrieve the wallet from the database based on the user ID
            var wallet = await _db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);

            // Check if the wallet exists
            if (wallet == null)
            {
               
                return RedirectToAction("WalletError");
            }

            wallet.Balance -= (int)totalOrderAmount;

            // Create a new transaction record for the withdrawal
            var withdrawalTransaction = new Transaction
            {
                Amount = (int)totalOrderAmount,
                Type = "Withdrawal",
                OrderDate = DateTime.Now,
                WalletId = wallet.Id,
                statuss= true
            };

            // Add the transaction to the database
            _db.Transactions.Add(withdrawalTransaction);

            await _db.SaveChangesAsync();

            // Clear the user's cart after placing the order
            _db.Carts.RemoveRange(cartItems);
            await _db.SaveChangesAsync();

            // Redirect to the Success view
            return RedirectToAction("Success");


        }

        public IActionResult Success()
        {
            return View();
        }


        public async Task<IActionResult> Invoice(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");

            var UserId = Convert.ToInt32(userId);
            // Retrieve the order details from the database based on the user ID
            var order = await _db.Orders
                .Include(o => o.UserSignUp)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.UserId == UserId && id == o.Id);

            if (order == null)
            {
                return NotFound();
            }

            // Assuming OrderItem is a class in your ViewModel namespace that matches the structure you need for your view
            var orderItems = await _db.OrderedItems
                .Include(c => c.Product)
                .Where(c => c.OrderId == id)
                .Select(c => new E_commercePro.ViewModel.OrderItem
                {
                    ProductName = c.Product.Name,
                    Price = c.Product.Price,
                    Quantity = c.Quantity
                })
                .ToListAsync();

            // Create a ViewModel to hold the order details
            var invoiceViewModel = new InvoiceViewModel
            {
                Order = order,
                User = order.UserSignUp,
                ShippingAddress = order.ShippingAddress,
                OrderItems = orderItems // Now this should work
            };

            // Pass the ViewModel to the view
            return View(invoiceViewModel);

        }




    }
}
