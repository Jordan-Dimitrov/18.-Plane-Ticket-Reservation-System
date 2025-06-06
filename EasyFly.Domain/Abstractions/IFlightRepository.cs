﻿using EasyFly.Domain.Models;

namespace EasyFly.Domain.Abstractions
{
    public interface IFlightRepository : IRepository<Flight>
    {
        Task<IEnumerable<Flight>> GetPagedByArrivalAirportIdAsync(Guid airpordId, bool trackChanges, int page, int size);
        Task<IEnumerable<Flight>> GetPagedByDepartingAirportIdAsync(Guid airpordId, bool trackChanges, int page, int size);
        Task<IEnumerable<Flight>> GetPagedByPlaneIdAsync(Guid planeId, bool trackChanges, int page, int size);
        Task<IEnumerable<Flight>> GetPagedByArrivalAndDepartureAsync(Guid departureId, Guid arrivalId, DateTime departure, bool trackChanges, int requiredSeats, int page, int size);
        Task<IEnumerable<Flight>> GetPagedByArrivalAndDepartureAirportsAsync(Guid departureId, Guid arrivalId, bool trackChanges, int requiredSeats, int page, int size);
        Task<IEnumerable<Flight>> GetPagedByArrivalAndDepartureWithoutConcreteDateAsync(Guid departureId, Guid arrivalId, DateTime departure, bool trackChanges, int requiredSeats, int page, int size);
    }
}
