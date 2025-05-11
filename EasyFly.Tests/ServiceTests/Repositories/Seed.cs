using EasyFly.Domain.Models;
using EasyFly.Persistence;

namespace EasyFly.Tests.ServiceTests.Repositories
{
    internal class Seed
    {
        private ApplicationDbContext _context;
        public Seed(ApplicationDbContext context)
        {
            _context = context;
        }
        public void SeedData()
        {
            _context.Airports.RemoveRange(_context.Airports);
            _context.SaveChanges();

            var plane = new Plane { Id = Guid.NewGuid(), Name = "Boeing 737" };

            var airport1 = new Airport
            {
                Id = Guid.NewGuid(),
                Name = "Airport 1"
            };

            var airport2 = new Airport
            {
                Id = Guid.NewGuid(),
                Name = "Airport 2"
            };

            var flight1 = new Flight
            {
                FlightNumber = "FL123",
                DepartureTime = DateTime.UtcNow,
                ArrivalTime = DateTime.UtcNow.AddHours(2),
                DepartureAirport = airport1,
                DepartureAirportId = airport1.Id,
                ArrivalAirport = airport2,
                ArrivalAirportId = airport2.Id,
                Plane = plane,
                PlaneId = plane.Id,
                TicketPrice = 200.00m
            };

            var flight2 = new Flight
            {
                FlightNumber = "FL456",
                DepartureTime = DateTime.UtcNow.AddHours(1),
                ArrivalTime = DateTime.UtcNow.AddHours(3),
                DepartureAirport = airport1,
                DepartureAirportId = airport1.Id,
                ArrivalAirport = airport2,
                ArrivalAirportId = airport2.Id,
                Plane = plane,
                PlaneId = plane.Id,
                TicketPrice = 250.00m
            };

            airport1.Flights = new List<Flight> { flight1, flight2 };
            airport2.Flights = new List<Flight>();

            _context.Add(plane);
            _context.Airports.AddRange(airport1, airport2);
            _context.SaveChanges();

            _context.Audits.RemoveRange(_context.Audits);
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();

            User user = new User
            {
                Id = "user1",
                UserName = "TestUser"
            };

            var audit1 = new Audit
            {
                Message = "Audit message 1",
                ModifiedAt = DateTime.UtcNow,
                User = user,
                UserId = user.Id
            };

            var audit2 = new Audit
            {
                Message = "Audit message 2",
                ModifiedAt = DateTime.UtcNow.AddMinutes(1),
                User = user,
                UserId = user.Id
            };

            var audit3 = new Audit
            {
                Message = "Audit message 3",
                ModifiedAt = DateTime.UtcNow.AddMinutes(2),
                User = user,
                UserId = user.Id
            };

            _context.Users.Add(user);
            _context.Audits.AddRange(audit1, audit2, audit3);
            _context.SaveChanges();
        }
    }
}
