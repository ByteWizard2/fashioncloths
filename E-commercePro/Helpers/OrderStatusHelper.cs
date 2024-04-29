using E_commercePro.Enum;

namespace E_commercePro.Helpers
{
    public static class OrderStatusHelper
    {
        public static bool IsOrderCancelled(OrderStatus status)
        {
            return status == OrderStatus.Cancelled;
        }
    }
}