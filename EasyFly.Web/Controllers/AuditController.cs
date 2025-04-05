using EasyFly.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyFly.Web.Controllers
{
    public class AuditController : BaseController
    {
        private const int _Size = 30;
        public AuditController(IAuditService auditService) : base(auditService)
        {
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index([FromQuery] int page = 1)
        {
            var response = await AuditService.GetAuditsPaged(page, _Size);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
                return RedirectToAction("Error");
            }

            string success = (string)TempData["Success"];

            ViewBag.Success = success;

            string error = (string)TempData["ErrorMessage"];

            ViewBag.ErrorMessage = error;

            response.Data.PageNumber = page;

            return View(response.Data);
        }
    }
}
