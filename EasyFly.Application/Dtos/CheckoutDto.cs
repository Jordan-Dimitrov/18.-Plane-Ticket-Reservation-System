namespace EasyFly.Application.Dtos
{
    public class CheckoutDto
    {
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public long Amount { get; set; }
        public string? Currency { get; set; }
        public List<Guid> Tickets { get; set; }
    }
}
