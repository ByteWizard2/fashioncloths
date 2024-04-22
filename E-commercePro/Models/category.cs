using System.ComponentModel.DataAnnotations;

namespace E_commercePro.Models
{
    public class category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please write the description")]
        public string Description { get; set; }


        public bool IsListed { get; set; }
    }
}
