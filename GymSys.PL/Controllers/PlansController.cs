using GymSys.DAL.DbContexts;
using GymSys.DAL.Repositories.Classes;
using GymSys.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymSys.PL.Controllers
{
    public class PlansController : Controller
    {
        private readonly IPlanRepository planRepository;

        public PlansController(IPlanRepository planRepository)
        {
            this.planRepository = planRepository;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await planRepository.GetAllAsync(ct: ct);
            return View(plans);
        }

        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await planRepository.GetByIdAsync(id, ct);
            if (plan is null)
                return RedirectToAction(nameof(Index));
            return View(plan);
        }
    }
}
