using E_commercePro.Models;

namespace E_commercePro.ViewModel
{
    internal class ProductDetailsViewModel
    {
        public Product Product { get; set; }
        public decimal DiscountedPrice { get; set; }
        public bool HasActiveOffer { get; set; }
        public Offer? ActiveOffer { get; internal set; }
    }
}