namespace EasyFly.Application.ViewModels
{
    public class TicketPagedViewModel
    {
        public IEnumerable<TicketViewModel> Tickets { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
