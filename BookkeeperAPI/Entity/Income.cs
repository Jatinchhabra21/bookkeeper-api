namespace BookkeeperAPI.Entity
{
    using BookkeeperAPI.Constants;
    #region usings
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    #endregion

    [Table("income")]
    public class Income
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("type")]
        [Required]
        public IncomeType Type { get; set; }

        [Column("amount")]
        [Required]
        public double Amount { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;


        [Column("user_id")]
        public Guid UserId { get; set; }

        public User? User { get; set; }
    }
}
