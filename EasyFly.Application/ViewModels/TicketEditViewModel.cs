using EasyFly.Application.Dtos;

namespace EasyFly.Application.ViewModels
{
    public class TicketEditViewModel
    {
        public TicketViewModel TicketViewModel { get; set; }
        public IEnumerable<SeatViewModel> Seats { get; set; }
        public IEnumerable<FlightViewModel> Flights { get; set; }
        public IEnumerable<UserDto> Users { get; set; }
    }
}
