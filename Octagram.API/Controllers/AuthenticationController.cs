using Microsoft.AspNetCore.Mvc;
using Octagram.Application.DTOs;
using Octagram.Application.Interfaces;
using Octagram.Domain.Repositories;

namespace Octagram.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(IAuthService authService, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository) : ControllerBase
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="newUser">The user registration details.</param>
    /// <returns>
    /// Returns an OK response with a message "User registered successfully." if the registration is successful.
    /// Returns a BadRequest response with a message "Username or email is already taken." if the username or email is already taken.
    /// Returns a BadRequest response with the ModelState if the request is invalid.
    /// </returns>
    [HttpPost("register")] 
    public async Task<IActionResult> Register([FromBody] RegisterDto newUser)
    {
        var result = await authService.RegisterAsync(newUser);

        if (result)
        {
            return Ok("User registered successfully."); 
        }
        return BadRequest("Username or email is already taken.");
    }

    /// <summary>
    /// Logs in an existing user.
    /// </summary>
    /// <param name="login">The user login details.</param>
    /// <returns>
    /// Returns an OK response with the generated jwt token if the login is successful.
    /// Returns an Unauthorized response with a message "Invalid username or password." if the login fails.
    /// Returns a BadRequest response with the ModelState if the request is invalid.
    /// </returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        var token = await authService.LoginUserAsync(login);

        if (token != null)
        {
            return Ok(token);
        }

        return Unauthorized("Invalid username or password."); 
    }
    
    /// <summary>
    /// Refreshes the JWT token using the provided refresh token.
    /// </summary>
    /// <param name="refreshTokenDto">The refresh token to use for token refresh.</param>
    /// <returns>
    /// Returns an OK response with the new JWT token if the refresh is successful.
    /// Returns an Unauthorized response with a message "Invalid refresh token." if the refresh token is invalid.
    /// Returns an Unauthorized response with a message "User associated with refresh token not found." if the user associated with the refresh token is not found.
    /// </returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        var storedRefreshToken = await refreshTokenRepository.GetByTokenAsync(refreshTokenDto.Token);
        if (storedRefreshToken == null || storedRefreshToken.Expires < DateTime.UtcNow)
        {
            return Unauthorized("Invalid refresh token.");
        }
        await refreshTokenRepository.DeleteAsync(storedRefreshToken);
        Console.WriteLine($"Token: {storedRefreshToken.Token} Deleted successfully.");

        var userId = storedRefreshToken.UserId; 
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null) 
        {
            return Unauthorized("User associated with refresh token not found.");
        }

        var newTokens = authService.GenerateJwtToken(userId, user.Username, storedRefreshToken.Role);
        
        return Ok(newTokens);
    }
}