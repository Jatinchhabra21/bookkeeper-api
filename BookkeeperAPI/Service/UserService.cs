namespace BookkeeperAPI.Service
{
    #region usings
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Exceptions;
    using BookkeeperAPI.Model;
    using BookkeeperAPI.Repository.Interface;
    using BookkeeperAPI.Service.Interface;
    using Microsoft.IdentityModel.Tokens;
    using System.Net;
    using System.Net.Mail;
    #endregion

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<UserView> GetUserByIdAsync(Guid userId)
        {
            User? result = await _userRepository.GetUserByIdAsync(userId);

            if (result == null)
            {
                throw new HttpOperationException(StatusCodes.Status404NotFound, $"User with id '{userId}' does not exist");
            }

            UserView user = new UserView()
            {
                DisplayName = result.Credential!.DisplayName!,
                Email = result.Credential.Email!,
                Id = result.Id,
                Preferences = result.Preferences!
            };

            return user;
        }

        public async Task<UserView> CreateNewUserAsync(CreateUserRequest request)
        {
            var _ = await _userRepository.GetUserByEmailAsync(request.Email) != null ? throw new HttpOperationException($"User with e-mail '{request.Email}' already exists.") : true;
            User user = new User();
            user.Preferences = request.UserPreference;
            user.Credential = new UserCredential()
            {
                UserId = user.Id,
                DisplayName = request.DisplayName,
                Password = request.Password,
                Email = request.Email,
                LastUpdated = DateTime.UtcNow,
                CreatedTime = DateTime.UtcNow,
            };

            await _userRepository.CreateUserAsync(user);

            return new UserView()
            {
                DisplayName = user.Credential.DisplayName!,
                Email = user.Credential.Email!,
                Id = user.Id,
                Preferences = user.Preferences!
            };
        }

        public async Task<UserView> UpdateUserPreferenceAsync(Guid userId, UserPreference preference)
        {
            User? user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new HttpOperationException(StatusCodes.Status404NotFound, $"User with id '{userId}' does not exist");
            }

            user.Preferences = preference;
            await _userRepository.SaveChangesAsync();

            return new UserView()
            {
                DisplayName = user.Credential!.DisplayName!,
                Email = user.Credential.Email!,
                Id = user.Id,
                Preferences = user.Preferences
            };
        }

        public async Task UpdatePasswordAsync(Guid userId, bool isValidUserId, UpdatePasswordRequest request)
        {
            // update password for unauthorized users
            if (!isValidUserId)
            {
                bool isOtpValid = false;
                // check if otp, email and new password is present in request
                if (!request.Otp.IsNullOrEmpty() && !request.Email.IsNullOrEmpty() && !request.NewPassword.IsNullOrEmpty())
                {
                    isOtpValid = await ValidateOtpAsync(request.Email!, request.Otp!);
                    // if otp is valid update password
                    if (isOtpValid)
                    {
                        await _userRepository.UpdatePasswordForUnauthorizedUserAsync(request.Email!, request.NewPassword);
                    }
                    // if otp is not valid throw error
                    else
                    {
                        throw new HttpOperationException(StatusCodes.Status400BadRequest, "OTP is invalid");
                    }
                }
                // if not present throw error
                else
                {
                    throw new HttpOperationException(StatusCodes.Status400BadRequest, "Bad request");
                }
            }
            // update password for authorized users
            else
            {
                // check if old password and new password are present in request
                if (!request.OldPassword.IsNullOrEmpty() && !request.NewPassword.IsNullOrEmpty())
                    await _userRepository.UpdatePasswordForAuthorizedUserAsync(userId, request.OldPassword!, request.NewPassword);
                // if old password and new password are not present in request, throw error
                else
                    throw new HttpOperationException(StatusCodes.Status400BadRequest, "Bad request");
            }
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            User? user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new HttpOperationException(StatusCodes.Status404NotFound, $"User with id '{userId}' does not exist");
            }

            await _userRepository.DeleteUserAsync(user);
        }

        public async Task SaveOtpAsync(string email, string otp)
        {
            DateTime expirationTime = DateTime.UtcNow.AddMinutes(2);
            OtpRecord otpRecord = new OtpRecord();
            otpRecord.Email = email;
            otpRecord.Otp = otp;
            otpRecord.ExpirationTime = expirationTime;

            await _userRepository.SaveOtpAsync(otpRecord);
        }

        public async Task<bool> ValidateOtpAsync(string email, string otp)
        {
            return await _userRepository.ValidateOtpAsync(email, otp);
        }

        public async Task SendOtpEmailAsync(string email, string body, string subject)
        {
            string otp = OneTimePassword.Generate();
            body = body.Replace("[User]", email);
            body = body.Replace("[OTP_CODE]", otp.ToString());
            await SaveOtpAsync(email, otp);

            MailMessage message = new MailMessage()
            {
                From = new MailAddress(_configuration[_configuration["Email:Address"]!]!, "Bookkeeper"),
                Subject = subject,
                IsBodyHtml = true,
                Body = body
            };
            message.To.Add(new MailAddress(email));
            await EmailService.SendEmail(new LoginCredential()
            {
                Email = _configuration[_configuration["Email:Address"]!],
                Password = _configuration[_configuration["Email:Password"]!]
            }, message);
        }
    }
}
