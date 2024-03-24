namespace BookkeeperAPI.Model;

public class AuthenticationResult
{
    public string? AccessToken { get; set; }

    public DateTime ExpiresAt { get; set; }

    public Guid TokenId { get; set; }

    public string? UserName { get; set; }

    public string? UserEmail { get; set; }
}