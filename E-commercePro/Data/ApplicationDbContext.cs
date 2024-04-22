using E_commercePro.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commercePro.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<UserSignUp> Sign_Up { get; set; }
        public DbSet<OtpDetails> OtpDetails { get; set; }
        public DbSet<category> Categories { get; set; }
        public DbSet<ProductModel> ProductsModel { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<OrderE> Orders { get; set; }
        public IEnumerable<object> OrderItems { get; internal set; }

        public DbSet<OrderedItem> OrderedItems { get; set; }

        public DbSet<whishlist> whishlist { get; set; }

        public DbSet<Wallet> Wallets { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

         public DbSet<CouponsList> Coupons { get; set; }

        public DbSet<Offer> Offers { get; set; }

        public DbSet<Rating> Ratings { get; set; }

        public DbSet<SalesChartData> SalesChartData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Existing configuration for the relationship between UserSignUp and Address
            modelBuilder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade); // If a user is deleted, delete their addresses as well

            // Configuration for the delete behavior of the UserId foreign key in the Order table
            modelBuilder.Entity<OrderE>()
                .HasOne(o => o.UserSignUp)
                .WithMany() // Assuming UserSignUp does not have a collection navigation property for Orders
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Or .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OrderedItem>()
              .HasOne(oi => oi.Product)
              .WithMany()
              .HasForeignKey(oi => oi.ProductId)
              .OnDelete(DeleteBehavior.Restrict);

           




            modelBuilder.Entity<OrderE>()
                .Property(o => o.PaymentMethod)
                .HasConversion<string>();

            modelBuilder.Entity<OrderE>()
                .Property(o => o.PaymentStatus)
                .HasConversion<string>();

            modelBuilder.Entity<OrderE>()
               .Property(o => o.Status)
               .HasConversion<string>();




        }

     
    }
}
