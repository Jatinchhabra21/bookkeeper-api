namespace BookkeeperAPI.Entity
{
    #region usings
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    #endregion

    [Table("users")]
    public class User
    {
        [Column("id")]
        [Key]
        public Guid Id { get; set; }

        [Column(name: "preference", TypeName = "jsonb")]
        [Required]
        public UserPreference? Preferences { get; set; }


        public UserCredential? Credential { get; set; }
        
        public IEnumerable<Transaction>? Transactions { get; set; }

        public IEnumerable<Income>? Income { get; set; }
    }
}
