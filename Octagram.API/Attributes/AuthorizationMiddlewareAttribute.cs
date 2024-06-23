using Microsoft.AspNetCore.Mvc.Filters;
using Octagram.API.Middlewares;
using Octagram.Application.Interfaces;
using Octagram.Domain.Repositories;

namespace Octagram.API.Attributes;

/// <summary>
/// Attribute for authorizing access to an action.
/// </summary>
/// <param name="allowAnonymous">Indicates whether anonymous access is allowed.</param>
/// <param name="roles">The roles that are authorized to access the action.</param>
public class AuthorizeMiddlewareAttribute(bool allowAnonymous, params string[] roles) : Attribute, IAsyncActionFilter
{
    public AuthorizeMiddlewareAttribute(params string[] roles) : this(false, roles)
    {
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var authService = httpContext.RequestServices.GetRequiredService<IAuthService>();
        var userRepository = httpContext.RequestServices.GetRequiredService<IUserRepository>();
        var roleRepository = httpContext.RequestServices.GetRequiredService<IRoleRepository>();
        var middleware = new AuthorizationMiddleware(async _ => await next(), authService, userRepository, roleRepository, roles, allowAnonymous);
        await middleware.InvokeAsync(httpContext);
    }
}