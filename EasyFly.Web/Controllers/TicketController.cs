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
using NuGet.Protocol;
using Newtonsoft.Json;

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

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            await AuditService.CreateAudit(Guid.Parse(currentUserId), new AuditDto()
            {
                ModifiedAt = DateTime.UtcNow,
                Message = $"Started modifying {JsonConvert.SerializeObject(response.TicketViewModel)}"
            });

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

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            await AuditService.CreateAudit(Guid.Parse(currentUserId), new AuditDto()
            {
                ModifiedAt = DateTime.UtcNow,
                Message = $"Modified {ticketId}"
            });

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
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            await AuditService.CreateAudit(Guid.Parse(currentUserId), new AuditDto()
            {
                ModifiedAt = DateTime.UtcNow,
                Message = $"Started deleting {JsonConvert.SerializeObject(response.Data)}"
            });


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

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            await AuditService.CreateAudit(Guid.Parse(currentUserId), new AuditDto()
            {
                ModifiedAt = DateTime.UtcNow,
                Message = $"Deleted{ticketId}"
            });

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

            response = await _ticketService.GetTicketsPagedByUserId(userId, page, _Size, null, null, null);

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
            [FromQuery] string? search = null,
            [FromQuery] string? typeFilter = null,
            [FromQuery] string? luggageFilter = null,
            [FromQuery] string? userId = null,
            [FromQuery] Guid? flightId = null)
        {
            DataResponse<TicketPagedViewModel> response;

            if (!string.IsNullOrEmpty(userId))
            {
                response = await _ticketService.
                    GetTicketsPagedByUserId(userId, page, _Size * 3, search, typeFilter, luggageFilter);
            }
            else if (flightId.HasValue)
            {
                response = await _ticketService
                               .GetTicketsPagedByFlightId(flightId.Value, page, _Size * 3, search, typeFilter, luggageFilter);
            }
            else
            {
                response = await _ticketService
                               .GetTicketsPaged(page, _Size * 3, search, typeFilter, luggageFilter);
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
        public async Task<IActionResult> EnterTicketDetails(Guid flightId, int requiredSeats, Guid? returningFlightId)
        {
            var flightResponse = await _flightService.GetFlight(flightId);

            if (!flightResponse.Success)
            {
                TempData["ErrorMessage"] = flightResponse.ErrorMessage;
                return RedirectToAction("Error");
            }

            decimal sum = 0;

            if (returningFlightId.HasValue)
            {
                var returningFlightResponse = await _flightService.GetFlight(returningFlightId.GetValueOrDefault());

                if (!returningFlightResponse.Success)
                {
                    TempData["ErrorMessage"] = returningFlightResponse.ErrorMessage;
                    return RedirectToAction("Error");
                }

                sum = returningFlightResponse.Data.TicketPrice;
            }

            var model = new TicketDetailsViewModel
            {
                FlightId = flightId,
                ReturningFlightId = returningFlightId,
                RequiredSeats = requiredSeats,
                Tickets = new List<ReserveTicketDto>(new ReserveTicketDto[requiredSeats]),
                TicketPrice = flightResponse.Data.TicketPrice,
                ReturningTicketPrice = sum
            };

            return View(model);
        }

        [HttpGet]
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
                return RedirectToAction("EnterTicketDetails", new
                {
                    flightId = model.FlightId,
                    returningFlightId = model.ReturningFlightId,
                    requiredSeats = model.RequiredSeats
                });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Error");
            }

            if (!model.Tickets.Any(x => x.PersonType == Domain.Enums.PersonType.Adult))
            {
                TempData["ErrorMessage"] = "Kids cannot travel alone";
                return RedirectToAction("EnterTicketDetails",
                    new { flightId = model.FlightId, requiredSeats = model.RequiredSeats, returningFlightId = model.ReturningFlightId });
            }

            var mainResponse = new DataResponse<CheckoutDto>();
            mainResponse.Data = new CheckoutDto();

            if (model.ReturningFlightId.HasValue)
            {
                var clone = model.Tickets.ToList();
                foreach (var ticket in model.Tickets)
                {
                    ticket.FlightId = model.FlightId;
                    ticket.UserId = userId;
                }

                var response = await _ticketService.CreateTickets(model.Tickets);

                foreach (var ticket in clone)
                {
                    ticket.FlightId = model.ReturningFlightId.Value;
                    ticket.UserId = userId;
                }

                var response2 = await _ticketService.CreateTickets(clone);

                mainResponse.Data.Amount = response.Data.Amount + response2.Data.Amount;
                mainResponse.Data.Currency = response.Data.Currency;
                mainResponse.Success = response.Success;
                mainResponse.Data.ProductName = response.Data.ProductName;
                mainResponse.Data.ProductDescription = response.Data.ProductDescription;
                mainResponse.Data.Tickets = response.Data.Tickets;
                mainResponse.Data.Tickets.AddRange(response2.Data.Tickets);
            }
            else
            {
                foreach (var ticket in model.Tickets)
                {
                    ticket.FlightId = model.FlightId;
                    ticket.UserId = userId;
                }

                var response = await _ticketService.CreateTickets(model.Tickets);

                mainResponse.Data.Amount = response.Data.Amount;
                mainResponse.Data.Currency = response.Data.Currency;
                mainResponse.Success = response.Success;
                mainResponse.Data.ProductName = response.Data.ProductName;
                mainResponse.Data.ProductDescription = response.Data.ProductDescription;
                mainResponse.Data.Tickets = response.Data.Tickets;
            }

            var session = _PaymentService.Checkout(mainResponse.Data, Request.Host, Request.Scheme);

            if (!mainResponse.Success)
            {
                TempData["ErrorMessage"] = mainResponse.ErrorMessage;
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