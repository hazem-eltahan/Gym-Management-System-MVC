using GymSys.BLL.Services.Interfaces;
using GymSys.BLL.ViewModels.PlanViewModels;
using GymSys.DAL.Data.Models;
using GymSys.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymSys.PL.Controllers
{
    public class PlansController : Controller
    {
        private readonly IPlanService _planService;

        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _planService.GetAllPlansAsync(ct);
            return View(plans);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await _planService.GetPlanDetailsByIdAsync(id, ct);
            if (plan is null)
            {
                TempData["FailMessage"] = "Plan not found!";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        #region Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var plan = await _planService.GetPlanToUpdateAsync(id,ct);
            if (plan is null)
            {
                TempData["FailMessage"] = "Plan can't be edited! : Not found, Inactive or has active memberships.";

                return RedirectToAction(nameof(Index));
            }

            return View(plan);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute]int id, UpdatePlanViewModel model, CancellationToken ct)
        {
            if(!ModelState.IsValid) return View(model);

            var result = await _planService.UpdatePlanAsync(id,model,ct);

            if (result)
                TempData["SuccessMessage"] = "Plan updated successfully!";
            else
                TempData["FailMessage"] = "Update plan failed!";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            var result = await _planService.ToggleActivationAsync(id,ct);

            if (result)
                TempData["SuccessMessage"] = "Plan status changed!";
            else
                TempData["FailMessage"] = "Failed to change plan status!";

            return RedirectToAction(nameof(Index));
        }
    }
}
