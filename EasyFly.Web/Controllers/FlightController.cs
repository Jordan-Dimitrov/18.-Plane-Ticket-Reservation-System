using Microsoft.AspNetCore.Mvc;

namespace EasyFly.Web.Controllers
{
    public class FlightController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
