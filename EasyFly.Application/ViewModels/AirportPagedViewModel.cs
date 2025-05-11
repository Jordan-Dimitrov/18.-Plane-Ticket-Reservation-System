namespace EasyFly.Application.ViewModels
{
    public class AirportPagedViewModel
    {
        public IEnumerable<AirportViewModel> Airports { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
