using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Application.ViewModels;
using System;
using System.Threading.Tasks;

namespace EasyFly.Web.Controllers
{
    public class FlightController : Controller
    {
        private const int _Size = 5;
        private readonly IFlightService _FlightService;
        private readonly IAirportService _AirportService;

        public FlightController(IFlightService flightService, IAirportService airportService)
        {
            _FlightService = flightService;
            _AirportService = airportService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create(Guid? planeId)
        {
            ViewBag.PlaneId = planeId;
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid planeId, FlightDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data";
                return RedirectToAction("GetFlights", new { planeId = planeId, page = 1 });
            }

            var response = await _FlightService.CreateFlight(dto);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Successfully created";
            }

            return RedirectToAction("GetFlights", new { planeId = planeId, page = 1 });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remove(Guid flightId)
        {
            var response = await _FlightService.GetFlight(flightId);

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
        public async Task<IActionResult> Delete(Guid flightId)
        {
            var response = await _FlightService.DeleteFlight(flightId);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Successfully deleted";
            }

            return RedirectToAction("GetFlights", new { page = 1 });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid flightId)
        {
            var airports = await _AirportService.GetAirportsPaged(1, int.MaxValue);
            var flight = await _FlightService.GetFlight(flightId);

            if (!flight.Success || !airports.Success)
            {
                ViewBag.ErrorMessage = flight.ErrorMessage;
                return View();
            }

            var response = new FlightEditViewModel()
            {
                Airports = airports.Data.Airports,
                FlightViewModel = flight.Data

            };

            return View(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FlightDto dto, Guid flightId)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data";
                return RedirectToAction("Edit", new { flightId = flightId });
            }

            var response = await _FlightService.UpdateFlight(dto, flightId);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Successfully updated";
            }

            return RedirectToAction("GetPlanes", "Plane", new { page = 1 });
        }

        public IActionResult Error()
        {
            string errorMessage = (string)TempData["ErrorMessage"];

            ViewBag.ErrorMessage = errorMessage;

            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFlights([FromQuery] Guid planeId, [FromQuery] int page = 1)
        {
            if (planeId == Guid.Empty)
            {
                TempData["ErrorMessage"] = "Invalid plane";
                return RedirectToAction("Error");
            }

            var response = await _FlightService.GetFlightsPagedByPlane(planeId, page, _Size);
            var airports = await _AirportService.GetAirportsPaged(1, int.MaxValue);

            if (!response.Success || !airports.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
                return RedirectToAction("Error");
            }

            string success = (string)TempData["Success"];

            ViewBag.Success = success;

            string error = (string)TempData["ErrorMessage"];

            ViewBag.ErrorMessage = error;

            response.Data.Airports = airports.Data.Airports;
            response.Data.PageNumber = page;
            ViewBag.PlaneId = planeId;

            return View(response.Data);
        }
    }
}