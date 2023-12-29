using BookkeeperAPI.Entity;
using BookkeeperAPI.Model;

namespace BookkeeperAPI.Service.Interface
{
    public interface IUserService
    {
        public Task<UserView> GetUserByIdAsync(Guid userId);

        public Task<UserView> CreateNewUserAsync(CreateUserRequest request);

        public Task<UserView> UpdateUserPreferenceAsync(Guid userId, UserPreference preference);

        public Task UpdatePasswordAsync(Guid userId, bool isValidUserId, UpdatePasswordRequest request);

        public Task DeleteUserAsync(Guid userId);

        public Task SaveOtpAsync(string email, string otp);

        public Task<bool> ValidateOtpAsync(string email, string otp);

        public Task SendOtpEmailAsync(string email, string body, string subject);
    }
}
