namespace EasyFly.Application.ViewModels
{
    public class AuditPagedViewModel
    {
        public IEnumerable<AuditViewModel> AuditViewModels { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
