using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_commercePro.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
  
        public string color { get; set; }

        [Required]
        public string Size { get; set; }

        [Required]
        
        public bool islisted { get; set; }


        [Required]
    
        public int stock { get; set; }


        [Required]
       
        public ICollection<string> ImageUrl { get; set; }

        [Required]
        
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        
        public category Category { get; set; }
    }
}
