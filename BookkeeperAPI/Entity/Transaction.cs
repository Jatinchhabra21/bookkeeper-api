namespace BookkeeperAPI.Entity
{
    using System.ComponentModel.DataAnnotations;
    #region usings
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization;
    using BookkeeperAPI.Constants;
    #endregion

    [Table("transaction")]
    public class Transaction
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("date")]
        [Required]
        public DateTime Date { get; set; }

        [Column("name")]
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Column("category")]
        [Required]
        public ExpenseCategory Category { get; set; }

        [Column("amount")]
        [Required]
        public double Amount { get ; set; }

        [Column("type")]
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TransactionType Type { get; set; }
        
        
        [Column("user_id")]
        [Required]
        public Guid UserId { get; set; }


        public User User { get; set; } = new User();
    }
}
