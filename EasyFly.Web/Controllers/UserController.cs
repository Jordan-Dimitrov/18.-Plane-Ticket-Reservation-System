using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyFly.Web.Controllers
{
    public class UserController : BaseController
    {
        private const int _Size = 10;
        private readonly IUserService _userService;

        public UserController(IUserService userService, IAuditService auditService)
            : base(auditService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid userId)
        {
            var response = await _userService.GetUser(userId);

            if (!response.Success)
            {
                ViewBag.ErrorMessage = response.ErrorMessage;
                return View();
            }

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            await AuditService.CreateAudit(Guid.Parse(currentUserId), new AuditDto()
            {
                ModifiedAt = DateTime.UtcNow,
                Message = $"Started modifying {response.Data.Id} - {response.Data.Username}," +
                    $" {response.Data.PhoneNumber ?? ""}," +
                    $" {response.Data.Email}"
            });

            return View(response.Data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserDto dto, Guid userId)
        {
            if (!ModelState.IsValid && ModelState.ErrorCount > 1)
            {
                TempData["ErrorMessage"] = "Invalid data";
                return RedirectToAction("Edit", new { userId = userId });
            }

            var response = await _userService.UpdateUser(dto, userId);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Successfully updated";
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                await AuditService.CreateAudit(Guid.Parse(currentUserId), new AuditDto()
                {
                    ModifiedAt = DateTime.UtcNow,
                    Message = $"Modified {dto.Id} - {dto.Username}," +
                    $" {dto.PhoneNumber ?? ""}," +
                    $" {dto.Email}"
                });
            }

            return RedirectToAction("GetUsers", new { page = 1 });
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remove(Guid userId)
        {
            var response = await _userService.GetUser(userId);

            if (!response.Success)
            {
                ViewBag.ErrorMessage = response.ErrorMessage;
                return View();
            }

            return View(response.Data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid userId)
        {
            var response = await _userService.DeleteUser(userId);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Successfully deleted";
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                await AuditService.CreateAudit(Guid.Parse(currentUserId), new AuditDto()
                {
                    ModifiedAt = DateTime.UtcNow,
                    Message = $"Removed {userId}"
                });
            }

            return RedirectToAction("GetUsers", new { page = 1 });
        }

        public IActionResult Error()
        {
            string errorMessage = (string)TempData["ErrorMessage"];

            ViewBag.ErrorMessage = errorMessage;

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1)
        {
            var response = await _userService.GetUsersPaged(page, _Size);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
                return RedirectToAction("Error");
            }

            string success = (string)TempData["Success"];

            ViewBag.Success = success;

            string error = (string)TempData["ErrorMessage"];

            response.Data.PageNumber = page;

            ViewBag.ErrorMessage = error;

            return View(response.Data);
        }
    }
}