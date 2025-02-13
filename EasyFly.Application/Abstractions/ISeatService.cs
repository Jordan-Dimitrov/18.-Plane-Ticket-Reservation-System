using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.Abstractions
{
    public interface ISeatService
    {
        Task<Response> CreateSeat(SeatDto plane);
        Task<Response> DeleteSeat(Guid id);
        Task<Response> UpdateSeat(SeatDto plane, Guid id);
        Task<DataResponse<SeatViewModel>> GetSeat(Guid id);
        Task<DataResponse<SeatPagedViewModel>> GetSeatsPaged(int page, int size);
        Task<DataResponse<SeatPagedViewModel>> GetSeatsPagedForFlight(Guid flightId, int page, int size);
    }
}
