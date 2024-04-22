using System.ComponentModel.DataAnnotations.Schema;

namespace E_commercePro.Models
{
    public class whishlist
    {
        public int id { get; set; }

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserSignUp UserSignUp { get; set; }


        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
    }
}
