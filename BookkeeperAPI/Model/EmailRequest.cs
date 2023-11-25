using System.ComponentModel.DataAnnotations;

namespace BookkeeperAPI.Model
{
    public class EmailRequest
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
