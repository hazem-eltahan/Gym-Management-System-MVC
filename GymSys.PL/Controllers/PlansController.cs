using GymSys.BLL.Services.Interfaces;
using GymSys.BLL.ViewModels.PlanViewModels;
using GymSys.DAL.Data.Models;
using GymSys.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymSys.PL.Controllers
{
    [Authorize]
    public class PlansController : Controller
    {
        private readonly IPlanService _planService;

        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var result = await _planService.GetAllPlansAsync(ct);
            return View(result.value);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var result = await _planService.GetPlanDetailsByIdAsync(id, ct);
            if (!result.success)
            {
                TempData["FailMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
            return View(result.value);
        }

        #region Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await _planService.GetPlanToUpdateAsync(id,ct);
            if (!result.success)
            {
                TempData["FailMessage"] = result.error;

                return RedirectToAction(nameof(Index));
            }

            return View(result.value);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute]int id, UpdatePlanViewModel model, CancellationToken ct)
        {
            if(!ModelState.IsValid) return View(model);

            var result = await _planService.UpdatePlanAsync(id,model,ct);

            if (result.success)
                TempData["SuccessMessage"] = "Plan updated successfully!";
            else
                TempData["FailMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            var result = await _planService.ToggleActivationAsync(id,ct);

            if (result.success)
                TempData["SuccessMessage"] = "Plan status changed!";
            else
                TempData["FailMessage"] = result.error;

            return RedirectToAction(nameof(Index));
        }
    }
}
