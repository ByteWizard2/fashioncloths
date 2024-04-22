using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commercePro.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

       
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserSignUp User { get; set; }


        public int ProductId {  get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
