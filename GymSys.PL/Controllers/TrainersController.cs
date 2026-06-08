using GymSys.BLL.Services.Interfaces;
using GymSys.BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymSys.PL.Controllers
{
    public class TrainersController : Controller
    {
        private readonly ITrainerService _trainerService;

        public TrainersController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var trainers = await _trainerService.GetAllTrainersAsync(ct);
            return View(trainers);
        }
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerDetailsAsync(id, ct);
            if (trainer is null)
            {
                TempData["FailMessage"] = "Trainer not found!";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        #region Create
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel model, CancellationToken ct)
        {
            if(!ModelState.IsValid) return View(nameof(Create), model);

            var result = await _trainerService.CreateTrainerAsync(model, ct);

            if (result)
                TempData["SuccessMessage"] = "Trainer created successfully!";
            else
                TempData["FailMessage"] = "Failed to create trainer!";

            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
