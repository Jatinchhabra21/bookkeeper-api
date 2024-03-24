namespace BookkeeperAPI.Controllers;

#region usings

using Exceptions;
using Model;
using Repository.Interface;
using Utility;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

#endregion

[ApiController]
[AllowAnonymous]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;

    public AuthenticationController(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

    [HttpPost("/api/oauth2/token")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResult))]
    [ProducesErrorResponseType(typeof(ErrorResponseModel))]
    public async Task<Ok<AuthenticationResult>> GetToken([FromBody] [Required] LoginCredential credential)
    {
        if (credential == null || credential.Email == null || credential.Password == null)
            throw new HttpOperationException(400, "Bad Request");

        var userInfo = await _userRepository.GetUserByEmailAsync(credential.Email);

        if (userInfo == null) throw new HttpOperationException(400, "Invalid credentials");

        if (!Parser.IsValidPassword(credential.Password, userInfo.Credential!.Password!))
            throw new HttpOperationException(400, "Invalid credentials");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userInfo.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("user_id", userInfo.Id.ToString()),
            new Claim("display_name", userInfo.Credential!.DisplayName!),
            new Claim("user_name", userInfo.Credential.Email!),
            new Claim("nbf", ToUnixEpoch(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64),
            new Claim("iat", ToUnixEpoch(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64)
        };

        var expiresAt = DateTime.UtcNow.AddMinutes(30);
        var rsa = RSA.Create();
        rsa.FromXmlString(
            Encoding.UTF8.GetString(Convert.FromBase64String(_configuration[_configuration["RSA:Key:Private"]!]!)));
        var singIn = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
        var token = new JwtSecurityToken(
            _configuration[_configuration["Jwt:Iss"]!]!,
            _configuration[_configuration["Jwt:Aud"]!]!,
            claims,
            signingCredentials: singIn,
            expires: expiresAt
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return TypedResults.Ok(new AuthenticationResult()
        {
            AccessToken = accessToken,
            ExpiresAt = expiresAt,
            TokenId = Guid.NewGuid(),
            UserName = userInfo.Credential.DisplayName!,
            UserEmail = userInfo.Credential.Email!
        });
    }

    private static long ToUnixEpoch(DateTime date)
    {
        return (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
            .TotalSeconds);
    }
}