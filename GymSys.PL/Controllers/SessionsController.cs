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
            var categoriesList = await _sessionService.GetCategorySelectListAsync();
            var trainersList = await _sessionService.GetTrainerSelectListAsync();
            ViewBag.Trainers = new SelectList(trainersList.value, "Id", "Name");
            ViewBag.Categories = new SelectList(categoriesList.value, "Id", "CategoryName");
        }

        public async Task PopulateTrainerSelectList()
        {
            var trainersList = await _sessionService.GetTrainerSelectListAsync();
            ViewBag.Trainers = new SelectList(trainersList.value, "Id", "Name");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var result = await _sessionService.GetSessionByIdAsync(id, ct);
            if (result.success)
            {
                return View(result.value);
            }
            else
            {
                TempData["FailMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }

        }

        #region Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await _sessionService.GetSessionToUpdateAsync(id, ct);
            if (result.success)
            {
                await PopulateTrainerSelectList();
                return View(result.value);
            }
            else
            {
                TempData["FailMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateTrainerSelectList();
                return View(model);
            }

            var result = await _sessionService.UpdateSessionAsync(id, model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["FailMessage"] = result.error;
                await PopulateTrainerSelectList();
                return View(model);
            }
        }
        #endregion

        #region Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _sessionService.GetSessionByIdAsync(id, ct);
            if (result.success)
            {
                return View(result.value);
            }
            else
            {
                TempData["FailMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _sessionService.DeleteSessionAsync(id, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["FailMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
    }
}
