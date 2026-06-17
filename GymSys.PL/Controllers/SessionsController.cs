using GymSys.BLL.Services.Interfaces;
using GymSys.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymSys.PL.Controllers
{
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions = await _sessionService.GetAllSessionsAsync(ct);
            return View(sessions);
        }

        #region Create
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerSelectListAsync(ct), "Id", "Name");
            ViewBag.Categories = new SelectList(await _sessionService.GetCategorySelectListAsync(ct), "Id", "CategoryName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _sessionService.CreateSessionAsync(model, ct);

            if (result)
            {
                TempData["SuccessMessage"] = "Session created successfully!";
            }
            else
                TempData["FailMessage"] = "Creating session failed!";

            return RedirectToAction(nameof(Index));

        }
        #endregion
    }
}
