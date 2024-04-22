using System.ComponentModel.DataAnnotations;

namespace E_commercePro.Models
{
    public class UserLogin
    {

       
        
            [Required(ErrorMessage = "Please enter Email")]
            public string Email { get; set; }


            [Required(ErrorMessage = "Please enter Password")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

        
    }
}

