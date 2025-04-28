using EasyFly.Domain.Models;
using System.Linq.Expressions;

namespace EasyFly.Domain.Abstractions
{
    public interface IAuditRepository : IRepository<Audit>
    {
        Task<ICollection<Audit>?> GetAllByAsync(Expression<Func<Audit, bool>> condition);
    }
}
