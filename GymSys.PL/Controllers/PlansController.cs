using GymSys.BLL.Services.Interfaces;
using GymSys.DAL.Data.Models;
using GymSys.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    }
}
