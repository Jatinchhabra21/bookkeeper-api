namespace BookkeeperAPI.Controllers
{

    #region usings
    using BookkeeperAPI.Data;
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Exceptions;
    using BookkeeperAPI.Model;
    using BookkeeperAPI.Service;
    using BookkeeperAPI.Service.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.Identity.Client;
    using Microsoft.IdentityModel.Tokens;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Net.Mail;
    using System.Security.Claims;
    #endregion

    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpGet("/api/me/account")]
        public async Task<ActionResult<UserView>> GetUser()
        {
            Guid userId;
            string userIdClaim = HttpContext.User.Claims.Where(x => x.Type == "user_id").First().Value.ToString();
            bool isValidUserId = Guid.TryParse(userIdClaim, out userId);

            if (!isValidUserId)
            {
                throw new HttpOperationException(401, "Unauthorized");
            }

            UserView user = await _userService.GetUserByIdAsync(userId);

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("/api/users/new")]
        public async Task<ActionResult<UserView>> CreateUser([FromBody] CreateUserRequest request)
        {
            bool isOtpValid = await _userService.ValidateOtpAsync(request.Email, request.Otp);
            
            if(!isOtpValid)
            {
                throw new HttpOperationException(StatusCodes.Status400BadRequest, "Invalid Otp");
            }

            UserView user = await _userService.CreateNewUserAsync(request);

            return StatusCode(StatusCodes.Status201Created, user);
        }

        [HttpPatch("/api/me/preference")]
        public async Task<ActionResult<UserView>> UpdateUserPreference(UserPreference preference)
        {
            Guid userId;
            string userIdClaim = HttpContext.User.Claims.Where(x => x.Type == "user_id").First().Value.ToString();
            bool isValidUserId = Guid.TryParse(userIdClaim, out userId);

            if (!isValidUserId)
            {
                throw new HttpOperationException(401, "Unauthorized");
            }

            UserView user = await _userService.UpdateUserPreferenceAsync(userId, preference);

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPatch("/api/me/account/password")]
        public async Task<ActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            Guid userId = Guid.Empty;
            List<Claim> claims = HttpContext.User.Claims.ToList();
            
            bool isValidUserId = claims!.Any() ? Guid.TryParse(claims?.Where(x => x.Type == "user_id").First().Value.ToString(), out userId) : false;

            await _userService.UpdatePasswordAsync(userId, isValidUserId, request);
            return StatusCode(StatusCodes.Status204NoContent, null);
        }

        [HttpDelete("/api/me/account")]
        public async Task<ActionResult> DeleteUser()
        {
            string userIdClaim = HttpContext.User.Claims.Where(x => x.Type == "user_id").First().Value.ToString();
            bool isValidUserId = Guid.TryParse(userIdClaim, out Guid userId);

            if (!isValidUserId)
            {
                throw new HttpOperationException(401, "Unauthorized");
            }

            await _userService.DeleteUserAsync(userId);

            return Ok(new ResponseModel()
            {
                Message = "Your account has been deleted successfully",
                StatusCode = StatusCodes.Status200OK,
            });
        }

        [AllowAnonymous]
        [HttpPost("/api/otp/account/activation")]
        public async Task<ActionResult> GetOtpForAccountActivation([FromBody] [Required] EmailRequest email)
        {
            string body = System.IO.File.ReadAllText("EmailTemplates/AccountActivation.htm");
            await _userService.SendOtpEmailAsync(email.Email, body, "Bookkeeper: Account activation");

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("/api/otp/account/reset")]
        public async Task<ActionResult> GetOtpForResetPassword([FromBody][Required] EmailRequest email)
        {
            string body = System.IO.File.ReadAllText("EmailTemplates/ResetPassword.htm");
            await _userService.SendOtpEmailAsync(email.Email, body, "Bookkeeper: Password reset");

            return Ok();
        }
    }
}
