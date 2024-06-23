using System.Security.Claims;
using Octagram.Application.Interfaces;
using Octagram.Domain.Repositories;

namespace Octagram.API.Middlewares;

/// <summary>
/// Middleware for handling authorization, validating JWT tokens and checking user permissions.
/// </summary>
public class AuthorizationMiddleware(
    RequestDelegate next,
    IAuthService authService,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    string[]? roles = null,
    bool allowAnonymous = false)
{
    /// <summary>
    /// Invokes the middleware to handle authorization.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            if (allowAnonymous)
            {
                await next(context);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: Missing Authorization header.");
            return;
        }

        var token = authorizationHeader.ToString().Split(' ')[1];

        // Extract claims from the validated token & Create a ClaimsPrincipal based on the validated token claims
        var claimsPrincipal = await authService.ValidateTokenAsync(token);
        if (claimsPrincipal == null)
        {
            // If the token is not valid, but allowAnonymous is true, allow the request
            if (allowAnonymous)
            {
                await next(context);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: Invalid token.");
            return;
        }

        // Set the ClaimsPrincipal on the HttpContext
        context.User = claimsPrincipal;

        // access the claims from context.User.Identity
        var claimsIdentity = context.User.Identity as ClaimsIdentity;
        var userName = claimsIdentity?.FindFirst(ClaimTypes.Name)?.Value;
        var roleClaim = claimsIdentity?.FindFirst(ClaimTypes.Role)?.Value;

        if (roleClaim == null || !await roleRepository.RoleExistsAsync(roleClaim))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden: Insufficient permissions.");
            return;
        }

        if (!await userRepository.UserExistsAsync(userName!))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: User not found.");
            return;
        }

        await next(context);
    }
}