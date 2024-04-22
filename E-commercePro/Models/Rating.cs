using System.ComponentModel.DataAnnotations.Schema;

namespace E_commercePro.Models
{
    public class Rating
    {
        public int Id { get; set; }

        public int Stars { get; set; }

        public DateTime RatingTimestamp { get; set; }

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserSignUp UserSignUp { get; set; }

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
    }
}
