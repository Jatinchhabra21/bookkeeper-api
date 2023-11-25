namespace BookkeeperAPI.Model
{
    using System.ComponentModel;
    #region usings
    using System.ComponentModel.DataAnnotations;
    #endregion

    public class UpdatePasswordRequest
    {
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(6)]
        [RegularExpression("(^[0-9]{6}$)")]
        public string? Otp { get; set; }

        [PasswordPropertyText(true)]
        [MinLength(8)]
        public string? OldPassword { get; set; }

        [PasswordPropertyText(true)]
        [MinLength(8)]
        public string NewPassword { get; set; } = String.Empty;
    }
}
