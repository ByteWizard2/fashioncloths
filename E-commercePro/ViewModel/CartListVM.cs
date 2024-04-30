using E_commercePro.Models;

namespace E_commercePro.ViewModel
{

    public class CartListVM
    {
        public IEnumerable<Cart> productList { get; set; }

        public IEnumerable<CouponsList> coupon { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Subtotal { get; internal set; }

    }
}
