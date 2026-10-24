using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TheGameVoice.Web.Controllers;

[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : Controller
{
    // Re-execution retains the original browser URL and HTTP error status.
    // No verb constraint: a missing POST must not turn into a 405 here.
    [Route("error/{statusCode:int}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult HttpStatus(int statusCode)
    {
        // Do not label forbidden/server-error responses as "page not found".
        if (statusCode != StatusCodes.Status404NotFound)
        {
            return StatusCode(statusCode is >= 400 and <= 599
                ? statusCode : StatusCodes.Status404NotFound);
        }

        Response.StatusCode = StatusCodes.Status404NotFound;
        return View("NotFound");
    }
}
