using Microsoft.AspNetCore.Mvc;

namespace EasyFly.Web.Controllers
{
    [Route("Error")]
    public class ErrorController : Controller
    {
        [Route("")]
        public IActionResult ServerError()
        {
            Response.StatusCode = 500;
            return View("ServerError");
        }

        [Route("{statusCode}")]
        public IActionResult HandleStatusCode(int statusCode)
        {
            string viewName;
            string message;

            switch (statusCode)
            {
                case 404:
                    viewName = "NotFound";
                    message = "Sorry, the page you’re looking for doesn’t exist.";
                    break;
                case 403:
                    viewName = "AccessDenied";
                    message = "You don’t have permission to access this page.";
                    break;
                default:
                    viewName = "GenericError";
                    message = "An unexpected error occurred.";
                    break;
            }

            ViewBag.ErrorMessage = message;
            Response.StatusCode = statusCode;
            return View(viewName);
        }
    }
}
