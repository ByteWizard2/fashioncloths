using System.ComponentModel.DataAnnotations.Schema;

namespace E_commercePro.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public int Amount { get; set; }

        public string Type { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public int WalletId { get; set; }

        [ForeignKey("WalletId")]
        public Wallet Wallets { get; set; }

        public bool statuss { get; set; }
    }
}
