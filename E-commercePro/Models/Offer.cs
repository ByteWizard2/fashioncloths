using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commercePro.Models
{
    public class Offer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter Name")]
        public string name { get; set; }

        [Required(ErrorMessage = "Please enter Description")]
        public string description { get; set; }

        [Required(ErrorMessage = "Please add Banner")]

        [ValidateNever]
        public string banner { get; set; }

        [Required(ErrorMessage = "Please enter Dicount")]
        public float discount { get; set; }

        [Required(ErrorMessage = "Please give the Expirydate")]
        public DateTime expirydate { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public category category { get; set; }
         
        
    
    }

}
