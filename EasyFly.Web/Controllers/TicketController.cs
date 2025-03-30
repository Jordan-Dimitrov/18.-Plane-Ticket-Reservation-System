using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Application.ViewModels;
using EasyFly.Application.Responses;
using EasyFly.Domain.Models;
using System.Security.Claims;
using System.Net.Sockets;

namespace EasyFly.Web.Controllers
{
    public class TicketController : BaseController
    {
        private const int _Size = 10;
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;
        private readonly ISeatService _seatService;
        private readonly IFlightService _flightService;
        private readonly IPaymentService _PaymentService;
        public TicketController(ITicketService ticketService, IUserService userService,
            ISeatService seatService, IFlightService flightService, IPaymentService paymentService,
            IAuditService auditService) : base(auditService)
        {
            _ticketService = ticketService;
            _userService = userService;
            _seatService = seatService;
            _flightService = flightService;
            _PaymentService = paymentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid ticketId)
        {
            var ticket = await _ticketService.GetTicket(ticketId);
            var users = await _userService.GetUsersPaged(1, int.MaxValue);
            var flights = await _flightService.GetFlightsPaged(1, int.MaxValue);

            var seatsForFlight = await _seatService.GetSeatsPagedForFlight(ticket.Data.FlightId, 1, int.MaxValue);

            if (!ticket.Success || !users.Success || !seatsForFlight.Success)
            {
                ViewBag.ErrorMessage = "Invalid data";
                return View();
            }

            var response = new TicketEditViewModel()
            {
                TicketViewModel = ticket.Data,
                Users = users.Data.Users,
                Seats = seatsForFlight.Data.Seats,
                Flights = flights.Data.Flights
            };

            return View(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TicketDto dto, Guid ticketId)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data";
                return RedirectToAction("Edit", new { ticketId = ticketId });
            }

            var response = await _ticketService.UpdateTicket(dto, ticketId);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Successfully updated";
            }

            return RedirectToAction("GetTickets", new { page = 1 });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data";
                return RedirectToAction("GetTickets", new { page = 1 });
            }

            var response = await _ticketService.CreateTicket(dto);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Successfully created";
            }

            return RedirectToAction("GetTickets", new { page = 1 });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remove(Guid ticketId)
        {
            var response = await _ticketService.GetTicket(ticketId);

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
        public async Task<IActionResult> Delete(Guid ticketId)
        {
            var response = await _ticketService.DeleteTicket(ticketId);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Successfully deleted";
            }

            return RedirectToAction("GetTickets", new { page = 1 });
        }

        public IActionResult Error()
        {
            string errorMessage = (string)TempData["ErrorMessage"];

            ViewBag.ErrorMessage = errorMessage;

            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTicketsForCurrentUser([FromQuery] int page = 1)
        {
            DataResponse<TicketPagedViewModel> response;

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Error");
            }

            response = await _ticketService.GetTicketsPagedByUserId(userId, page, _Size);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
                return RedirectToAction("Error");
            }

            ViewBag.Success = TempData["Success"] as string;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;

            response.Data.PageNumber = page;

            return View(response.Data);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTickets([FromQuery] int page = 1,
            [FromQuery] string? userId = null, [FromQuery] Guid? flightId = null)
        {
            DataResponse<TicketPagedViewModel> response;

            if (!string.IsNullOrEmpty(userId))
            {
                response = await _ticketService.GetTicketsPagedByUserId(userId, page, _Size);
            }
            else if (flightId.HasValue)
            {
                response = await _ticketService.GetTicketsPagedByFlightId(flightId.Value, page, _Size);
            }
            else
            {
                response = await _ticketService.GetTicketsPaged(page, _Size);
            }

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
                return RedirectToAction("Error");
            }

            ViewBag.Success = TempData["Success"] as string;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;

            response.Data.PageNumber = page;

            return View(response.Data);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EnterTicketDetails(Guid flightId, int requiredSeats)
        {
            var flightResponse = await _flightService.GetFlight(flightId);

            if (!flightResponse.Success)
            {
                TempData["ErrorMessage"] = flightResponse.ErrorMessage;
                return RedirectToAction("Error");
            }

            var model = new TicketDetailsViewModel
            {
                FlightId = flightId,
                RequiredSeats = requiredSeats,
                Tickets = new List<ReserveTicketDto>(new ReserveTicketDto[requiredSeats])
            };

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTicket(Guid ticketId)
        {
            var response = await _ticketService.GetTicket(ticketId);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
                return RedirectToAction("Error");
            }
            return View(response.Data);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnterTicketDetails(TicketDetailsViewModel model)
        {
            if (!ModelState.IsValid && ModelState.ErrorCount > model.Tickets.Count())
            {
                TempData["ErrorMessage"] = "Invalid data";
                return RedirectToAction("EnterTicketDetails", new { flightId = model.FlightId, requiredSeats = model.RequiredSeats });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Error");
            }

            foreach (var ticket in model.Tickets)
            {
                ticket.FlightId = model.FlightId;
                ticket.UserId = userId;
            }

            var response = await _ticketService.CreateTickets(model.Tickets);

            var session = _PaymentService.Checkout(response.Data, Request.Host, Request.Scheme);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
                return RedirectToAction("Error");
            }

            return Redirect(session.Url);
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            return View();
        }
        [HttpGet("cancel")]
        public IActionResult Cancel()
        {
            return View();
        }
    }
}