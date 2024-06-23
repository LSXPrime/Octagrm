using System.IdentityModel.Tokens.Jwt;
using Octagram.Application.DTOs;
using Octagram.Domain.Entities;
using Octagram.Domain.Repositories;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Octagram.Application.Interfaces;

namespace Octagram.Application.Services;

public class AuthService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, IConfiguration configuration)
    : IAuthService
{
    /// <summary>
    /// Authenticates a user and generates a JWT token if successful.
    /// </summary>
    /// <param name="login">The login credentials.</param>
    /// <returns>The JWT token DTO containing the access token, and expiration time, or null if authentication failed.</returns>
    public async Task<JwtTokenDto?> LoginUserAsync(LoginDto login)
    {
        var user = await userRepository.GetByUsernameAsync(login.Username);
        if (user == null || !VerifyPassword(login.Password, user.PasswordHash))
        {
            return null;
        }

        var token = GenerateJwtToken(user.Id, user.Username, "User");

        return token;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="newUser">The new user registration details.</param>
    /// <returns>True if registration was successful, false otherwise.</returns>
    public async Task<bool> RegisterAsync(RegisterDto newUser)
    {
        if (await userRepository.UserExistsAsync(newUser.Username))
        {
            return false;
        }

        if (await userRepository.GetByEmailAsync(newUser.Email) != null)
        {
            return false;
        }

        var user = new User
        {
            Username = newUser.Username,
            Email = newUser.Email,
            PasswordHash = HashPassword(newUser.Password)
        };

        await userRepository.AddAsync(user);
        return true;
    }

    /// <summary>
    /// Generates a JWT and refresh token for a given user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="userName">The username of the user.</param>
    /// <param name="role">The role of the user.</param>
    /// <returns>The JWT token DTO containing the access token, refresh token, and expiration times.</returns>
    public JwtTokenDto GenerateJwtToken(int userId, string userName, string role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(configuration.GetSection("Jwt:Secret").Value!);
        var accessTokenExpiration = int.Parse(configuration.GetSection("Jwt:AccessTokenExpirationInDays").Value!);
        var refreshTokenExpiration = int.Parse(configuration.GetSection("Jwt:RefreshTokenExpirationInDays").Value!);

        // Access Token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, userName),
                new(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow.AddDays(accessTokenExpiration),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var accessToken = tokenHandler.CreateToken(tokenDescriptor);
        var refreshToken = GenerateRefreshToken();
        var refreshExpirationDate = DateTime.UtcNow.AddDays(refreshTokenExpiration);

        // Store the refresh token in the database
        refreshTokenRepository.AddAsync(new RefreshToken { UserId = userId, Token = refreshToken, Expires = refreshExpirationDate, Role = role });
        
        return new JwtTokenDto
        {
            AccessToken = tokenHandler.WriteToken(accessToken),
            AccessTokenExpiration = tokenDescriptor.Expires.Value,
            RefreshToken = refreshToken,
            RefreshTokenExpiration = refreshExpirationDate
        };
    }
    
    /// <summary>
    /// Generates a random refresh token.
    /// </summary>
    /// <returns>The generated refresh token.</returns>
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Validates a JWT token and returns the associated ClaimsPrincipal if valid.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>The ClaimsPrincipal associated with the token, or null if the token is invalid.</returns>
    public Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration.GetSection("Jwt:Secret").Value!);
            var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = true
            }, out _);

            return Task.FromResult(claimsPrincipal)!;
        }
        catch
        {
            return Task.FromResult<ClaimsPrincipal>(null!)!;
        }
    }

    /// <summary>
    /// Hashes a password using PBKDF2 with HMACSHA256.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>The hashed password as a base64-encoded string.</returns>
    private string HashPassword(string password)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.ASCII.GetBytes(configuration.GetSection("Jwt:Salt").Value!),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
    }

    /// <summary>
    /// Verifies a password against a hashed password.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="hashedPassword">The hashed password to compare against.</param>
    /// <returns>True if the passwords match, false otherwise.</returns>
    private bool VerifyPassword(string password, string hashedPassword)
    {
        return HashPassword(password) == hashedPassword;
    }
}