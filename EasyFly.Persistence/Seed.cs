using EasyFly.Domain.Enums;
using EasyFly.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace EasyFly.Persistence
{
    public class Seed
    {
        private readonly ApplicationDbContext _Context;
        private readonly UserManager<User> _UserManager;
        private readonly RoleManager<IdentityRole> _RoleManager;
        private readonly IUserStore<User> _UserStore;

        public Seed(ApplicationDbContext context, UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager, IUserStore<User> userStore)
        {
            _Context = context;
            _UserManager = userManager;
            _RoleManager = roleManager;
            _UserStore = userStore;
        }

        public async Task SeedContext()
        {
            var hasher = new PasswordHasher<User>();

            if (!_Context.Users.Any())
            {
                if (!await _RoleManager.RoleExistsAsync(Role.User.ToString()))
                {
                    await _RoleManager.CreateAsync(new IdentityRole(Role.User.ToString()));
                }

                if (!await _RoleManager.RoleExistsAsync(Role.Admin.ToString()))
                {
                    await _RoleManager.CreateAsync(new IdentityRole(Role.Admin.ToString()));
                }

                User user = new User();

                User admin = new User();

                await _UserStore.SetUserNameAsync(admin, "admin@easyfly.com", default);
                await ((IUserEmailStore<User>)_UserStore).SetEmailAsync(admin, "admin@easyfly.com", default);
                await ((IUserEmailStore<User>)_UserStore).SetEmailConfirmedAsync(admin, true, default);
                await _UserManager.CreateAsync(admin, "Admin@123");

                await _UserManager.AddToRoleAsync(admin, "Admin");

                await _UserStore.SetUserNameAsync(user, "user@easyfly.com", default);
                await ((IUserEmailStore<User>)_UserStore).SetEmailAsync(user, "user@easyfly.com", default);
                await ((IUserEmailStore<User>)_UserStore).SetEmailConfirmedAsync(user, true, default);
                await _UserManager.CreateAsync(user, "User@123");

                await _UserManager.AddToRoleAsync(user, "User");

                List<Airport> airports = new List<Airport>
                {
                    new Airport
                    {
                        Name = "John F. Kennedy International Airport",
                        Latitude = 40.6413m,
                        Longtitude = -73.7781m
                    },
                    new Airport
                    {
                        Name = "Los Angeles International Airport",
                        Latitude = 33.9416m,
                        Longtitude = -118.4085m
                    }
                };

                _Context.Airports.AddRange(airports);
                await _Context.SaveChangesAsync();

                List<Plane> planes = new List<Plane>
                {
                    new Plane
                    {
                        Name = "Boeing 737"
                    },
                    new Plane
                    {
                        Name = "Airbus A320"
                    }
                };

                _Context.Planes.AddRange(planes);
                await _Context.SaveChangesAsync();

                List<Flight> flights = new List<Flight>
                {
                    new Flight
                    {
                        FlightNumber = "AA123",
                        DepartureTime = DateTime.Now.AddHours(2),
                        ArrivalTime = DateTime.Now.AddHours(5),
                        DepartureAirportId = airports[0].Id,
                        ArrivalAirportId = airports[1].Id,
                        PlaneId = planes[0].Id
                    },
                    new Flight
                    {
                        FlightNumber = "UA456",
                        DepartureTime = DateTime.Now.AddHours(3),
                        ArrivalTime = DateTime.Now.AddHours(6),
                        DepartureAirportId = airports[1].Id,
                        ArrivalAirportId = airports[0].Id,
                        PlaneId = planes[1].Id
                    }
                };

                _Context.Flights.AddRange(flights);
                await _Context.SaveChangesAsync();

                List<Seat> seats = new List<Seat>
                {
                    new Seat
                    {
                        Row = 1,
                        SeatLetter = SeatLetter.A,
                        FlightId = flights[0].Id
                    },
                    new Seat
                    {
                        Row = 2,
                        SeatLetter = SeatLetter.B,
                        FlightId = flights[1].Id
                    }
                };

                _Context.Seats.AddRange(seats);
                await _Context.SaveChangesAsync();

                List<Ticket> tickets = new List<Ticket>
                {
                    new Ticket
                    {
                        SeatId = seats[0].Id,
                        PersonType = PersonType.Adult,
                        UserId = user.Id,
                        PersonFirstName = "John",
                        PersonLastName = "Doe"
                    },
                    new Ticket
                    {
                        SeatId = seats[1].Id,
                        PersonType = PersonType.Kid,
                        UserId = admin.Id,
                        PersonFirstName = "Jane",
                        PersonLastName = "Doe"
                    }
                };

                _Context.Tickets.AddRange(tickets);
                await _Context.SaveChangesAsync();
            }
        }
    }
}