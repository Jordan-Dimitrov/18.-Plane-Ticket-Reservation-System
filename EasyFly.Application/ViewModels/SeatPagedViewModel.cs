namespace EasyFly.Application.ViewModels
{
    public class SeatPagedViewModel
    {
        public IEnumerable<SeatViewModel> Seats { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
