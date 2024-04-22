using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace E_commercePro.Models
{
    public class CouponsList

    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter Coupn Code")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Please enter Discount")]
        public int Discount { get; set; }

        [Required(ErrorMessage = "Please enter expiryDate")]
        public DateTime expiryDate { get; set; }

        [Required(ErrorMessage = "Please enter Userlimit")]
        public int Userlimit { get; set;}

        [Required(ErrorMessage = "Please enter Minamount")]
        public int Minamount { get; set;}
     
        [Required(ErrorMessage = "Please enter Description")]
        public string Description { get; set;}
    }
}
