namespace BookkeeperAPI.Model
{
    #region usings
    using BookkeeperAPI.Entity;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    #endregion

    public class CreateUserRequest
    {
        [MaxLength(100)]
        [Required]
        public string DisplayName { get; set; } = string.Empty;

        public UserPreference UserPreference { get; set; } = new UserPreference()
        {
            DailyReminder = true,
            DefaultCurrency = Constants.Currency.INR,
            DefaultTheme = Constants.Theme.Default
        };

        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;

        [PasswordPropertyText(true)]
        [MinLength(8)]
        [MaxLength(16)]
        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(6)]
        [RegularExpression("(^[0-9]{6}$)")]
        public string Otp { get; set; } = string.Empty;
    }
}
