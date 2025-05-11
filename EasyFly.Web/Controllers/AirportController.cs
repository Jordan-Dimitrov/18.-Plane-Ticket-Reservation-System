using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyFly.Web.Controllers
{
    public class AirportController : BaseController
    {
        private const int _Size = 30;
        private readonly IAirportService _AirportService;

        public AirportController(IAirportService airportService, IAuditService auditService) : base(auditService)
        {
            _AirportService = airportService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid airportId)
        {
            var response = await _AirportService.GetAirport(airportId);

            if (!response.Success)
            {
                ViewBag.ErrorMessage = response.ErrorMessage;
                return View();
            }

            return View(response.Data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AirportDto dto, Guid airportId)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data";
                return RedirectToAction("Edit", new { airportId = airportId });
            }

            var response = await _AirportService.UpdateAirport(dto, airportId);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Successfully updated";
            }

            return RedirectToAction("GetAirports", new { page = 1 });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AirportDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data";
                return RedirectToAction("GetAirports", new { page = 1 });
            }

            var response = await _AirportService.CreateAirport(dto);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Successfully created";
            }

            return RedirectToAction("GetAirports", new { page = 1 });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remove(Guid airportId)
        {
            var response = await _AirportService.GetAirport(airportId);

            if (!response.Success)
            {
                ViewBag.ErrorMessage = response.ErrorMessage;
                return View();
            }

            return View(response.Data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid airportId)
        {
            var response = await _AirportService.DeleteAirport(airportId);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Successfully deleted";
            }

            return RedirectToAction("GetAirports", new { page = 1 });
        }

        public IActionResult Error()
        {
            string errorMessage = (string)TempData["ErrorMessage"];

            ViewBag.ErrorMessage = errorMessage;

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAirports([FromQuery] int page = 1)
        {
            var response = await _AirportService.GetAirportsPaged(page, _Size);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
                return RedirectToAction("Error");
            }

            string success = (string)TempData["Success"];

            ViewBag.Success = success;

            string error = (string)TempData["ErrorMessage"];

            ViewBag.ErrorMessage = error;

            response.Data.PageNumber = page;

            return View(response.Data);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Reservation()
        {
            var airportsResponse = await _AirportService.GetAirportsPaged(1, int.MaxValue);
            if (!airportsResponse.Success)
            {
                TempData["ErrorMessage"] = airportsResponse.ErrorMessage;
                return RedirectToAction("Error");
            }

            var model = new ReservationViewModel
            {
                Airports = airportsResponse.Data.Airports.ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Reservation(ReservationDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data";
                return RedirectToAction("Reservation");
            }

            return RedirectToAction("SelectFlight", "Flight",
                new
                {
                    departureAirportId = model.DepartureAirportId,
                    arrivalAirportId = model.ArrivalAirportId,
                    requiredSeats = model.NumberOfTickets,
                    departure = model.Departure
                });
        }
    }
}