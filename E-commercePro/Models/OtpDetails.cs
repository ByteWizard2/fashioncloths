using System.ComponentModel.DataAnnotations;

namespace E_commercePro.Models
{
    public class OtpDetails
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string OTP { get; set; }
        public DateTime ExpiryDateTime { get; set; }
    }
}
