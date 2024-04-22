//using E_commercePro.Data;
//using E_commercePro.ViewModel;
//using Microsoft.AspNetCore.Mvc;
//using System.Linq;
//using Microsoft.EntityFrameworkCore;


//using E_commercePro.Data;
//using E_commercePro.ViewModel;
//using Microsoft.AspNetCore.Mvc;
//using System.Linq;
//using Microsoft.EntityFrameworkCore;

using E_commercePro.Models;

namespace E_commercePro.ViewModel
{
    internal class OrderViewModel
    {
        public List<Address> UserAddresses { get; set; }
        public List<Cart> CartItems { get; set; }

        public List<UserSignUp> User { get; set; }

     

        public decimal TotalPrice { get; set; }
        public decimal Discount { get; internal set; }
        public string? CouponCode { get; internal set; }
      
        public bool IsWalletPayment { get; internal set; }
    }
}