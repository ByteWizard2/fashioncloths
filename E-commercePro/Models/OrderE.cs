using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using E_commercePro.Enum;
using E_commercePro.ViewModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace E_commercePro.Models
{
    public class OrderE
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserSignUp UserSignUp { get; set; }

        [Display(Name = "Shipping Address")]
        public int ShippingAddressId { get; set; }
        [ForeignKey(nameof(ShippingAddressId))]
        public Address ShippingAddress { get; set; }

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        public int OrderAmount { get; set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public ICollection<OrderedItem> OrderedItems { get; set; } // Collection of ordered items

        [ValidateNever]
        public string CouponCod {  get; set; }

        [ValidateNever]
        public int Amount { get; set; }

        [ValidateNever]
        public int DiscountTotal { get; set; }
    }
}
