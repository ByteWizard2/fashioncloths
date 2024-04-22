using E_commercePro.Models;

namespace E_commercePro.ViewModel
{
    public class OrderListViewModel
    {
        public OrderE Order { get; set; }
        public Product Product { get; set; }
        public Address ShippingAddress { get; set; }

        public List<Rating> ProductRatings { get; set; } // Add this property

        public bool IsReturnPeriod { get; internal set; }
        public UserSignUp User { get; set; }
        public Cart Cart { get; set; }
        public decimal TotalPrice { get; internal set; }
        internal List<OrderDetailViewModel> Orders { get; set; }
    }
}
