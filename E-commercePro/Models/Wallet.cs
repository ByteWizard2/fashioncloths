using System.ComponentModel.DataAnnotations.Schema;

namespace E_commercePro.Models
{
    public class Wallet
    {
        public int Id { get; set; }

        public int Balance { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserSignUp User { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}
