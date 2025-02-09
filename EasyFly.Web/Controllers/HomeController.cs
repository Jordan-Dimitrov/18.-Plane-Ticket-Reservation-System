using EasyFly.Application.Abstractions;
using EasyFly.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mime;

namespace EasyFly.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IQrCodeService _QrCodeService;

        public HomeController(ILogger<HomeController> logger, IQrCodeService qrCodeService)
        {
            _logger = logger;
            _QrCodeService = qrCodeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Test()
        {
            var temp = _QrCodeService.GenerateQRCode("https://www.youtube.com/live/fXyMdXhsSb4?si=EuU4Zckgv63EZkFQ", 500);
            return File(temp, "image/jpeg");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
