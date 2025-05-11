namespace EasyFly.Domain.Models
{
    public abstract class SoftDeletableEntity
    {
        public DateTime? DeletedAt { get; set; }
    }
}
