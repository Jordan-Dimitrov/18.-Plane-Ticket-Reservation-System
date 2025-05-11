using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyFly.Web.Controllers
{
    public class PlaneController : BaseController
    {
        private const int _Size = 30;
        private readonly IPlaneService _PlaneService;

        public PlaneController(IPlaneService planeService, IAuditService auditService)
            : base(auditService)
        {
            _PlaneService = planeService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlaneDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data";
                return RedirectToAction("GetPlanes", new { page = 1 });
            }

            var response = await _PlaneService.CreatePlane(dto);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Successfully created";
            }

            return RedirectToAction("GetPlanes", new { page = 1 });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remove(Guid planeId)
        {
            var response = await _PlaneService.GetPlane(planeId);

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
        public async Task<IActionResult> Delete(Guid planeId)
        {
            var response = await _PlaneService.DeletePlane(planeId);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Successfully deleted";
            }

            return RedirectToAction("GetPlanes", new { page = 1 });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid planeId)
        {
            var response = await _PlaneService.GetPlane(planeId);

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
        public async Task<IActionResult> Edit(PlaneDto dto, Guid planeId)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data";
                return RedirectToAction("Edit", new { planeId = planeId });
            }

            var response = await _PlaneService.UpdatePlane(dto, planeId);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.ErrorMessage;
            }
            else
            {
                TempData["Success"] = "Successfully updated";
            }

            return RedirectToAction("GetPlanes", new { page = 1 });
        }

        public IActionResult Error()
        {
            string errorMessage = (string)TempData["ErrorMessage"];

            ViewBag.ErrorMessage = errorMessage;

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPlanes([FromQuery] int page = 1)
        {
            var response = await _PlaneService.GetPlanesPaged(page, _Size);

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