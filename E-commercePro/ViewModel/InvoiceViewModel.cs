using E_commercePro.Models;

namespace E_commercePro.ViewModel
{
    public class InvoiceViewModel
    {
        public OrderE Order { get; set; }
        public UserSignUp User { get; set; }
        public Address ShippingAddress { get; set; }
        public List<Product> Products { get; set; }
        internal List<OrderItem> OrderItems { get; set; }
    }

}
