using E_commercePro.Models;

namespace E_commercePro.ViewModel
{
    internal class OrderDetailViewModel
    {
        public OrderE Order { get; set; }
        public Product Product { get; set; }
        public Address Address { get; set; }
        public ICollection<OrderedItem> OrderedItems { get; internal set; }
        public bool IsReturnPeriod { get; internal set; }

        public bool IsOrderCancelled { get; set; }

    }
}