using Microsoft.AspNetCore.Identity;

namespace E_commercePro.Models
{
    public class ApplicationUser : IdentityUser
    {
        
        public int ID { get; internal set; }
        public string Name { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
       
        
    }
}
