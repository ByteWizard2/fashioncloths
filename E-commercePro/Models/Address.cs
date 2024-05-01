using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commercePro.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

     
        public String fillname { get; set; }

   
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits")]
        public string phone { get; set; }

     
        public String street { get; set; }

   
        public String locality { get; set; }

    
        public String district { get; set; }

     
        public String state { get; set; }


        public int pincode { get; set; }

        [ValidateNever]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public UserSignUp User { get; set; }

    }
}
