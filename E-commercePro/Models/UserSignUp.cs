    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using System.ComponentModel.DataAnnotations;

    namespace E_commercePro.Models
    {
        public class UserSignUp
        {
            ﻿
                [Key]
                public int ID { get; set; }

                [Required(ErrorMessage = "Please enter Name")]
                public string Name { get; set; }

                [Required(ErrorMessage = "Please enter UserName")]

                public string UserName { get; set; }

                [Required(ErrorMessage = "Please enter Email")]
                [DataType(DataType.EmailAddress)]
                [EmailAddress(ErrorMessage = "Invalid Email Address")]
                public string Email { get; set; }

                [Required(ErrorMessage = "Please enter Password")]

                public string Password { get; set; }

                [Required(ErrorMessage = "Please enter Confirm Passowrd")]
                [Compare("Password", ErrorMessage = ("Confirm Password can't matched!"))]
                [Display(Name = "ConformPassword")]
                public string ConformPassword { get; set; }


                 public bool IsBlocked { get; set; }

                [Required(ErrorMessage = "Please enter Phone Number")]
                [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits")]
                public string Phone { get; set; }


            [ValidateNever]
            public ICollection<Address> Addresses { get; set; }




        }
    }

