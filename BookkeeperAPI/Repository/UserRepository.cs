namespace BookkeeperAPI.Repository
{
    #region usings
    using BookkeeperAPI.Data;
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Exceptions;
    using BookkeeperAPI.Model;
    using BookkeeperAPI.Repository.Interface;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    #endregion

    public class UserRepository : IUserRepository
    {
        private readonly BookkeeperContext _context;
        public UserRepository(BookkeeperContext context)
        {
            _context = context;
        }


        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users
                .Where(x => x.Id.Equals(userId))
                .Include(x => x.Credential)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByEmailAsync([EmailAddress] string email)
        {
            return await _context.Users
                .Where(x => x.Credential!.Email == email)
                .Include(x => x.Credential)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByEmailAsync(LoginCredential credential)
        {
            return await _context.Users
                .Where(x => x.Credential!.Email == credential.Email && x.Credential.Password == credential.Password)
                .Include(x => x.Credential)
                .FirstOrDefaultAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.Credentials.AddAsync(user.Credential!);

            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task SaveOtpAsync(OtpRecord otp)
        {
            await _context.Otp.AddAsync(otp);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ValidateOtpAsync(string email, string otp)
        {
            OtpRecord? otpRecord = await _context.Otp.Where(x => x.Email == email && x.Otp == otp).FirstOrDefaultAsync();
            List<OtpRecord> records = await _context.Otp.Where(x => x.ExpirationTime > DateTime.UtcNow).ToListAsync();

            if (otpRecord == null)
            {
                return false;
            }

            _context.Otp.Remove(otpRecord);
            _context.Otp.RemoveRange(records);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdatePasswordForUnauthorizedUserAsync(string email, string newPassword)
        {
            UserCredential? credentials = await _context.Credentials.Where(x => x.Email == email).FirstOrDefaultAsync();

            if (credentials == null)
            {
                throw new HttpOperationException(StatusCodes.Status404NotFound, "User does not exist");
            }

            credentials.Password = newPassword;
            credentials.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task UpdatePasswordForAuthorizedUserAsync(Guid userId, string oldPassword, string newPassword)
        {
            UserCredential? credentials = await _context.Credentials.Where(x => x.UserId == userId).FirstOrDefaultAsync();

            if (credentials == null)
            {
                throw new HttpOperationException(StatusCodes.Status404NotFound, "User does not exist");
            }

            if (credentials.Password != oldPassword)
            {
                throw new HttpOperationException(StatusCodes.Status400BadRequest, "Old password does not match");
            }

            credentials.Password = newPassword;
            credentials.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
