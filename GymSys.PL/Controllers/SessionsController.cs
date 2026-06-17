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
            await PopulateTrainerAndCategorySelectList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateTrainerAndCategorySelectList();
                return View(model);
            }
            var result = await _sessionService.CreateSessionAsync(model, ct);

            if (result.success)
            {
                TempData["SuccessMessage"] = "Session created successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["FailMessage"] = result.error;
                await PopulateTrainerAndCategorySelectList();
                return View(model);
            }
        }
        #endregion

        public async Task PopulateTrainerAndCategorySelectList()
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerSelectListAsync(), "Id", "Name");
            ViewBag.Categories = new SelectList(await _sessionService.GetCategorySelectListAsync(), "Id", "CategoryName");
        }
    }
}
