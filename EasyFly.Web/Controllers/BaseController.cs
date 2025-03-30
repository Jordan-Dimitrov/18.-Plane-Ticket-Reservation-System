using EasyFly.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyFly.Web.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        public readonly IAuditService AuditService;

        public BaseController(IAuditService auditService)
        {
            AuditService = auditService;
        }
    }
}
