namespace E_commercePro.Enum
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled,
        Returned
    }

    public enum PaymentMethod
    {
        COD,
        Online,
        Wallet,
      
    }

   
    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed,
        Refunded,
    }

    public enum Methode
    {
        deposit,
        withdrawal
    }
}
