using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commercePro.Models
{
    public class Product
    {
        internal DateTime CreatedAt;

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter Description")]
        public string Description { get; set; }


        [Required(ErrorMessage = "Please enter Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative value")]
        public double Price { get; set; }


        [Required(ErrorMessage = "Please enter color")]
        public string color { get; set; }

        [Required(ErrorMessage = "Please enter Size")]
        public string Size { get; set; }

        [Required]

        public bool islisted { get; set; }


        [Required(ErrorMessage = "Please enter stock")]

        public int stock { get; set; }

        [Required(ErrorMessage = "Please select category")]
        public int CategoryId { get; set; }

        
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public category category { get; set; }

        [ValidateNever]
        public List<string> ImageUrls { get; set; }

        public decimal DiscountedPrice { get; set; }

        [ValidateNever]
        public virtual ICollection<Rating> Ratings { get; set; }
        
    }
}
