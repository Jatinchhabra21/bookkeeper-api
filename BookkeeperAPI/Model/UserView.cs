using BookkeeperAPI.Entity;

namespace BookkeeperAPI.Model
{
    public class UserView
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public UserPreference Preferences { get; set; } = new UserPreference();
    }
}
