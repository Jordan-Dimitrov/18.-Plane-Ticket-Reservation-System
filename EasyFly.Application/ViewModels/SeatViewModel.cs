using EasyFly.Domain.Enums;

namespace EasyFly.Application.ViewModels
{
    public class SeatViewModel
    {
        public Guid Id { get; set; }
        public int Row { get; set; }
        public SeatLetter SeatLetter { get; set; }
        public PlaneViewModel Plane { get; set; }
    }
}
