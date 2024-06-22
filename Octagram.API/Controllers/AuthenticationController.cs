using Microsoft.AspNetCore.Mvc;
using Octagram.Application.DTOs;
using Octagram.Application.Interfaces;

namespace Octagram.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(IAuthService authService) : ControllerBase
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
        if (!ModelState.IsValid) 
        {
            return BadRequest(ModelState); 
        }

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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); 
        }

        var token = await authService.LoginUserAsync(login);

        if (token != null)
        {
            return Ok(token);
        }

        return Unauthorized("Invalid username or password."); 
    }
}