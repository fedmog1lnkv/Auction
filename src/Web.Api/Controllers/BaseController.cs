using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace Web.Api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleFailure(Result result)
    {
        var payload = new { error = result.Error.Code, message = result.Error.Description };

        return result.Error.Type switch
        {
            ErrorType.Validation => BadRequest(payload),
            ErrorType.NotFound => NotFound(payload),
            ErrorType.Conflict => Conflict(payload),
            ErrorType.Problem => StatusCode(StatusCodes.Status500InternalServerError, payload),
            _ => Unauthorized(payload)
        };
    }
}