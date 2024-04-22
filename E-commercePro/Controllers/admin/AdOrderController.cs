using E_commercePro.Data;
using E_commercePro.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_commercePro.Enum;


namespace E_commercePro.Controllers.admin
{
    public class AdOrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        

        public AdOrderController(ApplicationDbContext db)
        {
            _db = db;
           
        }
        public IActionResult OrderList()
        {

            var orders = _db.Orders.Include(o => o.UserSignUp).ToList();
            return View(orders);
        }


       public async Task<IActionResult> OrderDetails(int id)
        {
            // Retrieve the order details from the database based on the provided order ID
            var order = await _db.Orders
                .Include(o => o.UserSignUp)
                .Include(o => o.OrderedItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Calculate the total price of the order



            decimal total = (decimal)order.OrderedItems.Sum(oi => oi.Quantity * oi.Product.Price);

            decimal totalPrice = total - order.Amount;
            // Create a view model instance and populate it with order, user, product, and address details
            var viewModel = new OrderListViewModel
            {
                Order = order,
                User = order.UserSignUp,
                Product = order.OrderedItems.Select(oi => oi.Product).FirstOrDefault(),
                ShippingAddress = order.ShippingAddress,
                TotalPrice = totalPrice
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            // Retrieve the order from the database
            var order = await _db.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            // Convert the status string to an OrderStatus enum value
            //if (Enum.TryParse<Enum.OrderStatus>(status, out var orderStatus))
            if( Enum.OrderStatus.TryParse<Enum.OrderStatus>(status, out var orderStatus))
            {
                // Update the order status
                order.Status = orderStatus;

                // Save changes to the database
                await _db.SaveChangesAsync();

                // Redirect back to the order details page
                return RedirectToAction("OrderDetails", new { id = orderId });
            }
            else
            {

               
                ModelState.AddModelError("status", "Invalid order status.");
                return View("OrderDetails", orderId); 
            }
        }



    }
}
