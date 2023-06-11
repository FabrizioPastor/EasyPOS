using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Web.API.Commons.Http;

namespace Web.API.Controllers;

public class ApiController: ControllerBase
{
    protected IActionResult Problem(List<Error> errors)
    {
        if (errors.Count is 0)
            return Problem();
        if (errors.All(error => error.Type == ErrorType.Validation))
            return ValidationProblem(errors);
        HttpContext.Items[HttpContextItemKeys.Errors] = errors;

        return Problem(errors[0]);
    }

    private IActionResult Problem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Conflict => StatusCode.Status409Conflict,
            ErrorType.Validation => StatusCode.Status400BadRequest,
            ErrorType.NotFound => StatusCode.Status404NotFound,
            _ => StatusCode.Status500InternalServerError,
        };
        return Problem(statusCode: statusCode, title: error.Description);
    }
    
    private IActionResult ValidationProblem()
}