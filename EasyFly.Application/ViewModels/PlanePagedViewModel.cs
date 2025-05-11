namespace EasyFly.Application.ViewModels
{
    public class PlanePagedViewModel
    {
        public IEnumerable<PlaneViewModel> Planes { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
