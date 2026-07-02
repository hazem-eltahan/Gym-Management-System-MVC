using GymSys.BLL.ViewModels.AccountViewModels;
using GymSys.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymSys.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct)
        {
            if(!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("InvalidLogin", "Invalid email or password!");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                _logger.LogInformation($"User {user.UserName} is signed in.");
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning($"User {user.UserName} is locked out");
                ModelState.AddModelError("InvalidLogin", "This account is locked out!, Try again later.");
                return View(model);
            }
            else
            {
                ModelState.AddModelError("InvalidLogin", "Invalid email or password!");
                return View(model);
            }
        }
    }
}
