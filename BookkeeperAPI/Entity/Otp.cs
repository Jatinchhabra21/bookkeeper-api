﻿namespace BookkeeperAPI.Entity
{
    #region usings
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    #endregion

    [Table("otp")]
    public class OtpRecord
    {

        [Key]
        [Column("id")]
        public int Id { get; set; }

        [EmailAddress]
        [Required]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("otp")]
        public int Otp { get; set; }

        [Required]
        [Column("expiration_time")]
        public DateTime ExpirationTime { get; set; }
    }
}
