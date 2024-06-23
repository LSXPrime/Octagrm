using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Octagram.API.Filters;

public class ModelStateValidationFilter : IActionFilter
{
    /// <summary>
    /// This method is called before the action method is invoked. It checks if the ModelState is valid.
    /// If the ModelState is not valid, it creates a problem details response with the model state errors and sets the result of the context to a BadRequestObjectResult.
    /// </summary>
    /// <param name="context">The context of the action being executed.</param>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            // Create a problem details response with the model state errors
            var problemDetails = new ValidationProblemDetails(context.ModelState);
            context.Result = new BadRequestObjectResult(problemDetails);
        }
    }

    /// <summary>
    /// This method is called after the action method is invoked. But it's not used in this case.
    /// </summary>
    /// <param name="context">The context of the action being executed.</param>
    public void OnActionExecuted(ActionExecutedContext context) { }
}