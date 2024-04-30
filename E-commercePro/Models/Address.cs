using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commercePro.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        public String fillname { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits")]
        public string phone { get; set; }

        [Required]
        public String street { get; set; }

        [Required]
        public String locality { get; set; }

        [Required]
        public String district { get; set; }

        [Required]
        public String state { get; set; }


        [MaxLength(6)]
        [Required]
        public int pincode { get; set; }

        [ValidateNever]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public UserSignUp User { get; set; }

    }
}
