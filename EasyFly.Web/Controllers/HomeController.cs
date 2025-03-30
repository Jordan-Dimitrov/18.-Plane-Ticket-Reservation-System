using EasyFly.Application.Abstractions;
using EasyFly.Application.ViewModels;
using EasyFly.Domain.Enums;
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
        private readonly ITicketService _TickerService;
        private readonly IUserService _UserService;
        public HomeController(ILogger<HomeController> logger, IQrCodeService qrCodeService,
            IUserService userService, ITicketService ticketService)
        {
            _logger = logger;
            _QrCodeService = qrCodeService;
            _TickerService = ticketService;
            _UserService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var homeViewModel = new HomeViewModel();

            var userResponse = await _UserService.GetUserCount();
            var ticketResponse = await _TickerService.GetTicketCount();

            homeViewModel.RegisteredUsersCount = userResponse.Data;
            homeViewModel.TicketCount = ticketResponse.Data;

            if (User.IsInRole(Role.Admin.ToString()))
            {
                return RedirectToAction("GetTickets", "Ticket", new { page = 1 });
            }
            else if (User.IsInRole(Role.User.ToString()))
            {
                return RedirectToAction("Reservation", "Airport");
            }

            return View(homeViewModel);
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
