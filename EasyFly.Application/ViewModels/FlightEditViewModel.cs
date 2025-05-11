namespace EasyFly.Application.ViewModels
{
    public class FlightEditViewModel
    {
        public FlightViewModel FlightViewModel { get; set; }
        public IEnumerable<AirportViewModel> Airports { get; set; }

    }
}
