namespace BookkeeperAPI.Data
{
    #region usings
    using BookkeeperAPI.Constants;
    using BookkeeperAPI.Entity;
    using Microsoft.EntityFrameworkCore;
    #endregion

    public class BookkeeperContext : DbContext
    {
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OtpRecord> Otp { get; set; }
        public DbSet<UserCredential> Credentials { get; set; }
        public DbSet<Income> Income {  get; set; }

        public BookkeeperContext(DbContextOptions<BookkeeperContext> options) 
            :base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Transactions)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Credential)
                .WithOne(c => c.User)
                .HasForeignKey<UserCredential>(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Income)
                .WithOne(i => i.User)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OtpRecord>()
                .HasKey(x => x.Id)
                .HasName("id");

            modelBuilder.Entity<Transaction>()
                .HasKey(x => x.Id)
                .HasName("id");


            modelBuilder.HasPostgresEnum<ExpenseCategory>();
            modelBuilder.HasPostgresEnum<TransactionType>();
            modelBuilder.HasPostgresEnum<IncomeType>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
