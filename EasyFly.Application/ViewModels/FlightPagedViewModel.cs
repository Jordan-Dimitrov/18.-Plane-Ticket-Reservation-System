namespace EasyFly.Application.ViewModels
{
    public class FlightPagedViewModel
    {
        public IEnumerable<FlightViewModel> Flights { get; set; }
        public IEnumerable<AirportViewModel> Airports { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
