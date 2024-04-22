using System.ComponentModel.DataAnnotations;

namespace E_commercePro.ViewModel
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = " Current Password")]
        public string CurrebtPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = " New Password")]
        public string NewPassword { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = " Conform new Password")]
        [Compare("NewPassword", ErrorMessage ="confirm new Password does not match")]
        public string ConfirmPassword { get; set;}


    }
}
