using FribergCarRental.Models;
using FribergCarRental.Services.Authentication;
using FribergCarRental.Services.Base;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRental.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IAuthService authenticationService;

        public AuthController(IAuthService authenticationService) : base(authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var loginDto = new LoginUserDto
            {
                Email = model.Email,
                Password = model.Password
            };

            var success = await authenticationService.AuthenticateAsync(loginDto);

            if (!success)
            {
                ModelState.AddModelError("", "Fel email eller lösenord");
                return View(model);
            }

            return RedirectToAction("Index", "Home");


            //var loginDto = new LoginUserDto
            //{
            //    Email = model.Email,
            //    Password = model.Password
            //};

            //try
            //{
            //    // Anropar API via AuthenticationService
            //    var result = await authenticationService.AuthenticateAsync(loginDto);

            //    if (!result)
            //    {
            //        ModelState.AddModelError("", "Felaktig e-post eller lösenord.");
            //        return View(model);
            //    }

            //    // Hämta användar-id från authentication service
            //    var userId = await authenticationService.GetUserId();

            //    // Skapa cookie med claims
            //    var claims = new List<Claim>
            //    {
            //        new Claim(ClaimTypes.Name, model.Email),
            //        new Claim(ClaimTypes.NameIdentifier, userId!)
            //    };

            //    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            //    var principal = new ClaimsPrincipal(identity);

            //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            //    return RedirectToAction("Index", "Home");
            //}
            //catch
            //{
            //    ModelState.AddModelError("", "Inloggningen misslyckades.");
            //    return View(model);
            //}
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            authenticationService.Logout();
            return RedirectToAction("Index", "Home");
        }
    }
}
