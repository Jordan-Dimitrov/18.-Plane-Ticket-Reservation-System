using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Application.ViewModels;
using System;
using System.Threading.Tasks;
using EasyFly.Application.Responses;
using EasyFly.Infrastructure.Services;
using Stripe;

namespace EasyFly.Web.Controllers
{
    public class FlightController : BaseController
    {
        private const int _Size = 30;
        private readonly IFlightService _FlightService;
        private readonly IAirportService _AirportService;
        private readonly IPlaneService _PlaneService;

        public FlightController(IFlightService flightService, IAirportService airportService,
            IAuditService auditService, IPlaneService planeService)
            : base(auditService)
        {
            _FlightService = flightService;
            _AirportService = airportService;
            _PlaneService = planeService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Guid? planeId)
        {
            ViewBag.PlaneId = planeId;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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

            return RedirectToAction("GetPlanes", "Plane", new { page = 1 });
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetFlights([FromQuery] Guid? planeId, [FromQuery] int page = 1)
        {
            ViewBag.PlaneId = planeId;

            DataResponse<FlightPagedViewModel> response;
            if (planeId.HasValue)
            {
                response = await _FlightService.GetFlightsPagedByPlane(planeId.Value, page, _Size);
            }
            else
            {
                response = await _FlightService.GetFlightsPaged(page, _Size);
            }

            var airports = await _AirportService.GetAirportsPaged(1, int.MaxValue);

            var planesResp = await _PlaneService.GetPlanesPaged(1, int.MaxValue);

            if (!response.Success || !airports.Success || !planesResp.Success)
            {
                TempData["ErrorMessage"] = "Unable to load flights, airports or planes.";
                return RedirectToAction("Error");
            }

            ViewBag.Success = (string)TempData["Success"];
            ViewBag.ErrorMessage = (string)TempData["ErrorMessage"];

            response.Data.Airports = airports.Data.Airports;
            response.Data.Planes = planesResp.Data.Planes;
            response.Data.SelectedPlaneId = planeId;
            response.Data.PageNumber = page;

            return View(response.Data);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SelectFlight(Guid departureAirportId, Guid arrivalAirportId, int requiredSeats, DateOnly departure)
        {
            var flightsResponse = await _FlightService
                .GetFlightsPagedByArrivalAndDepartureAsync(departureAirportId, arrivalAirportId, departure.ToDateTime(TimeOnly.MinValue), requiredSeats, 1, _Size);

            if(flightsResponse.Data.Flights == null)
            {
                ViewBag.FoundDate = false;
                flightsResponse = await _FlightService
                    .GeFlightstPagedByArrivalAndDepartureAirportsAsync(departureAirportId, arrivalAirportId, requiredSeats, 1, _Size);
            }

            if (flightsResponse.Data.Flights == null)
            {
                TempData["ErrorMessage"] = "No flights available for the selected route.";
                return RedirectToAction("Error");
            }

            if (!flightsResponse.Success || flightsResponse.Data.Flights.Count() == 0)
            {
                TempData["ErrorMessage"] = "No flights available for the selected route.";
                return RedirectToAction("Error");
            }

            ViewBag.RequiredSeats = requiredSeats;

            return View(flightsResponse.Data);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReserveTicket(Guid flightId, int requiredSeats, Guid? returningFlightId)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data";
                return RedirectToAction("ReserveTicket");
            }

            return RedirectToAction("EnterTicketDetails", "Ticket", new { flightId = flightId,
                requiredSeats = requiredSeats, returningFlightId = returningFlightId });
        }
    }
}